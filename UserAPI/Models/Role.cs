namespace UserAPI.Models
{
    public class Role
    {
        public RoleId RoleId { get; set; }
        public string Name { get; set; }
        public virtual List<User> Users { get; set; } = [];
    }
    public enum RoleId
    {
        ADMIN = 0,
        USER = 1
    }
}
