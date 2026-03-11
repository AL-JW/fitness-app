namespace WorkoutTrackingApp.ViewModels
{
    public class UserDashboardViewModel
    {
        // User profile information
        public string UserName { get; set; }
        public string Email { get; set; }

        // Subscription Details
        public bool IsSubscribed { get; set; }
        public DateTime SubscriptionEndDate { get; set; }
        public IEnumerable<WorkoutViewModel> RecentWorkouts { get; set; } = new List<WorkoutViewModel>();

        // ViewModel for sending a message
        public MessageViewModel SendMessageViewModel { get; set; }
    }
}
