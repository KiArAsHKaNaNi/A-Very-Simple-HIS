using A_Very_Simple_HIS.Models;
using Microsoft.AspNetCore.Identity;

namespace A_Very_Simple_HIS.Data
{
    public static class Seed
    {
        //Initializing the data for Authorization suing role -> claims
        public static async Task SeedDataAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = new[] { "Admin", "Supervisor", "Doctor", "Receptionist" };

            foreach (var r in roles)
            {
                if (await roleManager.RoleExistsAsync(r) == false)
                    await roleManager.CreateAsync(new IdentityRole(r));
            }

            // Role -> claims mapping
            // Admin: full access
            var adminRole = await roleManager.FindByNameAsync("Admin");
            await AddRoleClaimIfMissing(roleManager, adminRole, "Permission", "Patients.FullAccess");
            await AddRoleClaimIfMissing(roleManager, adminRole, "Permission", "Doctors.FullAccess");
            await AddRoleClaimIfMissing(roleManager, adminRole, "Permission", "Visits.FullAccess");
            await AddRoleClaimIfMissing(roleManager, adminRole, "Permission", "ManageUsers");

            // Supervisor: can add patients & doctors and view visits
            var supervisorRole = await roleManager.FindByNameAsync("Supervisor");
            await AddRoleClaimIfMissing(roleManager, supervisorRole, "Permission", "Patients.View");
            await AddRoleClaimIfMissing(roleManager, supervisorRole, "Permission", "Patients.Create");
            await AddRoleClaimIfMissing(roleManager, supervisorRole, "Permission", "Patients.Edit");
            await AddRoleClaimIfMissing(roleManager, supervisorRole, "Permission", "Doctors.View");
            await AddRoleClaimIfMissing(roleManager, supervisorRole, "Permission", "Doctors.Create");
            await AddRoleClaimIfMissing(roleManager, supervisorRole, "Permission", "Doctors.Edit");
            await AddRoleClaimIfMissing(roleManager, supervisorRole, "Permission", "Visits.View");

            // Doctor: can view visits only
            var doctorRole = await roleManager.FindByNameAsync("Doctor");
            await AddRoleClaimIfMissing(roleManager, doctorRole, "Permission", "Visits.View");

            // Receptionist: add/edit patients & visits and view
            var receptionistRole = await roleManager.FindByNameAsync("Receptionist");
            await AddRoleClaimIfMissing(roleManager, receptionistRole, "Permission", "Patients.Create");
            await AddRoleClaimIfMissing(roleManager, receptionistRole, "Permission", "Patients.Edit");
            await AddRoleClaimIfMissing(roleManager, receptionistRole, "Permission", "Patients.View");
            await AddRoleClaimIfMissing(roleManager, receptionistRole, "Permission", "Visits.Create");
            await AddRoleClaimIfMissing(roleManager, receptionistRole, "Permission", "Visits.Edit");
            await AddRoleClaimIfMissing(roleManager, receptionistRole, "Permission", "Visits.View");

            // Create a default admin user if not exists
            var adminEmail = "admin@his.local";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true};
                var createAdmin = await userManager.CreateAsync(adminUser, "Admin123!");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        static async Task AddRoleClaimIfMissing(RoleManager<IdentityRole> rm, IdentityRole role, string type, string value)
        {
            var claims = await rm.GetClaimsAsync(role);
            if (claims.Any(c => c.Type == type && c.Value == value) == false)
            {
                await rm.AddClaimAsync(role, new System.Security.Claims.Claim(type, value));
            }
        }
    }
}
