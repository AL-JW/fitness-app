using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using WorkoutTrackingApp.Models;

namespace WorkoutTrackingApp.Data
{
    public class DbInitializer
    {
        public static void ClearData(WorkoutTrackingAppContext context)
        {
            // Clearing all workout exercises first because was having trouble with foreign key restraints
            context.WorkoutExercises.RemoveRange(context.WorkoutExercises);
            context.SaveChanges();

            // Clearing the tracked workouts, workouts, and exercises
            context.TrackedWorkouts.RemoveRange(context.TrackedWorkouts);
            context.Workouts.RemoveRange(context.Workouts);
            context.Exercises.RemoveRange(context.Exercises);
            context.SaveChanges();

            // Clearing out the messages and accounts
            context.Messages.RemoveRange(context.Messages);
            context.Accounts.RemoveRange(context.Accounts);
            context.SaveChanges();
        }

        public static async Task InitializeAsync(WorkoutTrackingAppContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Clear data to re initialize
            // ClearData(context);

            // Create database and use Entity Framework model to create tables
            context.Database.EnsureCreated();

            // Check to see if trainer role exists
            if (!await roleManager.RoleExistsAsync("Trainer"))
            {
                await roleManager.CreateAsync(new IdentityRole("Trainer"));
            }

            // Create a trainer user if it doesn't exist
            var trainerEmail = "JackTheTrainer@gmail.com";
            var trainerUser = await userManager.FindByEmailAsync(trainerEmail);
            if (trainerUser == null)
            {
                trainerUser = new IdentityUser { UserName = trainerEmail, Email = trainerEmail };
                var createUserResult = await userManager.CreateAsync(trainerUser, "$Tr@in3rsOnly#!%");
                if (createUserResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(trainerUser, "Trainer");

                    var code = await userManager.GenerateEmailConfirmationTokenAsync(trainerUser);
                    var confirmResult = await userManager.ConfirmEmailAsync(trainerUser, code);

                    // Create and add account linked to this user
                    // Creating the all powerful trainer who can CRUD Exercises and Workouts. 
                    var newAccount = new Account
                    {
                        IdentityUserId = trainerUser.Id // Linking to the created user's ID
                    };
                    context.Accounts.Add(newAccount);
                    context.SaveChanges();

                    // Going to simulate that the trainer added a bunch of workouts and exercises now
                    // Linking them through the 

                    var exercises = new List<Exercise>
                    {
                        new Exercise  // Exercise 1
                        {
                            Name = "Push-Up",
                            Description = "Start in a plank position with your hands shoulder-width apart. " +
                            "Lower your chest to the ground by bending your elbows and then push back up to the starting position.",
                            Author = "Unknown",
                            Intensity = ExerciseIntensity.Medium
                        },

                         new Exercise // Exercise 2
                        {
                            Name = "Squat",
                            Description = "Stand with your feet shoulder-width apart. " +
                            "Lower your body by bending your knees and hips, keeping your back straight, and then return to the standing position.",
                            Author = "Unknown",
                            Intensity = ExerciseIntensity.Medium
                        },

                          new Exercise // Exercise 3
                        {
                            Name = "Burpee",
                            Description = "Stand with your feet shoulder-width apart. " +
                            "Lower your body by bending your knees and hips, keeping your back straight, and then return to the standing position.",
                            Author = "Royal H. Burpee",
                            Intensity = ExerciseIntensity.High
                        },

                           new Exercise // Exercise 4
                        {
                            Name = "Plank",
                            Description = "Start in a push-up position but with your weight on your forearms. " +
                            " Keep your body in a straight line from head to heels, engaging your core muscles.",
                            Author = "Uknown",
                            Intensity = ExerciseIntensity.Low
                        },

                         new Exercise // Exercise 5
                        {
                            Name = "Jumping Jacks",
                            Description = "Start with your feet together and arms at your sides. " +
                            " Jump your feet apart while raising your arms overhead, then return to the starting position. ",
                            Author = "Uknown",
                            Intensity = ExerciseIntensity.Medium
                        },

                          new Exercise // Exercise 6
                        {
                            Name = "Russian Twists",
                            Description = "Sit on the floor with your knees bent and your heels on the ground. " +
                            " Lean back slightly, lift your feet off the ground, and twist your torso from side to side, touching the floor beside your hips. ",
                            Author = "Uknown",
                            Intensity = ExerciseIntensity.Medium
                        },

                           new Exercise // Exercise 7
                        {
                            Name = "Mountain Climbers",
                            Description = "Begin in a plank position with your hands under your shoulders. " +
                            " Alternate bringing your knees towards your chest, as if you're climbing a mountain. ",
                            Author = "Uknown",
                            Intensity = ExerciseIntensity.High
                        },

                            new Exercise // Exercise 8
                        {
                            Name = "Bicycle Crunches",
                            Description = "Lie on your back with your hands behind your head." +
                            "Bring your right elbow and left knee together while extending your right leg out, then switch sides in a pedaling motion.",
                            Author = "Uknown",
                            Intensity = ExerciseIntensity.Medium
                        },

                         new Exercise // Exercise 9 
                        {
                            Name = "Lunges",
                            Description = "Step forward with one leg, lowering your body until both knees are bent at a 90-degree angle, " +
                            " then return to the starting position.",
                            Author = "Uknown",
                            Intensity = ExerciseIntensity.Medium
                        },

                          new Exercise
                        {
                            Name = " Vader's Force Choke Pull-Ups",
                            Description = "Pull-up exercises named after Darth Vader's iconic Force choke, focusing on upper body strength.",
                            Author = "Darth Vader",
                            Intensity = ExerciseIntensity.Medium
                        },
                    };
                    context.Exercises.AddRange(exercises);
                    context.SaveChanges();

                    // Selecting specific exercises for a JediKnightsCoreChallenge workout
                    // Workout 1
                    var JediKnightsCoreChallengeExercises = exercises.Where(e =>
                        e.Name == "Planks" ||
                        e.Name == "Russian Twists" ||
                        e.Name == "Bicycle Crunches"
                    ).ToList();

                    var JediKnightsCoreChallengeWorkout = new Workout
                    {
                        AccountId = newAccount.AccountId,
                        Name = "Jedi Knight's Core Challenge",
                        Author = "Master Yoda",
                        Description = "Strengthen your core with this challenging workout inspired by the wisdom of the Jedi. " +
                        "Feel the Force within you as you perform planks, Russian twists, and bicycle crunches.",
                        WorkoutExercises = JediKnightsCoreChallengeExercises.Select(e => new WorkoutExercise { ExerciseId = e.ExerciseId }).ToList()
                    };
                    context.Workouts.Add(JediKnightsCoreChallengeWorkout);
                    context.SaveChanges();

                    // Workout 2
                    var FullBodyCardioBlastExercises = exercises.Where(e =>
                        e.Name == "Jumping Jacks" ||
                        e.Name == "Squats" ||
                        e.Name == "Mountain Climbers"
                    ).ToList();

                    var FullBodyCardioBlastWorkout = new Workout
                    {
                        AccountId = newAccount.AccountId,
                        Name = "Fully-Body Cardio Blast",
                        Author = "Fitness Trainer",
                        Description = " A high-intensity cardio workout that engages your entire body. " +
                        " This circuit includes squats, jumping jacks, and mountain climbers to improve cardiovascular fitness.",
                        WorkoutExercises = FullBodyCardioBlastExercises.Select(e => new WorkoutExercise { ExerciseId = e.ExerciseId }).ToList()
                    };
                    context.Workouts.Add(FullBodyCardioBlastWorkout);
                    context.SaveChanges();

                    // Workout 3
                    var TotalBodyPowerExercises = exercises.Where(e =>
                        e.Name == "Push-Up" ||
                        e.Name == "Lunges" ||
                        e.Name == "Burpees" ||
                        e.Name == "Vader's Force Choke Pull-Ups"
                    ).ToList();

                    var TotalBodyPowerWorkout = new Workout
                    {
                        AccountId = newAccount.AccountId,
                        Name = "Total Body Power",
                        Author = "Fitness Trainer",
                        Description = "Enhance your strength and power with this challenging workout. " +
                        " It features a combination of push-ups, lunges, burpees, and Vader's Force choke pull-ups to work various muscle groups.",
                        WorkoutExercises = TotalBodyPowerExercises.Select(e => new WorkoutExercise { ExerciseId = e.ExerciseId }).ToList()
                    };
                    context.Workouts.Add(TotalBodyPowerWorkout);
                    context.SaveChanges();

                    // Workout 4
                    var QuickBurnExercises = exercises.Where(e =>
                        e.Name == "Squats" ||
                        e.Name == "Russian-Twists" ||
                        e.Name == "Jumping Jacks"
                    ).ToList();

                    var QuickBurnWorkout = new Workout
                    {
                        AccountId = newAccount.AccountId,
                        Name = "Quick Burn",
                        Author = "Fitness Trainer",
                        Description = "This quick workout is designed to burn calories and boost your metabolism. " +
                        " It includes exercises like squats, Russian twists, and jumping jacks for a full-body burn.",
                        WorkoutExercises = TotalBodyPowerExercises.Select(e => new WorkoutExercise { ExerciseId = e.ExerciseId }).ToList()
                    };
                    context.Workouts.Add(QuickBurnWorkout);
                    context.SaveChanges();

                    // Workout 5

                    var BountyHuntersAgilityExercises = exercises.Where(e =>
                        e.Name == "Mountain Climbers" ||
                        e.Name == "Burpees" ||
                        e.Name == "Russian Twists"
                    ).ToList();

                    var BountyHuntersAgilityWorkout = new Workout
                    {
                        AccountId = newAccount.AccountId,
                        Name = "Bounty Hunter's Agility Challenge",
                        Author = "Boba Fett",
                        Description = " Test your agility and speed with this workout designed by the most feared bounty hunter. " +
                        " Includes mountain climbers, burpees, and Russian twists to improve your overall fitness.",
                        WorkoutExercises = TotalBodyPowerExercises.Select(e => new WorkoutExercise { ExerciseId = e.ExerciseId }).ToList()
                    };
                    context.Workouts.Add(BountyHuntersAgilityWorkout);
                    context.SaveChanges();
                }
            }

            // Create a trainer user if it doesn't exist
            var aUserEmail = "BobTheAthlete@gmail.com";
            var aUser = await userManager.FindByEmailAsync(aUserEmail);
            if (aUser == null)
            {
                aUser = new IdentityUser { UserName = aUserEmail, Email = aUserEmail };
                var createUserResult = await userManager.CreateAsync(aUser, "Arandom@!U$er");
                if (createUserResult.Succeeded)
                {
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(aUser);
                    var confirmResult = await userManager.ConfirmEmailAsync(aUser, code);

                    // Create and add account linked to this user
                    // Creating the all powerful trainer who can CRUD Exercises and Workouts. 
                    var newAccount = new Account
                    {
                        IdentityUserId = aUser.Id // Linking to the created user's ID
                    };
                    context.Accounts.Add(newAccount);
                    context.SaveChanges();
                }
            }
        }
    }
}
