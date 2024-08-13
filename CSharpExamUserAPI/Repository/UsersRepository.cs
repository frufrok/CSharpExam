using AutoMapper;
using CSharpExamUserAPI.Models;
using CSharpExamUserAPI.Models.DTO;
using CSharpExamUserAPI.Models.Context;

namespace CSharpExamUserAPI.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IMapper _mapper;
        private readonly UsersDbContext _context;

        public UsersRepository(IMapper mapper, UsersDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public int AddRole(RoleDto role)
        {
            if (_context.Roles.Any(x => x.RoleCode == role.RoleCode))
            {
                return -1;
            }
            else
            {
                var result = _mapper.Map<Role>(role);
                _context.Roles.Add(result);
                _context.SaveChanges();
                return result.Id ?? -1;
            }
        }

        public int AddUser(UserDto user)
        {
            if (_context.Users.Any(x => x.Email.ToLower().Equals(user.Email.ToLower())))
            {
                return -1;
            }
            else
            {
                var result = _mapper.Map<User>(user);
                _context.Users.Add(result);
                _context.SaveChanges();
                return result.Id ?? -1;
            }
        }

        public int GetRoleId(RoleCodes roleCode)
        {
            var result = _context.Roles.FirstOrDefault(x => x.RoleCode == roleCode);
            return result == null ? -1 : result.Id ?? -1;
        }

        public IEnumerable<RoleDto> GetRoles()
        {
            return _context.Roles.Select(x => _mapper.Map<RoleDto>(x)).ToList();
        }

        public IEnumerable<UserDto> GetUsers()
        {
            return _context.Users.Select(x => _mapper.Map<UserDto>(x)).ToList();
        }

        public bool HaveUsers()
        {
            return _context.Users.Any();
        }
    }
}
