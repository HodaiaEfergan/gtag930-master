#region

using System.Collections.Generic;

#endregion

namespace Web.DTO
{
    public class UnitEditDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<UserDto> AssignedUsers { get; set; }
        public string HexRgbColor { get; set; }
    }
}