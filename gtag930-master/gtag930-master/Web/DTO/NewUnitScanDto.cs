#region

using System.Collections.Generic;

#endregion

namespace Web.DTO
{
    public class NewUnitScanDto
    {
        public string UnitId { get; set; }
        public List<string> TagIds { get; set; }
        public string Description { get; set; }
        public string ImageJpegBase64 { get; set; }
        public decimal? GpsLatitude { get; set; }
        public decimal? GpsLongitude { get; set; }
        public UnitScanStateDto State { get; set; }
    }
}