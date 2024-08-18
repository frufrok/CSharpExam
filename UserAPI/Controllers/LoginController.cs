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
            return DoIfEmailAndPasswordAreValid(email, password, registration);
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
                return DoIfEmailAndPasswordAreValid(email, password, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private ActionResult DoIfEmailAndPasswordAreValid(string email, string password, Func<string,string,ActionResult> workToDo)
        {
            if (SharedMethods.EmailMatchesPattern(email))
            {
                if (SharedMethods.PasswordMatchesLengthRequirement(password))
                {
                    if (SharedMethods.PasswordMatchesPattern(password))
                    {
                        return workToDo.Invoke(email, password);
                    }
                    else
                    {
                        return StatusCode(400, "Пароль не соответсвует шаблону: он должен содержать хотя бы по одной букве в нижнем и верхнем регистрах и хотя бы одну цифру.");
                    }
                }
                else
                {
                    return StatusCode(400, "Пароль имеет некорректную длинну. Задайте пароль длиной от 8 до 32 символов.");
                }
            }
            else
            {
                return StatusCode(400, "Email не соответствует шаблону.");
            }
        }
    }
}
