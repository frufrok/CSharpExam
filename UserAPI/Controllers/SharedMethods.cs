using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace UserAPI.Controllers
{
    public static class SharedMethods
    {
        public static bool EmailMatchesPattern(string email)
        {
            var regex = new Regex("^\\S+@\\S+\\.\\S+$");
            return regex.Matches(email).Count() == 1;
        }

        public static bool PasswordMatchesLengthRequirement(string password)
        {
            return password.Length >= 8 && password.Length <= 32;
        }

        public static bool PasswordMatchesPattern(string password)
        {
            var regex = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$");
            return regex.Matches(password).Count() > 0;
        }
    }
}
