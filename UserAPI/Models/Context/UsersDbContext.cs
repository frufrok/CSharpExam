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

                user.HasIndex(x => x.Guid)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Guid");
                user.Property(x => x.Guid)
                    .HasColumnName("Guid")
                    .IsRequired();

                user.HasIndex(x => x.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");
                user.Property(x => x.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(255)
                    .IsRequired(true);

                user.Property(x => x.Password)
                    .HasColumnName("Password")
                    .IsRequired(true);

                user.Property(x => x.Salt)
                    .HasColumnName("Salt")
                    .IsRequired(true);

                user.Property(x => x.RoleId).HasConversion<int>();
                /*
                user.HasOne(user => user.Role)
                    .WithMany(role => role.Users)
                    .HasForeignKey(user => user.RoleId)
                    .HasConstraintName("FK_Users_Roles_RoleId");
                user.Property(user => user.RoleId)
                    .HasColumnName("RoleId")
                    .IsRequired(true);   
                */
            });

            modelBuilder
                .Entity<Role>()
                .Property(e => e.RoleId)
                .HasConversion<int>();

            modelBuilder
                .Entity<Role>()
                .HasData(
                Enum.GetValues(typeof(RoleId))
                .Cast<RoleId>()
                .Select(x => new Role()
                {
                    RoleId = x,
                    Name = x.ToString()
                }));

            /*
            modelBuilder.Entity<Role>(role =>
            {
                role.ToTable("Roles");

                role.HasKey(x => x.Id)
                    .HasName("PK_Roles");
                role.Property(x => x.Id)
                    .HasColumnName("Id");

                role.HasIndex(x => x.RoleId)
                    .IsUnique()
                    .HasDatabaseName("IX_Roles_RoleCode");
                role.Property(x => x.RoleId)
                    .HasColumnName("RoleCode")
                    .IsRequired(true);
                    
            });
            */
        }
    }
}
