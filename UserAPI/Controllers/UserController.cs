using CSharpExamUserAPI.Models.DTO;
using CSharpExamUserAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CSharpExamUserAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUsersRepository _repo;
        public UserController(IUsersRepository repo)
        {
            _repo = repo;
        }

        [HttpGet(template:"get_users")]
        public ActionResult<IEnumerable<UserDto>> GetUsers()
        {
            return _repo.GetUsers().ToList();
        }

        /*
        
        [HttpPost(template:"add_user")]
        public ActionResult<int> AddUser([FromQuery] string email, string password)
        {
            if (_repo.HaveUsers())
            {
                int roleId = _repo.GetRoleId(Models.RoleId.USER);
                if (roleId == -1)
                {
                    return StatusCode(409);
                }
                else
                {
                    var user = new UserDto()
                    {
                        Email = email,
                        PasswordHash = password.GetHashCode(),
                        RoleId = roleId
                    };
                    return _repo.AddUser(user);
                }
            }
            else
            {
                int roleId = _repo.GetRoleId(Models.RoleId.ADMIN);
                if (roleId == -1)
                {
                    return StatusCode(409);
                }
                else
                {
                    var user = new UserDto()
                    {
                        Email = email,
                        PasswordHash = password.GetHashCode(),
                        RoleId = roleId
                    };
                    return _repo.AddUser(user);
                }
            }
        }
        */

    }
}
