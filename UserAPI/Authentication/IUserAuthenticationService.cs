using CSharpExamUserAPI.Models.DTO;
using UserAPI.Models.DTO;

namespace UserAPI.Authentication
{
    public interface IUserAuthenticationService
    {
        UserDto Authenticate(LoginDto login);
    }
}
