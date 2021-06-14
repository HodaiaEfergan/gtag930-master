#region

using System;
using System.Linq;
using AutoMapper;
using Web.DTO;
using Web.Models;

#endregion

namespace Web
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dto => dto.Roles, opt => opt.MapFrom(x => x.UserRoles.Select(y => y.Role).ToList()))
                .ForMember(dto => dto.IsLocked,
                    opt => opt.MapFrom(user => user.LockoutEnabled && user.LockoutEnd > DateTimeOffset.Now));

            CreateMap<ApplicationRole, RoleDto>();

            CreateMap<UnitInfo, UnitInfoDto>().ForMember(dto => dto.AssignedUsers,
                opt => opt.MapFrom(x => x.AssignedUsers.Select(y => y.ApplicationUser).ToList()));

            CreateMap<UnitScan, UnitScanDto>().ForMember(dto => dto.ImageJpegBase64,
                opt => opt.MapFrom(x => Convert.ToBase64String(x.ImageJpeg)));

            CreateMap<TagScan, TagScanDto>();
        }
    }
}