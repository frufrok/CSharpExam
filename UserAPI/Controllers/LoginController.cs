using CSharpExamUserAPI.Models;
using CSharpExamUserAPI.Models.DTO;
using CSharpExamUserAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserAPI.Models.DTO;

namespace UserAPI.Controllers
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
        [HttpPost]
        public ActionResult Login([FromBody] LoginDto userLogin)
        {
            try
            {
                var user = _userRepository.UserCheck(userLogin.Email, userLogin.Password);

                var token = CreateToken(user);

                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("AddAdmin")]
        public ActionResult AddAdmin([FromBody] LoginDto userLogin)
        {
            try
            {
                _userRepository.AddUser(userLogin.Email, userLogin.Password, RoleId.ADMIN);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return Ok();
        }

        [HttpPost]
        [Route("AddUser")]
        [Authorize(Roles = "ADMIN")]
        public ActionResult AddUser([FromBody] LoginDto userLogin)
        {
            try
            {
                _userRepository.AddUser(userLogin.Email, userLogin.Password, RoleId.USER);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return Ok();
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
            //var key = new SymmetricSecurityKey(
            //    Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
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
    }
}
