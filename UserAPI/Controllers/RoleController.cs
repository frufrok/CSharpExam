using CSharpExamUserAPI.Models;
using CSharpExamUserAPI.Models.DTO;
using CSharpExamUserAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CSharpExamUserAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IUsersRepository _repo;
        public RoleController(IUsersRepository repo)
        {
            _repo = repo;
        }

        [HttpGet(template: "get_roles")]
        public ActionResult<IEnumerable<RoleDto>> GetRoles()
        {
            return _repo.GetRoles().ToList();
        }

        [HttpPost(template: "add_role")]
        public ActionResult<int> AddRole([FromQuery] int roleCode)
        {
            if (Enum.IsDefined(typeof(RoleCodes), roleCode))
            {
                RoleCodes code = (RoleCodes)roleCode;
                return _repo.AddRole(new RoleDto() { RoleCode = code });
            }
            else
            {
                return StatusCode(406);
            }
        }
    }
}
