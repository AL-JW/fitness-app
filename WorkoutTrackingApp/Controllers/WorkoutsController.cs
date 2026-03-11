using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using WorkoutTrackingApp.Models;
using WorkoutTrackingApp.ViewModels;
using WorkoutTrackingApp.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WorkoutTrackingApp.Controllers
{
    /// <summary>
    /// This controller is for managing all the workouts to implement the CRUD functionalities. 
    /// </summary>
    public class WorkoutsController : Controller
    {
        private readonly WorkoutTrackingAppContext _context; // Defining the database context

        private readonly UserManager<IdentityUser> _userManager; // UserManager

        public WorkoutsController(WorkoutTrackingAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// A list of all workouts.
        /// </summary>
        /// <returns>A view with a list of all the workouts.</returns>
        [AllowAnonymous]
        public IActionResult Index()
        {
            var workouts = _context.Workouts

                                   .Select(w => new WorkoutViewModel
                                   {
                                       WorkoutID = w.WorkoutId,
                                       Name = w.Name,
                                       Author = w.Author,
                                       Description = w.Description,                                    
                                       SelectedExercises = w.WorkoutExercises.Select(we => we.ExerciseId).ToList(),
                                        Exercises = w.WorkoutExercises.Select(we => new ExerciseViewModel
                                        {                                         
                                            Name = we.Exercise.Name,
                                            Description = we.Exercise.Description,
                                           
                                        }).ToList()
                                   })
                                   .ToList();

            return View(workouts);
        }

        /// <summary>
        /// A form to create a new workout.
        /// </summary>
        /// <returns>A view for creating a new workout.</returns>
        [Authorize(Roles = "Trainer")]
        public IActionResult Create()
        {
            ViewBag.ExerciseList = _context.Exercises.ToList();
            return View("ManageWorkouts", new WorkoutViewModel());
        }

        /// <summary>
        /// This then creates the new workout based on the input. 
        /// </summary>
        /// <param name="workoutVM">This contains the workout details.</param>
        /// <param name="SelectedExercises">The exercises that are in a workout.</param>
        /// <returns>Goes back to the homepage. </returns>
        [Authorize(Roles = "Trainer")]
        [HttpPost]
        public async Task<IActionResult> Create(WorkoutViewModel workoutVM, List<int> SelectedExercises)
        {
            // This is to relate the ViewModel to Entity Model
            var workout = new Workout
            {
                Name = workoutVM.Name,
                Author = workoutVM.Author,
                Description= workoutVM.Description,
            };

            if (SelectedExercises != null)
            {
                workout.WorkoutExercises = SelectedExercises.Select(id => new WorkoutExercise { ExerciseId = id }).ToList();
            }
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
            
        }

        /// <summary>
        /// Makes a form that the trainer can edit a workout with, similar to the creating.
        /// </summary>
        /// <param name="id">The workoutId</param>
        /// <returns>View for editing workout..</returns>
        [Authorize(Roles = "Trainer")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Using LINQ here to filter workouts by the logged in user Id, also using the
            // _context to get a specific work out and its related data
            var workout = _context.Workouts
                          .Include(w => w.WorkoutExercises)
                          .ThenInclude(we => we.Exercise)
                          .FirstOrDefault(w => w.WorkoutId == id);

            if (workout == null)
            {
                return NotFound();
            }

            ViewBag.ExerciseList = _context.Exercises.ToList();

            // Connecting the entity model to the view wmodel 
            var workoutViewModel = new WorkoutViewModel
            {
                WorkoutID = workout.WorkoutId,
                Name = workout.Name,
                Author = workout.Author,
                SelectedExercises = workout.WorkoutExercises.Select(we => we.ExerciseId).ToList()
            };
            return View("ManageWorkouts", workoutViewModel);
        }

        /// <summary>
        /// Finalizes the editing of the workout by submitting it to the database.
        /// </summary>
        /// <param name="workoutVM">Updated workout details</param>
        /// <param name="SelectedExercises">Updated exercises in workout.</param>
        /// <returns>View for editing workout..</returns>
        [Authorize(Roles = "Trainer")]
        [HttpPost]
        public IActionResult Edit(WorkoutViewModel workoutVM, List<int> SelectedExercises)
        {
                var existingWorkout = _context.Workouts
                                        .Include(w => w.WorkoutExercises)
                                        .FirstOrDefault(w => w.WorkoutId == workoutVM.WorkoutID);

                if (existingWorkout != null)
                {
                    existingWorkout.Name = workoutVM.Name;
                    existingWorkout.Author = workoutVM.Author;
                    existingWorkout.Description = workoutVM.Description;

                    // Update the WorkoutExercises
                    existingWorkout.WorkoutExercises.Clear();
                    if (SelectedExercises != null)
                    {
                        foreach (var exerciseId in SelectedExercises)
                        {
                            existingWorkout.WorkoutExercises.Add(new WorkoutExercise { WorkoutId = workoutVM.WorkoutID, ExerciseId = exerciseId });
                        }
                    }
                    _context.Update(existingWorkout);
                    _context.SaveChanges();
                }

                return RedirectToAction("Index");

            ViewBag.ExerciseList = _context.Exercises.ToList();
            return View("ManageWorkouts", workoutVM);
        }


        /// <summary>
        /// Confirmation for deleting a workout. 
        /// </summary>
        /// <param name="id">The ID of the workout being deleted.</param>
        /// <returns>An extra confirmation of deleting workout from database.</returns>
        [Authorize(Roles = "Trainer")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            // WorkoutTrackingAppContext _context being used here to delete a workout with LINQ
            var workout = _context.Workouts
                          .Where(w => w.WorkoutId == id)
                          .Select(w => new WorkoutViewModel
                          {
                              WorkoutID = w.WorkoutId,
                              Name = w.Name,
                              Author = w.Author,
                              // Other properties...
                          })
                          .FirstOrDefault();

            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }


        /// <summary>
        /// Executes the deletion of the workout. 
        /// </summary>
        /// <param name="workoutVM">The workout to be removed.</param>
        /// <returns>Goes back to the workouts view. </returns>
        [Authorize(Roles = "Trainer")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(WorkoutViewModel workoutVM)
        {
            // First, delete related WorkoutExercises. This is using LINQ to load the related data.
            var relatedExercises = _context.WorkoutExercises.Where(we => we.WorkoutId == workoutVM.WorkoutID).ToList();
            if (relatedExercises.Any())
            {
                _context.WorkoutExercises.RemoveRange(relatedExercises);
            }


            var workoutToDelete = _context.Workouts.Find(workoutVM.WorkoutID);
            if (workoutToDelete == null)
            {
                return NotFound();
            }

            _context.Workouts.Remove(workoutToDelete);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
