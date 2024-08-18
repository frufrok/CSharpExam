using Microsoft.AspNetCore.Mvc;
using UserAPI.Models.DTO;

namespace UserAPI.Authorization
{
    public interface IControllerUserSource
    {
        public UserDto? GetUser(ControllerBase controller);
    }
}
