namespace CSharpExamUserAPI.Models.DTO
{
    public class UserDto
    {
        public Guid Guid { get; set; }
        public string Email { get; set; }
        public RoleId RoleId { get; set; }
    }
}
