using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using WorkoutTrackingApp.ViewModels;
using System;
using System.Collections.Generic; // this is for List<T>
using Microsoft.AspNetCore.Authorization;
using WorkoutTrackingApp.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WorkoutTrackingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;

// Central hub for user dashboard
// Here I can incorporate latest workout/ or workouts, don't need a profile picture i don't think, profile view was completly unneccesary since the index is the profile
// Don't need to manage subscription as a user so that view was deleted
// Basic Settings view can be integrated here into the index I think, possibly. 
namespace WorkoutTrackingApp.Controllers
{
    /// <summary>
    /// This controller is for managing the user dashboard and the user profile. Provides
    /// ablilities to view workouts, track workouts, and send messages. 
    /// </summary>
    public class UserProfileDashboardController : Controller
    {
        private readonly WorkoutTrackingAppContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserProfileDashboardController(WorkoutTrackingAppContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Shows the main dashboard for a user and recent workouts too. 
        /// </summary>
        /// <returns>The view of the user dashbaord.</returns>
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Get the current Identity user's ID
            var identityUserId = _userManager.GetUserId(User);

            // Find the corresponding account
            var userAccount = await _context.Accounts
                                            .FirstOrDefaultAsync(a => a.IdentityUserId == identityUserId);

            if (userAccount == null)
            {
                return NotFound(); // or handle appropriately
            }

            var userId = userAccount.AccountId;

            // Fetch recent tracked workouts including the workout details
            var recentTrackedWorkouts = await _context.TrackedWorkouts
                .Where(tw => tw.AccountId == userId)
                .OrderByDescending(tw => tw.DateCompleted)
                .Take(10)
                .Include(tw => tw.Workout) // Include the Workout entity
                .ToListAsync();

            // Map to WorkoutViewModels
            var workoutViewModels = recentTrackedWorkouts.Select(tw => new WorkoutViewModel
            {
                WorkoutID = tw.Workout.WorkoutId,
                Name = tw.Workout.Name,
                Author = tw.Workout.Author,
                Description = tw.Workout.Description,
                DateCompleted = tw.DateCompleted // Include the DateCompleted
            }).ToList();

            var messageViewModel = new MessageViewModel
            {
                Trainers = await GetTrainersList()
            };

            var viewModel = new UserDashboardViewModel
            {
                UserName = (await _userManager.FindByIdAsync(identityUserId))?.Email,
                Email = (await _userManager.FindByIdAsync(identityUserId))?.Email,
                RecentWorkouts = workoutViewModels,
                SendMessageViewModel = messageViewModel
            };

            return View(viewModel);
        }

        /// <summary>
        /// Shows a list of workouts for the user of which they can track.
        /// </summary>
        /// <returns>A view that has a list of workouts they can click to track from.</returns>
        [Authorize]
        public async Task<IActionResult> SelectedWorkouts()
        {
            var workouts = await _context.Workouts
                                .Select(w => new WorkoutViewModel
                                {
                                    //Putting Workout properties to WorkoutViewModel
                                    WorkoutID = w.WorkoutId,
                                    Name = w.Name,
                                    Description = w.Description,                                 
                                })
                                .ToListAsync();
            return View(workouts);
        }

        // Tracked Workout Methods:

        /// <summary>
        /// Starts the tracking of a selected workout.
        /// </summary>
        /// <param name="workoutId">Workout Id being tracked.</param>
        /// <returns>They view to track a workout.</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> TrackWorkout(int workoutId)
        {
            var workout = await _context.Workouts
                                .Include(w => w.WorkoutExercises)
                                .ThenInclude(we => we.Exercise)
                                .FirstOrDefaultAsync(w => w.WorkoutId == workoutId);

            if (workout == null)
            {
                return NotFound(); 
            }

            var exercisesViewModels = workout.WorkoutExercises
                                    .Select(we => new ExerciseViewModel
                                    {
                                        Id = we.Exercise.ExerciseId,
                                        Name = we.Exercise.Name,
                                        IsCompleted = false // Default value
                                    }).ToList();

            var viewModel = new TrackedWorkoutViewModel
            {
                WorkoutName = workout.Name,
                WorkoutId = workout.WorkoutId,            
                DateCompleted = DateTime.Now, // Default to current date
                Exercises = exercisesViewModels
            };
            return View(viewModel);
        }


        // Tracked Workout Methods:

        /// <summary>
        /// Tracks the workout to the database when you click track in the view.
        /// </summary>
        /// <param name="model">Information about the workout being tracked.</param>
        /// <returns>Back to the user dashboard.</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> TrackWorkout(TrackedWorkoutViewModel model)
        
        {
            // Code to help figure out model validation errors
            //var errors = ModelState
            //.Where(x => x.Value.Errors.Count > 0)
            //.Select(x => new { x.Key, x.Value.Errors })
            //.ToArray();
            
            if (ModelState.IsValid)
            {
                // Getting the IdentityUserId of the logged in user
                var identityUserId = _userManager.GetUserId(User);

                
                var account = await _context.Accounts
                                            .FirstOrDefaultAsync(a => a.IdentityUserId == identityUserId);

                if (account == null)
                {
                    // If this hits something is wrong/ no user was found
                    return NotFound();
                }

                var trackedWorkout = new TrackedWorkout
                {
                    WorkoutId = model.WorkoutId,
                    AccountId = account.AccountId,
                    DateCompleted = model.DateCompleted,
                    
                };

                _context.TrackedWorkouts.Add(trackedWorkout);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); // Go back tothe  dashboard
            }

            return View(model); // Return the form view for corrections if model state is invalid
        }


        // ...End of Tracked Workout methods....



        // Implementing Messaging feature action methods now
        /// <summary>
        /// Takes the user to the send message view and prepares it to be used.
        /// </summary>
        /// <returns>The view for sending a message. With the trainers, exercises, and workouts to choose from.</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SendMessage()
        {
            var trainersList = await GetTrainersList();
            var workoutsList = await GetWorkoutsList();
            var exercisesList = await GetExercisesList();
            var viewModel = new MessageViewModel
            {
                Trainers = trainersList,
                Workouts = workoutsList,
                Exercises = exercisesList                          
            };
            return View(viewModel);
        }


        /// <summary>
        /// Sends the message after the form has been filled out correctly.
        /// </summary>
        /// <param name="model">All the message info including the content, trainer, workout, and exercises specified.</param>
        /// <returns>Back to the user dashboard.</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendMessage(MessageViewModel model)
        {
           // var errors = ModelState
           //.Where(x => x.Value.Errors.Count > 0)
           //.Select(x => new { x.Key, x.Value.Errors })
           //.ToArray();
            if (ModelState.IsValid) 
            {
                var senderAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.IdentityUserId == _userManager.GetUserId(User));
                
                if (senderAccount == null)
                {                   
                    return NotFound("Sender account not found.");
                }
                var message = new Message
                {
                    SenderId = senderAccount.AccountId,
                    RecipientId = model.RecipientId,
                    Content = model.Content,
                    SelectedWorkoutId = model.SelectedWorkoutId, 
                    SelectedExerciseId = model.SelectedExerciseId 
                };
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();              
                return RedirectToAction("Index");
            }
            model.Trainers = await GetTrainersList();
            return View(model);
        }

        /// <summary>
        /// Method to populate trainers in the message form.
        /// </summary>
        /// <returns>list of trainers to choose from.</returns>
        [Authorize]
        private async Task<List<SelectListItem>> GetTrainersList()
        {
            
            var trainers = new List <SelectListItem>();

            // Getting the trainer roles
            var trainerUsers = await _userManager.GetUsersInRoleAsync("Trainer");

            foreach (var user in trainerUsers)
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.IdentityUserId == user.Id);
                if (account != null)
                {
                    trainers.Add(new SelectListItem { Value = account.AccountId.ToString(), Text = user.UserName });
                }
            }
            return trainers;
        }

        /// <summary>
        /// Method to fill up the workouts to choose from in message subject.
        /// </summary>
        /// <returns>list of workouts to specify.</returns>
        [Authorize]
        private async Task<List<SelectListItem>> GetWorkoutsList()
        {
            return await _context.Workouts.Select(w => new SelectListItem
            {
                Value = w.WorkoutId.ToString(),
                Text = w.Name
            }).ToListAsync();
        }

        /// <summary>
        /// Method to fill up the exercises to choose from in message subject.
        /// </summary>
        /// <returns>list of exercises to specify.</returns>
        [Authorize]
        private async Task<List<SelectListItem>> GetExercisesList()
        {
            return await _context.Exercises.Select(e => new SelectListItem
            {
                Value = e.ExerciseId.ToString(),
                Text = e.Name
            }).ToListAsync();
        }

        /// <summary>
        /// Grabs all the messages associated with an account to be shown in a view.
        /// </summary>
        /// <returns>All messages related to an account. </returns>
        [Authorize]
        public async Task<IActionResult> ViewMessages()
        {
            var userId = _userManager.GetUserId(User); 

            
            var userAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.IdentityUserId == userId);
            if (userAccount == null)
            {
                return NotFound("User account not found.");
            }
            var accountId = userAccount.AccountId;

            // Retrieving messages where the current user is the sender or recipient
            var messages = await _context.Messages
                                        .Where(m => m.SenderId == accountId || m.RecipientId == accountId)
                                        .Include(m => m.Sender)
                                        .Include(m => m.Recipient)
                                        .Include(m => m.SelectedWorkout)
                                        .Include(m => m.SelectedExercise)
                                        .ToListAsync();
           
            var userInfos = new Dictionary<int, string>();
          
            foreach (var message in messages)
            {
                if (!userInfos.ContainsKey(message.SenderId))
                {
                    var senderUser = await _userManager.FindByIdAsync(message.Sender.IdentityUserId);
                    userInfos[message.SenderId] = senderUser?.Email;
                }

                if (!userInfos.ContainsKey(message.RecipientId))
                {
                    var recipientUser = await _userManager.FindByIdAsync(message.Recipient.IdentityUserId);
                    userInfos[message.RecipientId] = recipientUser?.Email;
                }
            }
            ViewBag.UserInfos = userInfos;
            return View(messages);
        }      
    }
}
