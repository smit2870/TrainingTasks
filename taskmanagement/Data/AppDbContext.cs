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

        public DbSet<LearningTask> LearningTask { get; set; }

        public DbSet<TaskAssignment> TaskAssignment { get; set; }

        public DbSet<Submission> Submission { get; set; }

        public DbSet<Review> Review { get; set; }
        
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
                
                
                modelBuilder.Entity<TaskAssignment>()
                    .HasOne(t => t.Trainee)
                    .WithMany()
                    .HasForeignKey(t => t.TraineeId)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<TaskAssignment>()
                    .HasOne(t => t.Mentor)
                    .WithMany()
                    .HasForeignKey(t => t.MentorId)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<TaskAssignment>()
                    .HasOne(t => t.LearningTask)
                    .WithMany()
                    .HasForeignKey(t => t.LearningTaskId)
                    .OnDelete(DeleteBehavior.Restrict);

                
                modelBuilder.Entity<Submission>().HasKey(s => s.Id);

                modelBuilder.Entity<Submission>()
                    .HasOne(s => s.TaskAssignment)
                    .WithMany()
                    .HasForeignKey(s => s.TaskAssignmentId);

                modelBuilder.Entity<Review>()
                    .HasOne(r => r.Submission)
                    .WithMany()
                    .HasForeignKey(r => r.SubmissionId)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<Review>()
                    .HasOne(r => r.Mentor)
                    .WithMany()
                    .HasForeignKey(r => r.MentorId)
                    .OnDelete(DeleteBehavior.Restrict);

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