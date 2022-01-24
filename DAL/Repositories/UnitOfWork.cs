using DAL.Interfaces;
using System;

namespace DAL.Repositories
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        private readonly IFileStorageDbContext _context;
        public IFileRepository FileRepository { get; }
        public IDirectoryRepository DirectoryRepository { get; }
        public UnitOfWork(IFileStorageDbContext context)
        {
            _context = context;
            FileRepository = new FileRepository(_context);
            DirectoryRepository = new DirectoryRepository(_context);
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
        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
