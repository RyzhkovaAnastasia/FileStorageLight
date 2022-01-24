using DAL.Context;
using DAL.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;

namespace DAL.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<FileStorageDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FileStorageDbContext context)
        {
            context.Roles.AddOrUpdate(
                new IdentityRole() { Name = "Manager" },
                new IdentityRole() { Name = "User" }
                );

            CreateInitialManager(context);
        }

        private void CreateInitialManager(FileStorageDbContext context)
        {
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var manager = new ApplicationUser()
            {
                Email = "manager1@mail.com",
                UserName = "Manager1"
            };
            var password = "password1";

            var result = userManager.Create(manager, password);
            if (result.Succeeded)
            {
                var currentUser = userManager.FindByName(manager.UserName);

                userManager.AddToRole(currentUser.Id, "Manager");
                userManager.AddToRole(currentUser.Id, "User");
            }

            var directory = new Directory() { Name = "MyStorage", ApplicationUserId = manager.Id, ParentDirectoryId = null };
            context.Directories.Add(directory);
            context.SaveChanges();
        }
    }
}