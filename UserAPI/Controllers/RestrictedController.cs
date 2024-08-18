using UserAPI.Models;
using UserAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserAPI.Authorization;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestrictedController : ControllerBase
    {
        private readonly IControllerUserSource _userSource;

        public RestrictedController(IControllerUserSource userSource)
        {
            _userSource = userSource;
        }

        [HttpGet(template:"GetIdFromToken")]
        [Authorize(Roles = "ADMIN,USER")]
        public ActionResult GetIdFromToken()
        {
            var user = _userSource.GetUser(this);
            if (user != null)
            {
                return Ok(user.Guid);
            }
            else return StatusCode(400, "Для получения информации необходима аутентификация.");
        }
    }
}
