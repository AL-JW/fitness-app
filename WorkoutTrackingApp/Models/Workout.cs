using Microsoft.EntityFrameworkCore.Query;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutTrackingApp.Models
{
    public class Workout
    {
        public Workout()
        {
            WorkoutExercises = new HashSet<WorkoutExercise>();
        }

        public int AccountId { get; set; } // Foreign key to the account Model Entity // I think I only 
                                           // need this in the tracked workouts table

         public virtual Account Account { get; set; }    // Navigation Property 

        [Key]
        public int WorkoutId { get; set; } // Primary key

       // [Required]
        [Display(Name = "Workout Name")]
        public string Name { get; set; }

        // [Required]
        [Display(Name = "Author")]
        public string Author { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; }
    }
}
