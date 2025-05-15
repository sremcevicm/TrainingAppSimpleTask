using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TrainingApp.Server.Data.Models;

namespace TrainingApp.Server.Data.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<TrainingSession> TrainingSessions { get; set; }
        public DbSet<UserTraining> UserTrainings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: Configure relationships and constraints explicitly if needed

            modelBuilder.Entity<UserTraining>()
                .HasOne(ut => ut.User)
                .WithMany()
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserTraining>()
                .HasOne(ut => ut.TrainingSession)
                .WithMany(ts => ts.UserTrainings)
                .HasForeignKey(ut => ut.TrainingSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrainingSession>()
                .HasOne(ts => ts.Trainer)
                .WithMany(t => t.TrainingSessions)
                .HasForeignKey(ts => ts.TrainerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
