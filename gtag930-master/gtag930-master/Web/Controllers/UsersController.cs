#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Data;
using Web.DTO;
using Web.Models;

#endregion

namespace Web.Controllers
{
    [Authorize(Roles = ApplicationRole.AdminRoleName)]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager,
            ILogger<UsersController> logger)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<List<UserDto>> Index()
        {
            return _mapper.Map<List<UserDto>>(_context.Users.ToList());
        }

        [HttpDelete("{id}")]
        public ActionResult<string> DeleteUser(string id)
        {
            var userToDelete = _context.Users.Single(x => x.Id == id);
            _context.Users.Remove(userToDelete);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> EditUser([FromBody] UserEditDTO editedUser)
        {
            var userToEdit = await _userManager.FindByIdAsync(editedUser.Id);

            if (editedUser.IsLocked)
            {
                _logger.LogInformation("locking out user id: " + editedUser.Id);
                await _userManager.SetLockoutEndDateAsync(userToEdit, DateTimeOffset.MaxValue);
            }
            else if (await _userManager.IsLockedOutAsync(userToEdit))
            {
                _logger.LogInformation("unlocking user id: " + editedUser.Id);
                await _userManager.SetLockoutEndDateAsync(userToEdit, DateTimeOffset.Now);
            }

            switch (editedUser.Role)
            {
                case "Regular":
                    await _userManager.RemoveFromRoleAsync(userToEdit, ApplicationRole.AdminRoleName);
                    break;
                case "Admin":
                    await _userManager.AddToRoleAsync(userToEdit, ApplicationRole.AdminRoleName);
                    break;
                default:
                    throw new Exception("Unknown role selected");
            }

            return Ok();
        }
    }
}