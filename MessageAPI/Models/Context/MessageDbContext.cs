using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace MessageAPI.Models.Context
{
    public class MessageDbContext : DbContext
    {
        private readonly string _connectionString;

        public MessageDbContext() { }

        public MessageDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }

        public virtual DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>(msg =>
            {
                msg.ToTable("Messages");
                
                msg.HasKey(x => x.Id)
                    .HasName("PK_Messages");
                msg.Property(x => x.Id)
                    .HasColumnName("Id");

                msg.HasIndex(x => x.Guid)
                    .IsUnique()
                    .HasDatabaseName("IX_Messages_Guid");
                msg.Property(x => x.Guid)
                    .HasColumnName("Guid")
                    .IsRequired();

                msg.HasIndex(x => x.UserFromGuid)
                    .HasDatabaseName("IX_Message_UserFromGuid");
                msg.Property(x => x.UserFromGuid)
                    .HasColumnName("UserFromGuid")
                    .IsRequired(true);

                msg.HasIndex(x => x.UserToGuid)
                    .HasDatabaseName("IX_Message_UserToGuid");
                msg.Property(x => x.UserToGuid)
                    .HasColumnName("UserToGuid")
                    .IsRequired(true);

                msg.Property(x => x.Text)
                    .HasMaxLength(1023)
                    .HasColumnName("Text")
                    .IsRequired(true);

                msg.Property(x => x.DateTime)
                    .HasColumnName("DateTime")
                    .IsRequired(true);

                msg.HasIndex(x => x.IsReaded)
                    .HasDatabaseName("IX_Message_IsReaded");
                msg.Property(x => x.IsReaded)
                    .HasColumnName("IsReaded")
                    .IsRequired(true);

            });
        }
    }
}
