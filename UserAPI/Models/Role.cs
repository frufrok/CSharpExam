namespace CSharpExamUserAPI.Models
{
    public class Role
    {
        public int? Id { get; set; }
        public RoleCodes? RoleCode { get; set; }
        public virtual List<User>? Users { get; set; } = [];
    }
    public enum RoleCodes
    {
        ADMIN = 0,
        USER = 1
    }
}
