using System.Threading.Tasks;
using GarbageMap.Models.DbModels;
using Microsoft.AspNetCore.Identity;

namespace GarbageMap.Models.Initializer
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<IndividualPerson> userManager, RoleManager<IdentityRole> roleManager)
        {          
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            if (await roleManager.FindByNameAsync("organization") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("organization"));
            }

            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            await CreateAdminAsync(userManager);
            await CreateOrganizationAsync(userManager, "awe@gmail.com", "123456");
            await CreateOrganizationAsync(userManager, "sankom@gmail.com", "123456");
        }

        private static async Task CreateAdminAsync(UserManager<IndividualPerson> userManager)
        {
            var adminEmail = "admin@gmail.com";
            var password = "admin";
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                var admin = new IndividualPerson() { Email = adminEmail, UserName = adminEmail };
                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }

        private static async Task CreateOrganizationAsync(UserManager<IndividualPerson> userManager, string orgEmail, string orgPassword)
        {
            if (await userManager.FindByNameAsync(orgEmail) == null)
            {
                var admin = new IndividualPerson() { Email = orgEmail, UserName = orgEmail };
                var result = await userManager.CreateAsync(admin, orgPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "organization");
                }
            }
        }
    }
}
