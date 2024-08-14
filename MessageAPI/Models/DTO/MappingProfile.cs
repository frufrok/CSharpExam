using AutoMapper;
using MessageAPI.Models;
using MessageAPI.Models.DTO;

namespace MessageAPI.Models.DTO
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Message, MessageDto>(MemberList.Destination).ReverseMap();
        }
    }
}
