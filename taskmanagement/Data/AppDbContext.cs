using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using taskmanagement.Models.Entities;
using taskmanagement.Models.Enums;

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

        public DbSet<Mentor> Mentor { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<User>()
                    .HasIndex(u => u.Username)
                    .IsUnique();

                modelBuilder.Entity<User>()
                    .HasIndex(u => u.Email)
                    .IsUnique();

                modelBuilder.Entity<Mentor>()
                    .HasIndex(m => m.Email)
                    .IsUnique();
                
                
                // modelBuilder.Entity<Mentor>()
                //     .Property(m => m.Status)
                //     .HasConversion<string>()
                //     .HasDefaultValue(MentorStatus.Active);


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