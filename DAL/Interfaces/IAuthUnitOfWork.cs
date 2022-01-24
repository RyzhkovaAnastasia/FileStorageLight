using System;
using DAL.IdentityManagers;

namespace DAL.Interfaces
{
    public interface IAuthUnitOfWork: IDisposable
    {
        ApplicationUserManager UserManager { get; }
        ApplicationRoleManager RoleManager { get; }
    }
}
