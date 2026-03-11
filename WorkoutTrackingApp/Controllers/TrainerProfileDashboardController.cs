using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkoutTrackingApp.Data;
using WorkoutTrackingApp.ViewModels;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic; // this is for List<T>
using System.Linq;
using WorkoutTrackingApp.Models;

namespace WorkoutTrackingApp.Controllers
{
    /// <summary>
    /// Controller for functions related to the Trainer role. Enables
    /// Trainers to have a little more authority than subscribers. 
    /// </summary>
    public class TrainerProfileDashboardController : Controller
    {
        private readonly WorkoutTrackingAppContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <param name="context">The apps main database context.</param>
        /// <param name="userManager">This is for accessing users in the identity database.</param>
        public TrainerProfileDashboardController(WorkoutTrackingAppContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        /// <summary>
        /// Displays the main home page for the Trainers Dashboard. 
        /// </summary>
        /// <returns>The view for the Trainer dashboard.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Implementing Messaging feature action methods now, displays the send message form.
        /// </summary>
        /// <returns>The view for the Sending a message to the users</returns>

        [Authorize(Roles = "Trainer")]
        [HttpGet]
        public async Task<IActionResult> SendMessage()
        {
            var usersList = await GetUsersList();
            var workoutsList = await GetWorkoutsList();
            var exercisesList = await GetExercisesList();
            var viewModel = new MessageViewModel
            {
                Users = usersList,
                Workouts = workoutsList,
                Exercises = exercisesList
                               
            };
            return View(viewModel);
        }

        /// <summary>
        /// Handles the submission of a message from a trainer to a recipient.
        /// Takes the information submitted by the Trainer and sends it to the recipient which 
        /// is the user. 
        /// </summary>
        /// <param name="model">All the message information like workout, exercise, and message content.</param>
        /// <returns>
        ///   If the message sends, it takes you back to the dashboard, otherwise, it should display and error
        ///   or just redisplay the message form. 
        /// </returns>
        [Authorize(Roles = "Trainer")]
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
            model.Users = await GetUsersList();
            return View(model);
        }

        /// <summary>
        /// When a message is created there is an option to select which user to send it to.
        /// </summary>
        /// <returns>
        ///   A list of the current users that are able to recieve messages.
        /// </returns>
        [Authorize(Roles = "Trainer")]
        private async Task<List<SelectListItem>> GetUsersList()
        {
            // Getting all the users 
            var usersList = new List<SelectListItem>();

                
            var users = _userManager.Users.ToList();

            foreach (var user in users)
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.IdentityUserId == user.Id);
                if (account != null)
                {                  
                    usersList.Add(new SelectListItem { Value = account.AccountId.ToString(), Text = user.UserName });
                }
            }
            return usersList;
        }
        /// <summary>
        /// A method that gives a list of workouts to specify the subject of the message.
        /// </summary>
        /// <returns>
        /// Returns a list of workouts to select from to message about.
        /// </returns>
        [Authorize(Roles = "Trainer")]
        private async Task<List<SelectListItem>> GetWorkoutsList()
        {
            return await _context.Workouts.Select(w => new SelectListItem
            {
                Value = w.WorkoutId.ToString(),
                Text = w.Name
            }).ToListAsync();
        }

        /// <summary>
        /// A method that gives a list of exercies to specify in the subject of the message.
        /// </summary>
        /// <returns>
        /// Returns a list of exercises to select from to message about.
        /// </returns>
        [Authorize(Roles = "Trainer")]
        private async Task<List<SelectListItem>> GetExercisesList()
        {
            return await _context.Exercises.Select(e => new SelectListItem
            {
                Value = e.ExerciseId.ToString(),
                Text = e.Name
            }).ToListAsync();
        }

        /// <summary>
        /// A method to allow Trainers to view all their messages, like an inbox. 
        /// </summary>
        /// <returns>
        /// Returns all the messages a trainer has linked to their account. 
        /// </returns>
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> ViewMessages()
        {
            var userId = _userManager.GetUserId(User); // Logged in users Id

            var userAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.IdentityUserId == userId);
            if (userAccount == null)
            {
                return NotFound("User account not found.");
            }
            var accountId = userAccount.AccountId;
            // Getting all the messages
            var messages = await _context.Messages
                                        .Where(m => m.SenderId == accountId || m.RecipientId == accountId)
                                        .Include(m => m.Sender)
                                        .Include(m => m.Recipient)
                                        .Include(m => m.SelectedWorkout)
                                        .Include(m => m.SelectedExercise)
                                        .ToListAsync();

            // Creating this to help display the emails
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

        /// <summary>
        /// This is here so that a trainer can view all subscribers workouts that 
        /// are being tracked.
        /// </summary>
        /// <returns>
        /// Returns all the tracked workouts that each user has tied to their account. 
        /// </returns>
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> ViewSubscriberWorkouts()
        {
            var viewModel = new List<SubscriberViewModel>();

            var subscribers = await _context.Accounts.ToListAsync();

            foreach (var subscriber in subscribers)
            {
                var identityUser = await _userManager.FindByIdAsync(subscriber.IdentityUserId);
                if (identityUser == null)
                    continue;

                var trackedWorkouts = await _context.TrackedWorkouts
                    .Where(tw => tw.AccountId == subscriber.AccountId)
                    .Include(tw => tw.Workout)
                        .ThenInclude(w => w.WorkoutExercises)
                        .ThenInclude(we => we.Exercise)
                    .ToListAsync();

                var workoutsViewModel = trackedWorkouts.Select(tw => new TrackedWorkoutViewModel
                {
                    TrackedWorkoutId = tw.TrackedWorkoutId,
                    WorkoutId = tw.WorkoutId,
                    DateCompleted = tw.DateCompleted,
                    WorkoutName = tw.Workout.Name,
                    Exercises = tw.Workout.WorkoutExercises.Select(we =>
                        new ExerciseViewModel
                        {
                            Name = we.Exercise.Name,
                          
                        }
                    ).ToList()
                }).ToList();

                viewModel.Add(new SubscriberViewModel
                {
                    Email = identityUser.Email, // Should be having a username but only figured out email
                    TrackedWorkouts = workoutsViewModel
                });
            }
            return View(viewModel);
        }
    }
}
