#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.DTO;
using Web.Models;

#endregion

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UnitsController(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<List<UnitInfoDto>> Index(bool onlyOwn = false)
        {
            var userId = _userManager.GetUserId(User);

            IQueryable<UnitInfo> units = _context.UnitInfos;
            if (onlyOwn)
            {
                units = units.Where(x => x.AssignedUsers.Any(y => y.ApplicationUserId == userId));
            }
            else
            {
                if (!User.IsInRole(ApplicationRole.AdminRoleName))
                    throw new Exception("Only admins can see all the units");
            }

            var unitInfos = units.ToList();
            return _mapper.Map<List<UnitInfoDto>>(unitInfos);
        }


        [Authorize(Roles = ApplicationRole.AdminRoleName)]
        [HttpDelete("{id}")]
        public ActionResult<string> DeleteUnit(string id)
        {
            var unitToDelete = _context.UnitInfos.Single(x => x.Id == id);
            _context.UnitInfos.Remove(unitToDelete);
            _context.SaveChanges();

            return Ok();
        }

        [Authorize(Roles = ApplicationRole.AdminRoleName)]
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> EditUnit([FromBody] UnitEditDto editedUnit)
        {
            var unitToEdit = _context.UnitInfos.Single(x => x.Id == editedUnit.Id);
            unitToEdit.Id = editedUnit.Id;
            unitToEdit.Name = editedUnit.Name;
            unitToEdit.HexRgbColor =
                string.IsNullOrWhiteSpace(unitToEdit.HexRgbColor) ? "FF0000" : editedUnit.HexRgbColor;

            if (editedUnit.AssignedUsers != null)
            {
                unitToEdit.AssignedUsers.Clear();
                unitToEdit.AssignedUsers = editedUnit.AssignedUsers.Select(x => new ApplicationUserUnitInfo
                {
                    ApplicationUserId = x.Id, UnitInfoId = editedUnit.Id
                }).ToList();
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<string>> NewUnit([FromBody] UnitEditDto newUnit)
        {
            var userId = _userManager.GetUserId(User);

            var newUnitInfo = new UnitInfo
            {
                Id = newUnit.Id, Name = newUnit.Name,
                HexRgbColor = string.IsNullOrWhiteSpace(newUnit.HexRgbColor) ? "FF0000" : newUnit.HexRgbColor
            };
            newUnitInfo.AssignedUsers.Add(new ApplicationUserUnitInfo
            {
                ApplicationUserId = userId, UnitInfo = newUnitInfo
            });
            await _context.UnitInfos.AddAsync(newUnitInfo);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}