using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IDirectoryRepository: IRepository<Directory>
    {
        Task<string> GetVirtualLocationPath(Guid id);
        Task<string> GetFullVirtualLocationPath(Guid id);
        Task<Directory> GetDefaultDirAsync(string userId);
        ICollection<Directory> GetChildDirectories(Guid id);
    }
}
