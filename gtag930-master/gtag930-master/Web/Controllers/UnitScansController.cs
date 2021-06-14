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
    public class UnitScansController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UnitScansController(ApplicationDbContext context, IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<List<UnitScanDto>> Index([FromQuery] bool latestTagData, [FromQuery] List<string> unitIds,
            [FromQuery] UnitScanStateDto? state = null, [FromQuery] DateTimeOffset? from = null,
            [FromQuery] DateTimeOffset? to = null, [FromQuery] List<string> tagIds = null)
        {
            var userId = _userManager.GetUserId(User);

            if (_context.UnitInfos.Any(unitInfo =>
                unitIds.Contains(unitInfo.Id) &&
                !unitInfo.AssignedUsers.Select(u => u.ApplicationUserId).Contains(userId)))
                throw new Exception("cannot request units which the current user is not assigned to");

            IEnumerable<UnitScan> unitScans = _context.UnitScans.Where(x => unitIds.Contains(x.UnitInfo.Id));

            if (state != null)
            {
                var unitScanStateFilter = TranslateState((UnitScanStateDto) state);
                unitScans = unitScans.Where(x => x.State == unitScanStateFilter);
            }

            if (from != null) unitScans = unitScans.Where(x => x.CreatedOn >= from);

            if (to != null) unitScans = unitScans.Where(x => x.CreatedOn <= to);

            if (tagIds != null && tagIds.Count > 0)
                unitScans = unitScans.ToList().Where(x => x.TagScans.Any(tag => tagIds.Contains(tag.TagId)));

            if (latestTagData)
                unitScans = unitScans.GroupBy(x => x.UnitInfoId,
                    (unitInfoId, g) => g.OrderByDescending(scan => scan.CreatedOn).FirstOrDefault()).ToList();

            // unfortunatly, we have to use "asEnumerable" as this kind of query does not work in .net 3.1
            // https://stackoverflow.com/questions/59346353/problem-with-ef-orderby-after-migration-to-net-core-3-1
            return _mapper.Map<List<UnitScanDto>>(unitScans.OrderByDescending(unitScan => unitScan.CreatedOn).ToList());
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<List<string>>> NewUnitScan([FromBody] NewUnitScanDto newUnitScanDto)
        {
            var userId = _userManager.GetUserId(User);

            var unitInfo = _context.UnitInfos.SingleOrDefault(x => x.Id == newUnitScanDto.UnitId) ??
                           _context.UnitInfos.Single(x => x.Id == UnitInfo.DEFAULT_ID);

            var unitScan = new UnitScan
            {
                UnitInfo = unitInfo,
                Description = newUnitScanDto.Description,
                ImageJpeg =
                    string.IsNullOrEmpty(newUnitScanDto.ImageJpegBase64)
                        ? null
                        : Convert.FromBase64String(newUnitScanDto.ImageJpegBase64),
                TagScans = newUnitScanDto.TagIds
                    .Select(tagId => new TagScan {TagId = tagId}).ToList(),
                GpsLatitude = newUnitScanDto.GpsLatitude,
                GpsLongitude = newUnitScanDto.GpsLongitude,
                OwnerId = userId,
                State = TranslateState(newUnitScanDto.State)
            };
            await _context.UnitScans.AddAsync(unitScan);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private UnitScanState TranslateState(UnitScanStateDto dtoState)
        {
            switch (dtoState)
            {
                case UnitScanStateDto.None:
                    return UnitScanState.None;
                case UnitScanStateDto.InProgress:
                    return UnitScanState.InProgress;
                case UnitScanStateDto.Packing:
                    return UnitScanState.Packing;
                case UnitScanStateDto.Shipping:
                    return UnitScanState.Shipping;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dtoState), dtoState, null);
            }
        }
    }
}