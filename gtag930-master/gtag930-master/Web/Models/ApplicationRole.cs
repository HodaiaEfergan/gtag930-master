#region

using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

#endregion

namespace Web.Models
{
    public class ApplicationRole : IdentityRole
    {
        public const string AdminRoleName = "Admin";

        public ApplicationRole()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
        }

        public virtual List<ApplicationUserRole> UserRoles { get; set; } = new();
    }
}