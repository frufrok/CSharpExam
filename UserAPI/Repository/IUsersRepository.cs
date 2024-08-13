using CSharpExamUserAPI.Models;
using CSharpExamUserAPI.Models.DTO;

namespace CSharpExamUserAPI.Repository
{
    public interface IUsersRepository
    {
        public int AddUser(UserDto user);
        public IEnumerable<UserDto> GetUsers();
        public int AddRole(RoleDto role);
        public IEnumerable<RoleDto> GetRoles();
        public int GetRoleId(RoleCodes roleCode);
        public bool HaveUsers();
    }
}
