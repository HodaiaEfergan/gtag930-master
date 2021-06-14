#region

using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

#endregion

namespace Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
        }

        public ApplicationUser(string userName) : base(userName)
        {
        }

        public virtual List<ApplicationUserUnitInfo> AssignedUnits { get; set; } = new();
        public virtual List<ApplicationUserRole> UserRoles { get; set; } = new();
        public virtual List<UnitScan> UnitScans { get; set; } = new();
    }
}