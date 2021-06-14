#region

using System;
using System.Collections.Generic;

#endregion

namespace Web.DTO
{
    public class UnitScanDto
    {
        public string Id { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public virtual UnitInfoDto UnitInfo { get; set; }
        public string Description { get; set; }
        public string ImageJpegBase64 { get; set; }
        public decimal? BatteryVolt { get; set; }
        public bool Connection { get; set; }
        public decimal? RSSI { get; set; }
        public string NetworkConnection { get; set; }
        public decimal? TemperatureSensor1Celsius { get; set; }
        public decimal? TemperatureSensor2Celsius { get; set; }
        public decimal? GpsLatitude { get; set; }
        public decimal? GpsLongitude { get; set; }
        public List<TagScanDto> TagScans { get; set; } = new();
        public string OwnerId { get; set; }
        public UserDto Owner { get; set; }
        public UnitScanStateDto State { get; set; }
    }
}