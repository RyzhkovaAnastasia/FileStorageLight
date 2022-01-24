using DAL.Context;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Ninject.Modules;
using System.Data.Entity;
using DAL.IdentityManagers;

namespace DAL.Config
{
    public class DataAccessLayerDIConfig: NinjectModule
    {
        public override void Load()
        {
            Bind<IFileStorageDbContext>().To<FileStorageDbContext>();
            Bind<DbContext>().To<FileStorageDbContext>();

            Bind<IAuthUnitOfWork>().To<AuthUnitOfWork>();
            Bind<IUnitOfWork>().To<UnitOfWork>();
            Bind<IFileRepository>().To<FileRepository>();
            Bind<IDirectoryRepository>().To<DirectoryRepository>();

            Bind<IRoleStore<IdentityRole, string>>().To<RoleStore<IdentityRole, string, IdentityUserRole>>();
            Bind<IUserStore<ApplicationUser>>().To<UserStore<ApplicationUser>>();
            Bind<ApplicationUserManager>().ToSelf();
            Bind<ApplicationRoleManager>().ToSelf();
        }
    }
}
