using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using taskmanagement.Models;

namespace taskmanagement.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            
        }
        public DbSet<Trainee> Trainees { get; set; }

        public DbSet<User> Users { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<User>()
                    .HasIndex(u => u.Username)
                    .IsUnique();

                modelBuilder.Entity<User>()
                    .HasIndex(u => u.Email)
                    .IsUnique();

                // Seed Admin user
                modelBuilder.Entity<User>().HasData(new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = UserRole.Admin,
                    CreatedDate = DateTime.UtcNow
                });
        }
    }
}