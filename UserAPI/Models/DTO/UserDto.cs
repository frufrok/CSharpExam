namespace CSharpExamUserAPI.Models.DTO
{
    public class UserDto
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public int? PasswordHash { get; set; }
        public int? RoleId { get; set; }
    }
}
