using CSharpExamUserAPI.Models;
using CSharpExamUserAPI.Models.DTO;

namespace CSharpExamUserAPI.Repository
{
    public interface IUsersRepository
    {
        public Guid AddUser(string email, string password, RoleId roleId);
        public IEnumerable<UserDto> GetUsers();
        public bool HaveUsers();
        public UserDto UserCheck(string email, string password);
    }
}
