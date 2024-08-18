using Microsoft.AspNetCore.Mvc;
using Moq;
using UserAPI.Authentication;
using UserAPI.Controllers;
using UserAPI.Models;
using UserAPI.Models.DTO;
using UserAPI.Repository;

namespace UserAPITest
{
    [TestFixture]
    public class LoginControllerTest
    {
        private Guid _testGuid = Guid.NewGuid();
        private ITokenSource _tokenSrsMock;
        private IUsersRepository _repoMock;
        private LoginController _controller;

        [SetUp]
        public void Setup()
        {
            var tokenMock = new Mock<ITokenSource>();
            tokenMock.Setup(x => x.CreateToken("user@mail.ru", RoleId.USER, _testGuid)).Returns(_testGuid.ToString());
            _tokenSrsMock = tokenMock.Object;

            var repoMock = new Mock<IUsersRepository>();
            repoMock.Setup(x => x.EmailIsFree("user@mail.ru")).Returns(true);
            repoMock.Setup(x => x.HaveUsers()).Returns(true);
            repoMock.Setup(x => x.AddUser("user@mail.ru", "Password1", RoleId.USER)).Returns(Guid.NewGuid());
            repoMock.Setup(x => x.UserCheck("user@mail.ru", "Password1")).Returns(new UserDto() { Email = "user@mail.ru", Guid = _testGuid, RoleId = RoleId.USER });
            _repoMock = repoMock.Object;

            _controller = new(_repoMock, _tokenSrsMock);
        }

        [Test]
        public void InvalidEmailTest()
        {
            var result = (ObjectResult)_controller.Register("user", String.Empty);
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(400));
                Assert.That(result.Value, Is.EqualTo("Email не соответствует шаблону."));
            });
        }

        [Test]
        public void ShortPasswordTest()
        {
            var result = (ObjectResult)_controller.Register("user@mail.ru", String.Empty);
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(400));
                Assert.That(result.Value, Is.EqualTo("Пароль имеет некорректную длинну. Задайте пароль длиной от 8 до 32 символов."));
            });
        }

        [Test]
        public void LongPasswordTest()
        {
            var result = (ObjectResult)_controller.Register("user@mail.ru", new string('_',33));
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(400));
                Assert.That(result.Value, Is.EqualTo("Пароль имеет некорректную длинну. Задайте пароль длиной от 8 до 32 символов."));
            });
        }

        [Test]
        public void InvalidPasswordTest()
        {
            var result = (ObjectResult)_controller.Register("user@mail.ru", "password");
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(400));
                Assert.That(result.Value, Is.EqualTo("Пароль не соответсвует шаблону: он должен содержать хотя бы по одной букве в нижнем и верхнем регистрах и хотя бы одну цифру."));
            });
        }

        [Test]
        public void AdminRegisterTest()
        {
            var mock = new Mock<IUsersRepository>();
            mock.Setup(x => x.EmailIsFree("admin@mail.ru")).Returns(true);
            mock.Setup(x => x.HaveUsers()).Returns(false);
            mock.Setup(x => x.AddUser("admin@mail.ru", "Password1", RoleId.ADMIN)).Returns(Guid.NewGuid());
            var repo = mock.Object;
            var controller = new LoginController(repo, _tokenSrsMock);
            var result = (ObjectResult)controller.Register("admin@mail.ru", "Password1");
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(200));
                Assert.That(result.Value, Is.TypeOf(typeof(Guid)));
            });
        }

        [Test]
        public void UserRegisterTest()
        {
            var result = (ObjectResult)_controller.Register("user@mail.ru", "Password1");
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(200));
                Assert.That(result.Value, Is.TypeOf(typeof(Guid)));
            });
        }

        [Test]
        public void LoginTest()
        {
            var result = (ObjectResult)_controller.Login("user@mail.ru", "Password1");
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(200));
                Assert.That(result.Value, Is.EqualTo(_testGuid.ToString()));
            });
        }
    }
}