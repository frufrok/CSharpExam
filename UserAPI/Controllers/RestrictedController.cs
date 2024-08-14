using CSharpExamUserAPI.Models;
using CSharpExamUserAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestrictedController : ControllerBase
    {
        [HttpGet(template:"get_id_from_token")]
        [Authorize(Roles = "0,1")]
        public ActionResult GetIdFromToken()
        {
            var user = GetCurrentUser();
            return Ok(user.Email);
        }

        private UserDto? GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return new UserDto()
                {
                    Email = userClaims
                        .FirstOrDefault(
                            x => x.Type == ClaimTypes.NameIdentifier)?
                            .Value,
                    RoleId = int.Parse(
                        userClaims.FirstOrDefault(
                            x => x.Type == ClaimTypes.Role)?
                        .Value)
                };
            }
            else return null;
        }
    }
}
