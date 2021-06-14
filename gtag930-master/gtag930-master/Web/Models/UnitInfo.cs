#region

using System.Collections.Generic;

#endregion

namespace Web.Models
{
    public class UnitInfo
    {
        public const string DEFAULT_ID = "DEFAULT_UNIT_INFO_ID";
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual List<UnitScan> UnitScans { get; set; } = new();
        public virtual List<ApplicationUserUnitInfo> AssignedUsers { get; set; } = new();
        public string HexRgbColor { get; set; } = "FF0000";
    }
}