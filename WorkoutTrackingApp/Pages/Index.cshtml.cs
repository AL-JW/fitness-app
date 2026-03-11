using WorkoutTrackingApp.Data;
using WorkoutTrackingApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTrackingApp.Controllers;

public class IndexModel : PageModel
{
    private readonly WorkoutTrackingAppContext _context;

    public IndexModel(WorkoutTrackingAppContext context)
    {
        _context = context;
    }

    public List<WorkoutViewModel> Workouts { get; set; }
    public List<ExerciseViewModel> Exercises { get; set; }

    public void OnGet()
    {
        Workouts = FetchWorkouts();
        Exercises = FetchExercises();
    }

    private List<WorkoutViewModel> FetchWorkouts()
    {
        var workoutList = _context.Workouts
            .Include(w => w.WorkoutExercises)
            .ThenInclude(we => we.Exercise)
            .Select(w => new WorkoutViewModel
            {
                WorkoutID = w.WorkoutId,
                Name = w.Name,
                Author = w.Author,
                Description = w.Description,
                Exercises = w.WorkoutExercises.Select(we =>
                    new ExerciseViewModel
                    {
                        // Assuming ExerciseViewModel has properties like Id, Name, etc.
                        Id = we.Exercise.ExerciseId,
                        Name = we.Exercise.Name,
                        // ... other properties ...
                    }
                ).ToList()
            })
            .ToList();

        return workoutList;
    }

    private List<ExerciseViewModel> FetchExercises()
    {
        // Adjust this method to fetch exercises instead of workouts
        var exerciseList = _context.Exercises
            // Add any relevant .Include() if needed
            .Select(e => new ExerciseViewModel
            {
                // Map properties from Exercise to ExerciseViewModel
                Id = e.ExerciseId,
                Name = e.Name,
                Description = e.Description
                // ... other properties ...
            })
            .ToList();

        return exerciseList;
    }
}
