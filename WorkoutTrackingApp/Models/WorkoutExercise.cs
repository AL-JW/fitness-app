using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutTrackingApp.Models
{
    public class WorkoutExercise
    {
        [Key]
        public int WorkoutExerciseId { get; set; } // Primary Key for this join table

         [ForeignKey("Workout")]
          public int WorkoutId { get; set; } // Foreign key to Workout
          public Workout Workout { get; set; } // Navigation property to Workout

         [ForeignKey("Exercise")]

         public int ExerciseId { get; set;} // Foreign key to exercise
         public Exercise Exercise { get; set; } // Navigation to exercise
    }
}
