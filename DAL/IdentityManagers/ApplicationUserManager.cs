using DAL.Entities;
using Microsoft.AspNet.Identity;

namespace DAL.IdentityManagers
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store): base(store)
        {
        }
    }
}
