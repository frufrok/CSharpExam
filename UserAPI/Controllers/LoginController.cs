using CSharpExamUserAPI.Models.DTO;
using CSharpExamUserAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserAPI.Authentication;
using UserAPI.Models.DTO;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserAuthenticationService _authService;
        public LoginController(IConfiguration config, IUserAuthenticationService service)
        {
            _config = config;
            _authService = service;
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login([FromBody] LoginDto login)
        {
            var user = _authService.Authenticate(login);
            return user != null ? Ok(CreateToken(user)) : NotFound("User not found");
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
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
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
