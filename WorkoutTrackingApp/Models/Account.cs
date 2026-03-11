using System.ComponentModel.DataAnnotations;

namespace WorkoutTrackingApp.Models
{
    public class Account
    {
        [Key] 
        public int AccountId { get; set; } // Primary key

        public string IdentityUserId { get; set; } // Key to get the Id from identity database

        public virtual ICollection<TrackedWorkout> TrackedWorkouts { get; set; }
    }
}
