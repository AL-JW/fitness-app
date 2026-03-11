using WorkoutTrackingApp.Models;

namespace WorkoutTrackingApp.ViewModels
{
    public class ExerciseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }     
        public ExerciseIntensity Intensity { get; set; }
        public bool IsCompleted { get; set; }
    }
}
