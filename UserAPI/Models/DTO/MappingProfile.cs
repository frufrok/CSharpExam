using AutoMapper;
using UserAPI.Models;
using UserAPI.Models.DTO;

namespace UserAPI.Models.DTO
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, User>()
                .ForMember(user => user.Id, opts => opts.Ignore())
                .ForMember(user => user.Guid, opts => opts.MapFrom(dto => dto.Guid))
                .ForMember(user => user.Email, opts => opts.MapFrom(dto => dto.Email))
                .ForMember(user => user.Password, opts => opts.Ignore())
                .ForMember(user => user.Salt, opts => opts.Ignore())
                .ForMember(user => user.RoleId, opts => opts.MapFrom(dto => dto.RoleId))
                .ReverseMap();
        }
    }
}
