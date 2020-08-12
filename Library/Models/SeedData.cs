using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models
{
  public static class SeedData
  {
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
      using (var context = new LibraryContext(
        serviceProvider.GetRequiredService<DbContextOptions<LibraryContext>>()))
      {
        var adminID = await EnsureUser(serviceProvider, "admin", "admin");
        await EnsureRole(serviceProvider, adminID, Constants.AdministratorsRole);

        SeedDB(serviceProvider, context, adminID);
      }
    }

    private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string password, string username)
    {
      var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
      var user = await userManager.FindByNameAsync(username);
      if (user == null)
      {
        user = new IdentityUser { UserName = username };
        await userManager.CreateAsync(user, password);
      }
      return user.Id;
    }

    private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
    {
      IdentityResult IR = null;
      var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

      if (roleManager == null)
      {
        throw new Exception("rolemanager null");
      }

      if (!await roleManager.RoleExistsAsync(role))
      {
        IR = await roleManager.CreateAsync(new IdentityRole(role));
      }

      var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
      var user = await userManager.FindByIdAsync(uid);
      if (user == null)
      {
        throw new Exception("Error!");
      }

      IR = await userManager.AddToRoleAsync(user, role);

      return IR;
    }

    public async static void SeedDB(IServiceProvider serviceProvider, LibraryContext context, string adminID)
    {
      if (context.Patrons.Any())
      {
        return;
      }
      var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
      var admin = await userManager.FindByIdAsync(adminID) as ApplicationUser;
      context.Patrons.Add(new Patron() { FirstName = "John", LastName = "Doe", FullName = "John Doe", User = admin });
      context.SaveChanges();
    }
  }
}