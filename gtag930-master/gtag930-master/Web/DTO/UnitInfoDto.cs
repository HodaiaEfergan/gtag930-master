#region

using System.Collections.Generic;

#endregion

namespace Web.DTO
{
    public class UnitInfoDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<UserDto> AssignedUsers { get; set; } = new();
        public string HexRgbColor { get; set; }
    }
}