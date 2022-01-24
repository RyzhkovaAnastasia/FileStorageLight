using System;
using DAL.Context;
using DAL.Entities;
using DAL.IdentityManagers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace BLL.Config
{
    public class BusinessLogicLayerAppBuilderConfiguration
    {
        public static void Configure(IAppBuilder app)
        {
            app.CreatePerOwinContext(() =>
                new ApplicationUserManager(new UserStore<ApplicationUser>(new FileStorageDbContext())));

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                SlidingExpiration = false, 
                LoginPath = new PathString("/User/Login"),
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        TimeSpan.Zero,
                        (manager, user) => manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie))
                }
            });
        }
    }
}
