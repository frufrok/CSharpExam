using CSharpExamUserAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAPI.Authorization;
using UserAPI.Models;
using UserAPI.Models.DTO;
using UserAPI.Repository;

namespace UserAPITest
{
    public class UserControllerTest
    {
        private UserController _controller;
        private Guid _testGuid = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            var currentUserSourceMock = new Mock<IControllerUserSource>();
            currentUserSourceMock.SetReturnsDefault<UserDto>(new UserDto() { Email = "admin@mail.ru" });

            var userRepoMock = new Mock<IUsersRepository>();
            userRepoMock.Setup(x => x.GetUsers()).Returns(new List<UserDto>() { new() { Email = "admin@mail.ru" }, new() { Email = "user@mail.ru" } });
            userRepoMock.Setup(x => x.GetUserGuid("admin@mail.ru")).Returns(_testGuid);
            userRepoMock.Setup(x => x.GetUserEmail(_testGuid)).Returns("admin@mail.ru");
            userRepoMock.Setup(x => x.EmailIsFree("user@mail.ru")).Returns(true);
            userRepoMock.Setup(x => x.AddUser("user@mail.ru", "Password1", UserAPI.Models.RoleId.USER)).Returns(_testGuid);
            userRepoMock.Setup(x => x.EmailIsFree("user2@mail.ru")).Returns(false);
            userRepoMock.Setup(x => x.DeleteUser("user2@mail.ru")).Returns(_testGuid);

            _controller = new UserController(userRepoMock.Object, currentUserSourceMock.Object);
        }

        [Test]
        public void GetUsersTest()
        {
            var result = _controller.GetUsers();
            Assert.That(result.Value, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Value.Count, Is.EqualTo(2));
                Assert.That(result.Value.ToList()[0].Email, Is.EqualTo("admin@mail.ru"));
                Assert.That(result.Value.ToList()[1].Email, Is.EqualTo("user@mail.ru"));
            });
        }

        [Test]
        public void GetUserGuidTest()
        {
            var result = (ObjectResult)_controller.GetUserGuid("admin@mail.ru");
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(200));
                Assert.That(result.Value, Is.EqualTo(_testGuid));
            });
        }

        [Test]
        public void GetUserEmailTest()
        {
            var result = (ObjectResult)_controller.GetUserEmail(_testGuid);
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(200));
                Assert.That(result.Value, Is.EqualTo("admin@mail.ru"));
            });
        }

        [Test]
        public void AddUserTest()
        {
            var result = (ObjectResult)_controller.AddUser("user@mail.ru", "Password1", 1);
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(200));
                Assert.That(result.Value, Is.EqualTo(_testGuid));
            });
        }

        [Test]
        public void DeleteUser()
        {
            var result = (ObjectResult)_controller.DeleteUser("user2@mail.ru");
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(200));
                Assert.That(result.Value, Is.EqualTo(_testGuid));
            });
        }
    }
}
