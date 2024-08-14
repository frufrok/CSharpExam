using AutoMapper;
using CSharpExamUserAPI.Models;
using CSharpExamUserAPI.Models.DTO;
using CSharpExamUserAPI.Models.Context;
using System.Text;
using System.Security.Cryptography;

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

        public Guid AddUser(string email, string password, RoleId roleId)
        {
            var user = new User()
            {
                Guid = Guid.NewGuid(),
                Email = email,
                RoleId = roleId,
                Salt = new byte[16]
            };

            new Random().NextBytes(user.Salt);

            var data = Encoding.ASCII.GetBytes(password).Concat(user.Salt).ToArray();

            SHA512 shaM = new SHA512Managed();

            user.Password = shaM.ComputeHash(data);

            _context.Users.Add(user);

            _context.SaveChanges();

            return user.Guid;
        }

        public bool EmailIsFree(string email)
        {
            return _context.Users.FirstOrDefault(x => x.Email.ToLower().Equals(email.ToLower())) == null;
        }

        public IEnumerable<UserDto> GetUsers()
        {
            return _context.Users.Select(x => _mapper.Map<UserDto>(x)).ToList();
        }

        public bool HaveUsers()
        {
            return _context.Users.Any();
        }

        public Guid DeleteUser(string email)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email.ToLower().Equals(email.ToLower()));
            if (user != null)
            {
                Guid result = new Guid(user.Guid.ToByteArray());
                _context.Users.Remove(user);
                _context.SaveChanges();
                return result;
            }
            else throw new ArgumentException("В списке пользователей нет пользователя с указанным email.");
        }

        public UserDto UserCheck(string Email, string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == Email);

            if (user == null) throw new Exception("User not found");

            var data = Encoding.ASCII.GetBytes(password).Concat(user.Salt).ToArray();
            var shaM = new SHA512Managed();
            var bspassword = shaM.ComputeHash(data);

            if (user.Password.SequenceEqual(bspassword))
            {
                return _mapper.Map<UserDto>(user);
            }
            else throw new Exception("Wrong password");
        }

        public Guid GetUserGuid(string email)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email.ToLower().Equals(email.ToLower()));
            if (user != null)
            {
                return user.Guid;
            }
            else throw new Exception("Пользователя с указанных Email не существует");
        }

        public string GetUserEmail(Guid guid)
        {
            var user = _context.Users.FirstOrDefault(x => x.Guid.Equals(guid));
            if (user != null)
            {
                return user.Email;
            }
            else throw new Exception("Пользователя с указанным GUID не существует.");
        }
    }
}
