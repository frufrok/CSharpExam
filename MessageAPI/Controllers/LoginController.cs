using MessageAPI.Models;
using MessageAPI.Models.DTO;
using MessageAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using MessageAPI.Models.DTO;

namespace MessageAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUsersRepository _userRepository;
        public LoginController(IConfiguration config, IUsersRepository userRepository)
        {
            _config = config;
            _userRepository = userRepository;
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
        [HttpPost(template: "Login")]
        public ActionResult Login([FromQuery] string email, string password)
        {
            try
            {
                ActionResult result(string email, string password)
                {
                    var user = _userRepository.UserCheck(email, password);

                    var token = CreateToken(user);

                    return Ok(token);
                }
                return DoIfEmailAndPasswordAreValid(email, password, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public static class RSATools
        {
            public static RSA GetPrivateKey()
            {
                var f = System.IO.File.ReadAllText("rsa/private_key.pem");
                var rsa = RSA.Create();
                rsa.ImportFromPem(f);
                return rsa;
            }
        }
  
        private string CreateToken(UserDto user)
        {
            var key = new RsaSecurityKey(RSATools.GetPrivateKey());
            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.RsaSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()),
                new Claim(ClaimTypes.SerialNumber, user.Guid.ToString())
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
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
