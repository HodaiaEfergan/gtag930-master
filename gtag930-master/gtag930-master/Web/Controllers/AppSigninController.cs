#region

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Models;

#endregion

namespace Web.Controllers
{
    public class UserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    [Route("[controller]")]
    public class AppSigninController : Controller
    {
        private readonly ILogger<AppSigninController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppSigninController(SignInManager<ApplicationUser> signInManager, ILogger<AppSigninController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] UserModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return Ok();
                }

                return BadRequest("Bad Credentials");
            }

            return BadRequest(ModelState.ErrorCount);
        }
    }
}