using UserAPI.Models;
using UserAPI.Models.DTO;
using UserAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserAPI.Controllers;

namespace CSharpExamUserAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUsersRepository _userRepository;
        public UserController(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpGet(template:"GetUsers")]
        public ActionResult<IEnumerable<UserDto>> GetUsers()
        {
            return _userRepository.GetUsers().ToList();
        }

        [AllowAnonymous]
        [HttpGet(template:"GetUserGuid")]
        public ActionResult GetUserGuid([FromQuery] string email)
        {
            if (SharedMethods.EmailMatchesPattern(email))
            {
                try
                {
                    return Ok(_userRepository.GetUserGuid(email));
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            else
            {
                return StatusCode(400, "Email не соответствует шаблону.");
            }
        }

        [AllowAnonymous]
        [HttpGet(template: "GetUserEmail")]
        public ActionResult GetUserEmail([FromQuery] Guid guid)
        {
            try
            {
                return Ok(_userRepository.GetUserEmail(guid));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost(template:"AddUser")]
        [Authorize(Roles = "ADMIN")]
        public ActionResult AddUser([FromQuery] string email, string password, int roleId)
        {
            try
            {
                ActionResult result(string email, string password)
                {
                    if (Enum.IsDefined(typeof(RoleId), roleId))
                    {
                        var id = _userRepository.AddUser(email, password, RoleId.ADMIN);
                        return Ok(id);
                    }
                    else
                    {
                        return StatusCode(400, "Задан несуществующий ID роли.");
                    }

                }
                return this.DoIfEmailAndPasswordAreValid(email, password, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete(template:"DeleteUser")]
        [Authorize(Roles = "ADMIN")]
        public ActionResult DeleteUser([FromQuery] string email)
        {
            if (SharedMethods.EmailMatchesPattern(email))
            {
                var self = GetCurrentUser();
                if (self != null)
                {
                    if (!email.ToLower().Equals(self.Email.ToLower()))
                    {
                        try
                        {
                            if (_userRepository.EmailIsFree(email))
                            {
                                return StatusCode(400, "Такого пользователя нет в базе данных.");
                            }
                            else return Ok(_userRepository.DeleteUser(email));
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, ex);
                        }
                    }
                    else
                    {
                        return StatusCode(400, "Нельзя удалять собственный email.");
                    }
                }
                else return StatusCode(400, "Не удалось считать данные пользователя из предоставленного токена.");
            }
            else
            {
                return StatusCode(400, "Email не соответствует шаблону.");
            }
        }

        private UserDto? GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return new UserDto()
                {
                    Email = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                    RoleId = (RoleId)Enum.Parse(typeof(RoleId),
                        userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value),
                    Guid = Guid.Parse(userClaims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value)
                };
            }
            else return null;
        }
    }
}
