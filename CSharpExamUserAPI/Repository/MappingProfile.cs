using AutoMapper;
using CSharpExamUserAPI.Models;
using CSharpExamUserAPI.Models.DTO;

namespace CSharpExamUserAPI.Repository
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>(MemberList.Destination).ReverseMap();
            CreateMap<Role, RoleDto>(MemberList.Destination).ReverseMap();
        }
    }
}
