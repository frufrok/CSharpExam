using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UserAPI.Controllers;

namespace UserAPITest
{
    public class SharedMethodsTest
    {
        [Test]
        public static void EmailMatchesPatternTest()
        {
            Assert.Multiple(() =>
            {
                Assert.That(SharedMethods.EmailMatchesPattern("user"), Is.False);
                Assert.That(SharedMethods.EmailMatchesPattern("user@mail"), Is.False);
                Assert.That(SharedMethods.EmailMatchesPattern("user@mail.ru"), Is.True);
                Assert.That(SharedMethods.EmailMatchesPattern("@mail.ru"), Is.False);
            });
        }

        [Test]
        public static void PasswordMatchesLengthRequirementTest()
        {
            Assert.Multiple(() =>
            {
                Assert.That(SharedMethods.PasswordMatchesLengthRequirement(string.Empty), Is.False);
                Assert.That(SharedMethods.PasswordMatchesLengthRequirement(new string('_', 8)), Is.True);
                Assert.That(SharedMethods.PasswordMatchesLengthRequirement(new string('_', 32)), Is.True);
                Assert.That(SharedMethods.PasswordMatchesLengthRequirement(new string('_', 33)), Is.False);
            });
        }

        [Test]
        public static void PasswordMatchesPatternTest()
        {
            Assert.Multiple(() =>
            {
                Assert.That(SharedMethods.PasswordMatchesPattern("password"), Is.False);
                Assert.That(SharedMethods.PasswordMatchesPattern("Password"), Is.False);
                Assert.That(SharedMethods.PasswordMatchesPattern("password1"), Is.False);
                Assert.That(SharedMethods.PasswordMatchesPattern("Password1"), Is.True);
            });
        }
    }
}
