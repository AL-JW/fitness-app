using System.ComponentModel.DataAnnotations;
using WorkoutTrackingApp.Models;

namespace WorkoutTrackingApp.ViewModels
{
    public class TrackedWorkoutViewModel
    {
        public int TrackedWorkoutId { get; set; } // For identification
        [Required]
        [Display(Name = "Workout")]
        public int WorkoutId { get; set; } // To store the selected workout ID
        [Required]
        [Display(Name = "Date Completed")]
        [DataType(DataType.Date)]
        public DateTime DateCompleted { get; set; } // To store when the workout was completed

        // List of exercises to be displayed with checkmarks
        public string WorkoutName { get; set; } // Add this property
        public List<ExerciseViewModel> Exercises { get; set; } = new List<ExerciseViewModel>();
    }
}
