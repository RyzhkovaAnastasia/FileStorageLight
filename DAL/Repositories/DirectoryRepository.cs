using DAL.Entities;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    /// <summary>
    /// DirectoryRepository responsive for managing directories in DB
    /// </summary>
    public class DirectoryRepository : IDirectoryRepository
    {
        private readonly IFileStorageDbContext _context;

        /// <summary>
        /// Constructor DirectoryRepository
        /// </summary>
        /// <param name="context">Context instance</param>
        public DirectoryRepository(IFileStorageDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task CreateAsync(Directory item)
        {
            if (item == null)
            {
                return;
            }
            _context.Directories.Add(item);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// DeleteAsync directory with connected directories and files
        /// </summary>
        /// <param name="id">directory id</param>
        public async Task DeleteAsync(Guid id)
        {

            var folder = await GetAsync(id);
            if (folder != null)
            {
                DeleteChild(new List<Directory>() { folder });
                await _context.SaveChangesAsync();
            }
        }

        void DeleteChild(List<Directory> folders)
        {
            if (folders.Count == 0)
            {
                return;
            }
            else
            {

                foreach (var item in folders)
                {
                    DeleteChild(_context.Directories.Where(f => f.ParentDirectoryId == item.Id).ToList());
                    _context.Directories.Remove(item);
                }
            } 
        }

        public async Task<string> GetVirtualLocationPath(Guid id)
        {
            var directory = await _context.Directories.FindAsync(id);
            if (directory == null)
            {
                return null;
            }

            directory = directory.ParentDirectory;
            var path = new StringBuilder();
            while (directory != null && !string.IsNullOrEmpty(directory.ParentDirectoryId.ToString()))
            {

                path.Insert(0, directory.Name);
                path.Insert(0, "\\");
                directory = directory.ParentDirectory;
            }

            return path.ToString().Trim('\\');
        }

        public async Task<string> GetFullVirtualLocationPath(Guid id)
        {
            await _context.Directories.LoadAsync();
            var directory = _context.Directories.Local.FirstOrDefault(f=>f.Id == id);
            if (directory == null)
            {
                return null;
            }

            var path = new StringBuilder();
            while (directory != null && !string.IsNullOrEmpty(directory.ParentDirectoryId.ToString()))
            {
                path.Insert(0, directory.Name);
                path.Insert(0, "\\");
                directory = directory.ParentDirectory;
            }
            return path.ToString().Trim('\\');
        }

        public async Task<Directory> GetDefaultDirAsync(string userId)
        {
            await _context.Directories.LoadAsync();
            var defaultDirectory = _context.Directories.Local.FirstOrDefault(d => d.ParentDirectoryId == null && d.ApplicationUserId == userId);
            return defaultDirectory;
        }

        public ICollection<Directory> GetChildDirectories(Guid id)
        {
            var childDirectories = _context.Directories.Where(d => d.ParentDirectoryId == id).ToList();
            childDirectories.ForEach(d => d.Location = GetVirtualLocationPath(d.Id).Result);
            return childDirectories;
        }

        /// <inheritdoc />
        public async Task<Directory> GetAsync(Guid id)
        {
            await _context.Directories.LoadAsync();
            var folder = _context.Directories.Local.FirstOrDefault(x => x.Id == id);
            if (folder != null)
            {
                folder.Location = await GetVirtualLocationPath(folder.Id);
            }

            return folder;
        }

        /// <inheritdoc />
        public async Task<ICollection<Directory>> GetAll()
        {
            await _context.Directories.LoadAsync();
            var directories = _context.Directories;
            await directories.ForEachAsync(d => d.Location = GetVirtualLocationPath(d.Id).Result);
            return _context.Directories.Local.ToList();
        }

        /// <inheritdoc />
        public ICollection<Directory> GetAllByUserId(string userId)
        {
            _context.Directories.Load();
            var userDirectories = _context.Directories.Local.Where(dir => dir.ApplicationUser.Id == userId).ToList();
            userDirectories.ForEach(d => d.Location = GetVirtualLocationPath(d.Id).Result);
            return userDirectories;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Directory item)
        {
            if (item == null)
            {
                return;
            }

            await _context.Directories.LoadAsync();
            var dir = _context.Directories.Local.FirstOrDefault(f => f.Id == item.Id);

            if (dir == null)
            {
                return;
            }

            dir.Name = item.Name;
            dir.Description = item.Description;

            
            await _context.SaveChangesAsync();
        }


        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }



        ~DirectoryRepository()
        {
            Dispose(false);
        }
    }
}
