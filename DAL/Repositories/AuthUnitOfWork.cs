using DAL.Interfaces;
using System;
using DAL.Context;
using DAL.Entities;
using DAL.IdentityManagers;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DAL.Repositories
{
    public sealed class AuthUnitOfWork : IAuthUnitOfWork
    {
        private bool _disposed;
        private readonly IFileStorageDbContext _context;
        public ApplicationUserManager UserManager { get; }
        public ApplicationRoleManager RoleManager { get; }
        public AuthUnitOfWork(IFileStorageDbContext context)
        {
            _context = context;
            UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>((FileStorageDbContext)_context));
            RoleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>((FileStorageDbContext)_context));
            _disposed = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (!_disposed)
            {
                if (isDisposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
        ~AuthUnitOfWork()
        {
            Dispose(false);
        }
    }
}
