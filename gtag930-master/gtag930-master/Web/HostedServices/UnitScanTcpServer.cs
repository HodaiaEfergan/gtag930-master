#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Web.Data;
using Web.Models;

#endregion

namespace Web.HostedServices
{
    public class UnitScanTcpServer : BackgroundService
    {
        private readonly ILogger<UnitScanTcpServer> _logger;
        private readonly TcpListener _server;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UnitScanTcpServer(ILogger<UnitScanTcpServer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;

            var localAddr = IPAddress.Parse("0.0.0.0");
            _server = new TcpListener(localAddr, 9870);
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hosted service starting");

            _server.Start();

            return Task.Factory.StartNew(async () =>
            {
                // loop until a cancellation is requested
                while (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Hosted service executing - {0}", DateTime.Now);
                    try
                    {
                        _logger.LogInformation("Waiting for a connection...");
                        var client = await _server.AcceptTcpClientAsync();
                        _logger.LogInformation("Connected!");
                        var t = new Thread(HandleConnection);
                        t.Start(client);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            }, cancellationToken);
        }

        private void HandleConnection(object obj)
        {
            using var client = (TcpClient) obj;
            var stream = client.GetStream();
            using var sr = new StreamReader(stream);
            using var sw = new StreamWriter(stream) {AutoFlush = true};
            try
            {
                var line = sr.ReadLine();
                _logger.LogInformation("got line: " + line);

                var newUnitScan = HandleScanString(line);

                var decodedMessage = "decoded unit scan and inserted in DB. new id: " + newUnitScan.Id;
                _logger.LogInformation(decodedMessage);

                sw.WriteLine("OK");
                sw.WriteLine(decodedMessage);
            }
            catch (Exception e1)
            {
                var errMessage = "error while handling incoming connection: " + e1;
                _logger.LogError(errMessage);
                try
                {
                    sw.WriteLine(errMessage);
                }
                catch (Exception e2)
                {
                    _logger.LogWarning("could not write exception to socket: " + e2);
                }
            }
        }

        private UnitScan HandleScanString(string line)
        {
            var r = new Regex(@"UID(.*)UBAT(.*)MVOLIND(.*)URSSI(.*),(.*)NETCON(.*)MCUTMP(.*)EXTTMP(.*)LOC(.*)");
            var groups = r.Match(line).Groups;

            using var scope = _serviceScopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var unitId = groups[1].Value;
            var unitInfo = context.UnitInfos.SingleOrDefault(x => x.Id == unitId) ??
                           context.UnitInfos.Single(x => x.Id == UnitInfo.DEFAULT_ID);

            //location and tags
            var locationAndTags = groups[9].Value.Split("TAGS");
            var location = locationAndTags[0];
            var tags = locationAndTags.Length == 2 ? locationAndTags[1] : string.Empty;

            var parsedGpsCoordinates = TryParseGpsCoordinates(location);
            var unitScan = new UnitScan
            {
                UnitInfo = unitInfo,
                BatteryVolt = decimal.Parse(groups[2].Value) / 1000,
                Connection = groups[3].Value switch
                {
                    "Ext_ON" => true,
                    "Ext_OFF" => false,
                    _ => throw new Exception("Unknown Connection Indication")
                },
                RSSI = decimal.Parse(groups[4].Value),
                NetworkConnection = groups[6].Value.Replace("\"", ""),
                TemperatureSensor1Celsius = ParseTemperature(groups[7].Value),
                TemperatureSensor2Celsius = ParseTemperature(groups[8].Value),
                GpsLatitude = parsedGpsCoordinates?.Latitude,
                GpsLongitude = parsedGpsCoordinates?.Longitude,
                TagScans = ParseTagScans(tags).ToList(),
                OwnerId = null,
                State = UnitScanState.None
            };

            // add to db
            context.UnitScans.Add(unitScan);
            context.SaveChanges();

            return unitScan;
        }

        private IEnumerable<TagScan> ParseTagScans(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) yield break;

            var tagScans = value.Split(",");
            foreach (var tagScan in tagScans)
            {
                var r = new Regex(@"TID(.*)TRSSI-(.*)");
                var groups = r.Match(tagScan).Groups;
                var tagId = groups[1].Value.Trim();
                if (string.IsNullOrWhiteSpace(tagId))
                    continue;
                if (!decimal.TryParse(groups[2].Value, out var rssi))
                    continue;
                yield return new TagScan {TagId = tagId, Rssi = rssi};
            }
        }

        // example: $GPGGA,053527.000,3149.1482,N,03515.2412,E,1,06,1.4,686.3,M, $SPEED
        private ParsedGpsCoordinates TryParseGpsCoordinates(string value)
        {
            try
            {
                var parts = value.Split(",");
                var latPart = parts[2];
                var lonPart = parts[4];
                return new ParsedGpsCoordinates
                {
                    Latitude = ddmToDd(decimal.Parse(latPart.Substring(0, 2)), decimal.Parse(latPart.Substring(2)),
                        char.Parse(parts[3])),
                    Longitude = ddmToDd(decimal.Parse(lonPart.Substring(0, 3)), decimal.Parse(lonPart.Substring(3)),
                        char.Parse(parts[5]))
                };
            }
            catch (Exception)
            {
                _logger.LogDebug("could not parse GPS coordinates: LOC line part: " + value);
                return null;
            }
        }

        /// <summary>
        ///     "Degrees and decimal minutes" to "Decimal degrees"
        /// </summary>
        private decimal ddmToDd(decimal degrees, decimal minutes, char direction)
        {
            var val = degrees + minutes / 60;
            return direction == 'N' || direction == 'E' ? val : -val;
        }

        private decimal ParseTemperature(string value)
        {
            return decimal.Parse(value.Replace("TPM", "").Replace("TPS", ""));
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background service stopping");
            await Task.CompletedTask;
        }
    }
}