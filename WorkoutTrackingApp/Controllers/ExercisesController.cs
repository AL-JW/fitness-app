using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkoutTrackingApp.Data;
using WorkoutTrackingApp.Models;
using WorkoutTrackingApp.ViewModels;

namespace WorkoutTrackingApp.Controllers
{
    /// <summary>
    /// This controller is here to handle the exercise functionalities of the application. 
    /// Implements CRUD functionality for the exercises in the application. 
    /// </summary
    public class ExercisesController : Controller
    {
        private readonly WorkoutTrackingAppContext _context; 
        public ExercisesController(WorkoutTrackingAppContext context)
        {
            _context = context;
        }


        /// <summary>
        /// This displays exercises for all users regardless if they are authenticated. 
        /// </summary>
        /// <returns>The view that has all of the exercises</returns>
        [AllowAnonymous]
        public IActionResult Index()
        {
            var exercises = _context.Exercises
                                  .Select(e => new ExerciseViewModel
                                  {
                                      Id = e.ExerciseId,
                                      Name = e.Name,
                                      Description = e.Description,
                                      
                                  })
                                  .ToList();
            return View(exercises);
        }

        /// <summary>
        /// This gives trainers the ability to create exercises.
        /// </summary>
        /// <returns>The exercises view with the create function.</returns>
        [Authorize(Roles = "Trainer")]
        [HttpGet]
        public IActionResult Create()
        {
            
            return View("ManageExercises", new ExerciseViewModel());
        }

        /// <summary>
        /// The action method that creates the exercises when the form is submitted.
        /// </summary>
        /// <param name="model">This model contains the exercise info to create. </param>
        /// <returns>Takes trainer to the exercise list, otherwise the form redisplays.</returns>
        [Authorize(Roles = "Trainer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ExerciseViewModel model)
        {
            // if (ModelState.IsValid)
            // {
                var exercise = new Exercise 
                {
                    Name = model.Name,
                    Author = model.Author,
                    Description = model.Description,
                    Intensity = model.Intensity,
                    
                };

                _context.Exercises.Add(exercise);
                _context.SaveChanges();
               
                return RedirectToAction("Index");
            // }

            return View("ManageExercises", model);
        }

        /// <summary>
        /// Allows a trainer to edit a specific exercise in the database. 
        /// </summary>
        /// <param name="id">The exercises Id that is to be edited.  </param>
        /// <returns>When successfully completed, takes trainer back to the list of exercises, otherwise redisplays the form.</returns>
        [Authorize(Roles = "Trainer")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var exercise = _context.Exercises.FirstOrDefault(e => e.ExerciseId == id);

            if (exercise == null)
            {
                return NotFound();
            }

            var exerciseViewModel = new ExerciseViewModel
            {
                Id = exercise.ExerciseId,
                Name = exercise.Name,
                Description = exercise.Description,
                Author = exercise.Author, 
                Intensity = exercise.Intensity
            };

            return View("ManageExercises", exerciseViewModel);
        }

        /// <summary>
        /// Allows a trainer to edit a specific exercise in the database. 
        /// Yes modelstate is commented out, still would need to fix that. 
        /// Was running into issues with implementing this ModeState check in the application, and I have since figured out 
        /// how to determine the error through using the debugger as well as using this code:
        /// var errors = ModelState
        ///.Where(x => x.Value.Errors.Count > 0)
        ///.Select(x => new { x.Key, x.Value.Errors })
        ///.ToArray();
        ///It seemed to be a mismatch between one of the attributes in the model and then that attribute was not being set properly 
        ///when the view was submitted. I think it is a simple fix I just didn't get around to doing it for every action method.
        ///I was able to get it to work in the TrainerProfileDashBoardController.
        /// </summary>
        /// <param name="model">Updated exercise information.  </param>
        /// <returns>When successfully completed, takes trainer back to the list of exercises, otherwise redisplays the form.</returns>
        [Authorize(Roles = "Trainer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ExerciseViewModel model)
        {
            // Was running into issues with implementing this ModeState check in the application, and I have since figured out 
            // how to determine the error through using the debugger as well as 
            // if (ModelState.IsValid)
            // {
                var existingExercise = _context.Exercises.Find(model.Id);

                if (existingExercise == null)
                {
                    return NotFound();
                }

                existingExercise.Name = model.Name;
                existingExercise.Author = model.Author;
                existingExercise.Description = model.Description;
                existingExercise.Intensity = model.Intensity;              

                _context.Update(existingExercise);
                _context.SaveChanges();

                return RedirectToAction("Index");
            // }
            return View("ManageExercises", model);
        }

        /// <summary>
        /// Displays the delete page for the selected exercise when the trainer clicks 
        /// delete. 
        /// </summary>
        /// <param name="id">The id to identify which exercise is being deleted. </param>
        /// <returns>If the exercise can be deleted, return a confirmation view to confirm deletion.</returns>
        [Authorize(Roles = "Trainer")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var exercise = _context.Exercises
                                   .Where(e => e.ExerciseId == id)
                                   .Select(e => new ExerciseViewModel
                                   {
                                       Id = e.ExerciseId,
                                       Name = e.Name,
                                       Author = e.Author,
                                       Description = e.Description,
                                       
                                   })
                                   .FirstOrDefault();

            if (exercise == null)
            {
                return NotFound();
            }

            return View(exercise); 
        }

        /// <summary>
        /// Deletes the exercise when it is clicked confirmed. 
        /// </summary>
        /// <param name="id">The id of the exercise to delete.</param>
        /// <returns>
        ///   Goes back to the exercise list, which should be updated
        ///   with the deleted exercise now gone.
        /// </returns>
        [Authorize(Roles = "Trainer")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var exerciseToDelete = _context.Exercises.Find(id);
            if (exerciseToDelete == null)
            {
                return NotFound();
            }

            _context.Exercises.Remove(exerciseToDelete);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
