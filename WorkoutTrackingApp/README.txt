Project Information for running:

I have been having an issue getting the project to run correctly right from the download with the database. When I 
Run Update database on Identity first, and then Update database on the WorkoutTrackingAppDb, it usually works. 
But the app does not function unless you run update databse on both Contexts before running the app. Other than that
I think it should start running.


Before running: Update-Database -Context AltercationContext
				Update-Database -Context WorkoutTrackingAppContext

Trainer 1. would be the best to use, added the others just for demonstration aspects. 


UserNames and PassWords:

I have added the following accounts into the program that should initialize through DbIntilizer when the app runs
	-Trainer

		1. UserName: JackTheTrainer@gmail.com
		   PassWord: $Tr@in3rsOnly#!%	

		   There should be some pre-made workouts and exercises created by his user when the app runs. 
		

	-Subscribers

		1. BobTheAthlete@gmail.com
			Arandom@!U$er
		I don't think this one is working though, you should be able to register new users through the app itself though. 

		Thisisapassword123!