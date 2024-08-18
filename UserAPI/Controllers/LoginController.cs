using UserAPI.Models;
using UserAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Authentication;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUsersRepository _userRepository;
        private readonly ITokenSource _tokenSource;
        public LoginController(IUsersRepository userRepository, ITokenSource tokenSource)
        {
            _userRepository = userRepository;
            _tokenSource = tokenSource;
        }

        [AllowAnonymous]
        [HttpPost(template:"Register")]
        public ActionResult Register([FromQuery] string email, string password)
        {
            ActionResult registration(string email, string password)
            {
                if (_userRepository.EmailIsFree(email)) 
                {
                    if (_userRepository.HaveUsers())
                    {
                        var id = _userRepository.AddUser(email, password, RoleId.USER);
                        return Ok(id);
                    }
                    else
                    {
                        var id = _userRepository.AddUser(email, password, RoleId.ADMIN);
                        return Ok(id);
                    }
                }
                else
                {
                    return StatusCode(400, "Пользователь с таким email уже зарегистрирован.");
                }
            }
            return this.DoIfEmailAndPasswordAreValid(email, password, registration);
        }

        [AllowAnonymous]
        [HttpGet(template: "Login")]
        public ActionResult Login([FromQuery] string email, string password)
        {
            try
            {
                ActionResult result(string email, string password)
                {
                    var user = _userRepository.UserCheck(email, password);
                    
                    var token = _tokenSource.CreateToken(user.Email, user.RoleId, user.Guid);

                    return Ok(token);
                }
                return this.DoIfEmailAndPasswordAreValid(email, password, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
