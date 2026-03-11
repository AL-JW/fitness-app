using Microsoft.EntityFrameworkCore;
using WorkoutTrackingApp.Data;
using Microsoft.AspNetCore.Identity;
using WorkoutTrackingApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace WorkoutTrackingApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // Adding DbContext to the service container, indicating that WorkoutTrackingAppContext is registered
            builder.Services.AddDbContext<WorkoutTrackingAppContext>(options =>
                                            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure Identity with roles
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                // Password settings and other options
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 2;
            })
            .AddEntityFrameworkStores<AltercationContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

            //

            // Adding DbContext to the service container
            builder.Services.AddDbContext<AltercationContext>(options =>
                                            options.UseSqlServer(builder.Configuration.GetConnectionString("AltercationContextConnection")));
            // This builds the application
            var app = builder.Build();

            // Since the DbContext is a "scoped service", need to create a scope to retrieve the service
            using (var scope = app.Services.CreateScope())
            {
            // Service provider for the scope
                var services = scope.ServiceProvider;
                try
                {


                    // Get the DbContext from the service provider
                    var workoutContext = services.GetRequiredService<WorkoutTrackingAppContext>();
                    var altercationContext = services.GetRequiredService<AltercationContext>();

                    // Apply migrations for both contexts
                    workoutContext.Database.Migrate();
                    altercationContext.Database.Migrate();

                    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    DbInitializer.InitializeAsync(workoutContext, userManager, roleManager).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error"); // Points to the Razor page now instead of controller
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Top-level route registrations
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();
            app.MapFallbackToFile("Index");

            app.Run();
        }
    }
}