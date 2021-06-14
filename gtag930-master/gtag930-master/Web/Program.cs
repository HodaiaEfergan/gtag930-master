#region

using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

#endregion

namespace Web
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().MinimumLevel
                .Override("Microsoft", LogEventLevel.Information).Enrich.FromLogContext().WriteTo.Console().WriteTo
                .File("logs/log.txt", rollingInterval: RollingInterval.Day).CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).UseSerilog().ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, 5000);
                    options.Listen(IPAddress.Any, 5001, listenOptions =>
                    {
                        listenOptions.UseHttps();
                    });
                });
            });
        }
    }
}