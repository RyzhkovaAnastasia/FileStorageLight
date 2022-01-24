using DAL.Entities;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IFileStorageDbContext : IDisposable
    {
        DbSet<File> Files { get; }
        DbSet<Directory> Directories { get; }
        DbSet<SharedFile> SharedFiles { get; }
        DbSet<FileShortLink> FileShortLinks { get; }
        Task<int> SaveChangesAsync();
    }
}
