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

namespace MessageAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMessageRepository _messageRepository;
        public MessageController(IConfiguration config, IMessageRepository messageRepository)
        {
            _config = config;
            _messageRepository = messageRepository;
        }

        [HttpPost(template:"SendMessage")]
        [Authorize(Roles = "ADMIN, USER")]
        public ActionResult SendMessage([FromQuery] string emailTo, string text)
        {
            if (SharedMethods.EmailMatchesPattern(emailTo))
            {
                if (text.Length > 0)
                {
                    return Ok(_messageRepository.AddMessage(GetCurrentUser().Email, emailTo, text));
                }
                else
                {
                    return StatusCode(400, "Текст сообщения не может быть пустым.");
                }
            }
            else
            {
                return StatusCode(400, "Email не соответсвует шаблону.");
            }
        }

        [HttpGet(template:"GetMessages")]
        [Authorize(Roles = "ADMIN, USER")]
        public ActionResult GetMessages()
        {
            return Ok(_messageRepository.GetMessages(GetCurrentUser().Email));
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
