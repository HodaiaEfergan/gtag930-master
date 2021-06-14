#region

using System.Collections.Generic;

#endregion

namespace Web.DTO
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public List<RoleDto> Roles { get; set; } = new();
        public bool IsLocked { get; set; }
    }
}