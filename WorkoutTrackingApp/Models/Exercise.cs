using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace WorkoutTrackingApp.Models
{
    public enum ExerciseIntensity
    {
        Low,
        Medium,
        High
    }

    public class Exercise
    {

        [Key]
        public int ExerciseId { get; set; } // PrimaryKey

        [Required]
        [Display(Name = "Exercise Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Author")]
        public string Author { get; set; }


        [Required]
        [Display(Name = "Intensity")]
        public ExerciseIntensity Intensity{ get; set; }

        // Navigation property for join table WorkoutExercise
        public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; }

    }
}
