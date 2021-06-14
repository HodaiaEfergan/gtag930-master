namespace Web.Models
{
    public class ApplicationUserUnitInfo
    {
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string UnitInfoId { get; set; }
        public virtual UnitInfo UnitInfo { get; set; }
    }
}