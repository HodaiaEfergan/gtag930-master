#region

using System;
using System.Collections.Generic;

#endregion

namespace Web.Models
{
    public class UnitScan
    {
        public string Id { get; set; }
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        public virtual UnitInfo UnitInfo { get; set; }
        public string UnitInfoId { get; set; }
        public string Description { get; set; }
        public byte[] ImageJpeg { get; set; }
        public decimal? BatteryVolt { get; set; }
        public bool Connection { get; set; }
        public decimal? RSSI { get; set; }
        public string NetworkConnection { get; set; }
        public decimal? TemperatureSensor1Celsius { get; set; }
        public decimal? TemperatureSensor2Celsius { get; set; }
        public decimal? GpsLatitude { get; set; }
        public decimal? GpsLongitude { get; set; }
        public virtual List<TagScan> TagScans { get; set; } = new();
        public string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public UnitScanState State { get; set; }
    }
}