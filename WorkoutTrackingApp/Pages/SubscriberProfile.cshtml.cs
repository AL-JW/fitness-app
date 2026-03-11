using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutTrackingApp.Data;
using WorkoutTrackingApp.Models; 


public class SubscriberProfileModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly WorkoutTrackingAppContext _context; 

    public SubscriberProfileModel(UserManager<IdentityUser> userManager, WorkoutTrackingAppContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public string Username { get; set; }
    public List<TrackedWorkout> TrackedWorkouts { get; set; }

    public async Task OnGetAsync()
    {
        var identityUserId = _userManager.GetUserId(User);
        var userAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.IdentityUserId == identityUserId);
        if (userAccount != null)
        {
            Username = _userManager.GetUserName(User);

            TrackedWorkouts = await _context.TrackedWorkouts
                .Include(tw => tw.Workout) 
                .Where(tw => tw.AccountId == userAccount.AccountId) 
                .ToListAsync();
        }
    }
}