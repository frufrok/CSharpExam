using UserAPI.Models;
using UserAPI.Models.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using UserAPI.Authentication;
using static UserAPI.Controllers.LoginController;

namespace UserAPI.rsa
{
    public class RSATokenSource : ITokenSource
    {
        private readonly IConfiguration _config;

        public RSATokenSource(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(string email, RoleId role, Guid guid)
        {
            var key = new RsaSecurityKey(GetPrivateKey());
            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.RsaSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, email),
                new Claim(ClaimTypes.Role, role.ToString()),
                new Claim(ClaimTypes.SerialNumber, guid.ToString())
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static RSA GetPrivateKey()
        {
            var f = System.IO.File.ReadAllText("rsa/private_key.pem");
            var rsa = RSA.Create();
            rsa.ImportFromPem(f);
            return rsa;
        }

        public static RSA GetPublicKey()
        {
            var f = File.ReadAllText("rsa/public_key.pem");
            var rsa = RSA.Create();
            rsa.ImportFromPem(f);
            return rsa;
        }
    }
}
