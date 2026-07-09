using Authentication.RefreshToken.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.RefreshToken.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(user => user.UserName).IsUnique();
            entity.Property(user => user.UserName).IsRequired().HasMaxLength(100);
            entity.Property(user => user.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.Property(session => session.RefreshTokenHash).IsRequired();
            entity.HasIndex(session => session.RefreshTokenHash).IsUnique();
            entity.HasOne(session => session.User)
                .WithMany(user => user.Sessions)
                .HasForeignKey(session => session.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
