namespace Web.Models
{
    public class TagScan
    {
        public string Id { get; set; }
        public string TagId { get; set; }
        public decimal? Rssi { get; set; }
        public string UnitScanId { get; set; }
        public virtual UnitScan UnitScan { get; set; }
    }
}