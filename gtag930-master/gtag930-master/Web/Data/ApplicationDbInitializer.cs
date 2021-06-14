#region

using Microsoft.AspNetCore.Identity;
using Web.Models;

#endregion

namespace Web.Data
{
    public static class ApplicationDbInitializer
    {
        public static void SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            if (roleManager.RoleExistsAsync(ApplicationRole.AdminRoleName).Result) return;

            roleManager.CreateAsync(new ApplicationRole(ApplicationRole.AdminRoleName)).Wait();
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            AddAdmin(userManager, "orentet@gmail.com", "Gtag123!");
            AddAdmin(userManager, "hezi.nis@8tec.co.il", "Gtag123!");
        }

        private static void AddAdmin(UserManager<ApplicationUser> userManager, string email, string password)
        {
            if (userManager.FindByEmailAsync(email).Result != null) return;

            var user = new ApplicationUser {UserName = email, Email = email};

            var result = userManager.CreateAsync(user, password).Result;

            if (result.Succeeded) userManager.AddToRoleAsync(user, ApplicationRole.AdminRoleName).Wait();
        }
    }
}