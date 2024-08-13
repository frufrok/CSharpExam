using Microsoft.EntityFrameworkCore;

namespace CSharpExamUserAPI.Models.Context
{
    public class UsersDbContext : DbContext
    {
        private readonly string _connectionString;

        public UsersDbContext() { }

        public UsersDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString).UseLazyLoadingProxies();
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(user =>
            {
                user.ToTable("Users");
                
                user.HasKey(x => x.Id)
                    .HasName("PK_Users");
                user.Property(x => x.Id)
                    .HasColumnName("Id");

                user.HasIndex(x => x.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");
                user.Property(x => x.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(255)
                    .IsRequired(true);

                user.Property(x => x.PasswordHash)
                    .HasColumnName("PasswordHash")
                    .IsRequired(true);

                user.HasOne(user => user.Role)
                    .WithMany(role => role.Users)
                    .HasForeignKey(user => user.RoleId)
                    .HasConstraintName("FK_Users_Roles_RoleId");
                user.Property(user => user.RoleId)
                    .HasColumnName("RoleId")
                    .IsRequired(true);                    
            });

            modelBuilder.Entity<Role>(role =>
            {
                role.ToTable("Roles");

                role.HasKey(x => x.Id)
                    .HasName("PK_Roles");
                role.Property(x => x.Id)
                    .HasColumnName("Id");

                role.HasIndex(x => x.RoleCode)
                    .IsUnique()
                    .HasDatabaseName("IX_Roles_RoleCode");
                role.Property(x => x.RoleCode)
                    .HasColumnName("RoleCode")
                    .IsRequired(true);
                    
            });

        }
    }
}
