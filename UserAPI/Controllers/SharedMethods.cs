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

        public static ActionResult DoIfEmailAndPasswordAreValid(this ControllerBase controller, string email, string password, Func<string, string, ActionResult> workToDo)
        {
            if (SharedMethods.EmailMatchesPattern(email))
            {
                if (SharedMethods.PasswordMatchesLengthRequirement(password))
                {
                    if (SharedMethods.PasswordMatchesPattern(password))
                    {
                        return workToDo.Invoke(email, password);
                    }
                    else
                    {
                        return controller.StatusCode(400, "Пароль не соответсвует шаблону: он должен содержать хотя бы по одной букве в нижнем и верхнем регистрах и хотя бы одну цифру.");
                    }
                }
                else
                {
                    return controller.StatusCode(400, "Пароль имеет некорректную длинну. Задайте пароль длиной от 8 до 32 символов.");
                }
            }
            else
            {
                return controller.StatusCode(400, "Email не соответствует шаблону.");
            }
        }
    }
}
