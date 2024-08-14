using CSharpExamUserAPI.Models.DTO;
using UserAPI.Models.DTO;

namespace UserAPI.Authentication
{
    public class UserAuthenticationServiceMock : IUserAuthenticationService
    {
        public UserDto Authenticate(LoginDto login)
        {
            if (login.Email == "admin" && login.Password == "password")
            {
                return new UserDto()
                {
                    Id = 1,
                    Email = login.Email,
                    PasswordHash = login.Password.GetHashCode(),
                    RoleId = 0
                };
            }

            if (login.Email == "user" && login.Password == "password")
            {
                return new UserDto()
                {
                    Id = 2,
                    Email = login.Email,
                    PasswordHash = login.Password.GetHashCode(),
                    RoleId = 1
                };
            }

            return null;
        }
    }
}
