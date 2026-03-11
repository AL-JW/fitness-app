using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace WorkoutTrackingApp.ViewModels
{
    public class MessageViewModel
    {  
        //[Display(Name = "Recipient")]
        public int RecipientId { get; set; }
        public string Content { get; set; }
        // Trainers for the drop down to choose who to message. 
        public List<SelectListItem> Trainers { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();
        // Workouts drop down 
        public List<SelectListItem> Workouts { get; set; } = new List<SelectListItem>();
        // Exercises drop down 
        public List<SelectListItem> Exercises { get; set; } = new List<SelectListItem>();
        [Display(Name = "Select Workout")]
        public int? SelectedWorkoutId { get; set; }
        [Display(Name = "Select Exercise")]
        public int? SelectedExerciseId { get; set; }
    }
}

