namespace Web.DTO
{
    public class TagScanDto
    {
        public string Id { get; set; }
        public string TagId { get; set; }
        public decimal? Rssi { get; set; }
        public string UnitScanId { get; set; }
    }
}