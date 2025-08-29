using A_Very_Simple_HIS.Data;
using A_Very_Simple_HIS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace A_Very_Simple_HIS
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddAuthorization(options =>
            {
                // Patients
                options.AddPolicy("Patients.View", p => p.RequireClaim("Permission", "Patients.View", "Patients.FullAccess"));
                options.AddPolicy("Patients.Create", p => p.RequireClaim("Permission", "Patients.Create", "Patients.FullAccess"));
                options.AddPolicy("Patients.Edit", p => p.RequireClaim("Permission", "Patients.Edit", "Patients.FullAccess"));

                // Doctors
                options.AddPolicy("Doctors.View", p => p.RequireClaim("Permission", "Doctors.View", "Doctors.FullAccess"));
                options.AddPolicy("Doctors.Create", p => p.RequireClaim("Permission", "Doctors.Create", "Doctors.FullAccess"));
                options.AddPolicy("Doctors.Edit", p => p.RequireClaim("Permission", "Doctors.Edit", "Doctors.FullAccess"));

                // Visits
                options.AddPolicy("Visits.View", p => p.RequireClaim("Permission", "Visits.View", "Visits.FullAccess"));
                options.AddPolicy("Visits.Create", p => p.RequireClaim("Permission", "Visits.Create", "Visits.FullAccess"));
                options.AddPolicy("Visits.Edit", p => p.RequireClaim("Permission", "Visits.Edit", "Visits.FullAccess"));

                // Admin/perhaps other global policies
                options.AddPolicy("ManageUsers", p => p.RequireClaim("Permission", "ManageUsers", "Admin.FullAccess"));
            });

            builder.Services.AddControllersWithViews();



            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await Seed.SeedDataAsync(services); // runs once here
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
