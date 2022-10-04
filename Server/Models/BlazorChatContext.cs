using BlazorChat.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorChat.Server.Models
{
    public partial class BlazorChatContext : DbContext
    {
        public BlazorChatContext()
        {
        }

        public BlazorChatContext(DbContextOptions<BlazorChatContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Log> Logs { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Name=BlazorChat");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.LogId).HasColumnName("log_id");

                entity.Property(e => e.CreatedDate).HasColumnName("created_date");

                entity.Property(e => e.EventName).HasColumnName("event_name");

                entity.Property(e => e.ExceptionMessage).HasColumnName("exception_message");

                entity.Property(e => e.LogLevel).HasColumnName("log_level");

                entity.Property(e => e.Source).HasColumnName("source");

                entity.Property(e => e.StackTrace).HasColumnName("stack_trace");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.AboutMe).HasColumnName("about_me");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("DATE")
                    .HasColumnName("created_date");

                entity.Property(e => e.DarkTheme).HasColumnName("dark_theme");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("DATETIME")
                    .HasColumnName("date_of_birth");

                entity.Property(e => e.EmailAddress).HasColumnName("email_address");

                entity.Property(e => e.FirstName).HasColumnName("first_name");

                entity.Property(e => e.LastName).HasColumnName("last_name");

                entity.Property(e => e.Notifications).HasColumnName("notifications");

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.ProfilePicDataUrl).HasColumnName("profile_pic_data_url");

                entity.Property(e => e.ProfilePictureUrl).HasColumnName("profile_picture_url");

                entity.Property(e => e.Source).HasColumnName("source");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
