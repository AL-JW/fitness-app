using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutTrackingApp.Models
{
    public class TrackedWorkout
    {
        /// <value>
        /// The primary key for the TrackedWorkout.
        /// </value>
        [Key]
        public int TrackedWorkoutId { get; set; } // The primary key for tracked workouts

        /// <value>
        ///1st Foreign key for a workout, the ID of the associated Workout.
        /// </value>
        [Required]
        public int WorkoutId { get; set; } 
        public virtual Workout Workout { get; set; } // Navigation property to Workout

        /// <value>
        ///2st Foreign key to an account, the ID of the associated account.
        /// </value>
        [Required]
        public int AccountId { get; set; } // Foreign key to Account
        public virtual Account Account { get; set; } // Navigation property to Account


        /// <value>
        ///Date Completed value 
        /// </value>
        [Required]
        [Display(Name = "Date Completed")]
        [DataType(DataType.Date)]
        public DateTime DateCompleted { get; set; }
    }
}
