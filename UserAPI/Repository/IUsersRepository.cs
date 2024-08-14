using CSharpExamUserAPI.Models;
using CSharpExamUserAPI.Models.DTO;

namespace CSharpExamUserAPI.Repository
{
    public interface IUsersRepository
    {
        public bool HaveUsers();
        public IEnumerable<UserDto> GetUsers();
        public bool EmailIsFree(string email);
        public Guid AddUser(string email, string password, RoleId roleId);
        public UserDto UserCheck(string email, string password);
        public Guid DeleteUser(string email);
        public Guid GetUserGuid(string email);
        public string GetUserEmail(Guid guid);
    }
}
