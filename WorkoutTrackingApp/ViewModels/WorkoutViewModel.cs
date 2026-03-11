namespace WorkoutTrackingApp.ViewModels
{
    public class WorkoutViewModel
    {
        public int WorkoutID { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }

        // For tracked workout history displaying purposes
        public DateTime? DateCompleted { get; set; }
        public List<int> SelectedExercises { get; set; }
        // A list of exercises for this workout.
        public List<ExerciseViewModel> Exercises { get; set; }
    }
}
