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
    /// FileRepository responsive for managing files in DB
    /// </summary>
    public class FileRepository : IFileRepository
    {
        private readonly IFileStorageDbContext _context;

        /// <summary>
        /// FileRepository constructor
        /// </summary>
        /// <param name="context">Db context instance</param>

        public FileRepository(IFileStorageDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task CreateAsync(File item)
        {
            if (item == null)
            {
                return;
            }

            _context.Files.Add(item);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task CreateAsync(IEnumerable<File> items)
        {
            if (items == null)
            {
                return;
            }

            _context.Files.AddRange(items);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file != null)
            {
                _context.Files.Remove(file);
                await _context.SaveChangesAsync();
            }
        }

        /// <inheritdoc />
        public async Task<File> GetAsync(Guid id)
        {
            await _context.Files.LoadAsync();
            var file = _context.Files.Local.FirstOrDefault(x => x.Id == id);
            if (file != null)
            {
                file.Location = await GetVirtualLocationPath(file.Id);
            }
            return file;
        }

        /// <inheritdoc />
        public async Task<ICollection<File>> GetAll()
        {
            await _context.Files.LoadAsync();
            var files = _context.Files.Local.ToList();

            foreach (var file in files)
            {
                file.Location = await GetVirtualLocationPath(file.Id);
            }
            return files;
        }

        /// <inheritdoc />
        public ICollection<File> GetAllByUserId(string userId)
        {
            _context.Files.Load();
            var userFiles = _context.Files.Local.Where(file => file.Directory.ApplicationUserId == userId).ToList();
            userFiles.ForEach(f => f.Location = GetVirtualLocationPath(f.Id).Result);
            return userFiles;
        }

        /// <inheritdoc />
        public ICollection<File> GetImportant(string userId)
        {
            _context.Files.Load();
            var importantFiles = _context.Files.Local
                .Where(f => f.IsImportant && f.Directory.ApplicationUserId == userId).ToList();
            importantFiles.ForEach(f => f.Location = GetVirtualLocationPath(f.Id).Result);
            return importantFiles;
        }

        /// <inheritdoc />
        public async Task ShareForRecipientsAsync(Guid id, ICollection<ApplicationUser> users)
        {
            var items = users.Select((applicationUser, iterator) =>
                new SharedFile()
                {
                    ApplicationUserId = users.ToArray()[iterator].Id,
                    FileId = id
                }).ToList();

            var sharedFilesByFileId = _context.SharedFiles.Where(s => s.FileId == id);
            _context.SharedFiles.RemoveRange(sharedFilesByFileId);
            _context.SharedFiles.AddRange(items);

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public ICollection<File> GetByName(string name)
        {
            _context.Files.Load();
            return _context.Files.Local.Where(f => f.Name.ToLower().Contains(name.ToLower())).ToList();
        }

        public ICollection<File> GetByNameTypeFilter(string name)
        {
            _context.Files.Load();
            return _context.Files.Local.Where(f => f.Name.ToLower().Contains(name.ToLower())).OrderBy(x=>x.Type).ToList();
        }

        public ICollection<File> GetByNameDateFilter(string name)
        {
            _context.Files.Load();
            return _context.Files.Local.Where(f => f.Name.ToLower().Contains(name.ToLower())).OrderBy(x=>x.UploadDate).ToList();
        }

        /// <inheritdoc />
        public async Task UpdateAsync(File item)
        {
            var file = await GetAsync(item.Id);

            if (file == null)
            {
                return;
            }
            file.Name = item.Name;
            file.Description = item.Description;
            file.IsImportant = item.IsImportant;
            
           
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public ICollection<File> GetAvailable(string userId)
        {
            _context.Files.Load();
            _context.SharedFiles.Load();
            var sharedFilesId = _context.SharedFiles.Local
                .Where(sf => sf.ApplicationUserId == userId)
                .Select(x => x.FileId)
                .ToList();
            var files = _context.Files.Local.Where(f => sharedFilesId.Contains(f.Id)).ToList();
            return files;
        }

        /// <inheritdoc />
        public async Task<FileShortLink> CreateFileShortLinkAsync(FileShortLink fileShortLink)
        {
            var fileLink = await _context.FileShortLinks.FindAsync(fileShortLink.Id);
            if (fileLink == null)
            {
                _context.FileShortLinks.Add(fileShortLink);
                await _context.SaveChangesAsync();
                return fileShortLink;
            }

            return fileLink;
        }

        /// <inheritdoc />
        public async Task ShareByLinkAsync(SharedFile recipient)
        {
            var isGuestExist = _context.SharedFiles
            .FirstOrDefault(r =>
                r.ApplicationUserId == recipient.ApplicationUserId &&
                r.FileId == recipient.FileId) != null;

            if (isGuestExist)
            {
                return;
            }

            _context.SharedFiles.Add(recipient);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public File GetFileByShortLink(string shortUrl)
        {
            _context.FileShortLinks.Load();
            return _context.FileShortLinks.Local.FirstOrDefault(f => f.ShortLink == shortUrl)?.File;
        }

        public async Task<string> GetFullVirtualLocationPath(Guid id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file == null)
            {
                return null;
            }
            var directory = await _context.Directories.FindAsync(file.DirectoryId);

            var path = new StringBuilder();
            while (directory != null && !string.IsNullOrEmpty(directory.ParentDirectoryId.ToString()))
            {

                path.Insert(0, directory.Name);
                path.Insert(0, '\\');
                directory = directory.ParentDirectory;
            }

            path.Append("\\");
            path.Append(file.Name + file.Type);
            return path.ToString().Trim('\\');
        }

        public async Task<string> GetVirtualLocationPath(Guid id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file == null)
            {
                return null;
            }

            var directory = await _context.Directories.FindAsync(file.DirectoryId);
            if (directory == null)
            {
                return null;
            }

            var path = new StringBuilder();
            while (directory != null && !string.IsNullOrEmpty(directory.ParentDirectoryId.ToString()))
            {

                path.Insert(0, directory.Name);
                path.Insert(0, '\\');
                directory = directory.ParentDirectory;
            }

            return path.ToString().Trim('\\');
        }

        public async Task<string> GetFileOwner(Guid id)
        {
            var file = await _context.Files.FindAsync(id);
            return file?.Directory.ApplicationUserId;
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

        ~FileRepository()
        {
            Dispose(false);
        }
    }
}