using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkoutTrackingApp.Models;

namespace WorkoutTrackingApp.Data
{
    // Here is where Entity framework core is interacting with the database
    public class WorkoutTrackingAppContext : DbContext
    {
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<TrackedWorkout> TrackedWorkouts { get; set;}
        public DbSet<WorkoutExercise> WorkoutExercises { get;set; }
        
        public WorkoutTrackingAppContext(DbContextOptions<WorkoutTrackingAppContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add your Message entity configuration here
            modelBuilder.Entity<Message>()
                .HasOne<Account>(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne<Account>(m => m.Recipient)
                .WithMany()
                .HasForeignKey(m => m.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);           
        }
    }
}
