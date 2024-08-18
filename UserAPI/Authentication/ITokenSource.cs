using UserAPI.Models;
using UserAPI.Models.DTO;

namespace UserAPI.Authentication
{
    public interface ITokenSource
    {
        public string CreateToken(string email, RoleId role, Guid guid);
    }
}
