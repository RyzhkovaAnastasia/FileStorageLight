using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IFileRepository: IRepository<File>
    {
        /// <summary>
        /// Get important files from collection
        /// </summary>
        /// <param name="userId">Owner user id</param>
        /// <returns>Important files collection</returns>
        ICollection<File> GetImportant(string userId);

        /// <summary>
        /// Get files that were shared for current user
        /// </summary>
        /// <param name="userId">Recipient user id</param>
        /// <returns>Files collection</returns>
        ICollection<File> GetAvailable(string userId);

        /// <summary>
        /// Get files by path of their name
        /// </summary>
        /// <param name="name">Part of file name</param>
        /// <returns>File collection</returns>
        ICollection<File> GetByName(string name);

        /// <summary>
        /// Create file short URN for access file by link
        /// </summary>
        /// <param name="fileShortLink">File link item</param>
        /// <returns>File link item</returns>
        Task<FileShortLink> CreateFileShortLinkAsync(FileShortLink fileShortLink);

        /// <summary>
        /// Create many files and add to collection
        /// </summary>
        /// <param name="items">Collection of files</param>
        Task CreateAsync(IEnumerable<File> items);

        /// <summary>
        /// Set recipients for shared file by manual including
        /// </summary>
        /// <param name="id">File id</param>
        /// <param name="users">Recipients of the file</param>
        Task ShareForRecipientsAsync(Guid id, ICollection<ApplicationUser> users);

        /// <summary>
        /// Set recipients of the shared file by link access
        /// </summary>
        /// <param name="recipient">Shared file item</param>
        /// <returns></returns>
        Task ShareByLinkAsync(SharedFile recipient);

        /// <summary>
        /// Get file by its short link URN
        /// </summary>
        /// <param name="shortUrl">Short URN</param>
        /// <returns></returns>
        File GetFileByShortLink(string shortUrl);

        Task<string> GetVirtualLocationPath(Guid id);

        Task<string> GetFullVirtualLocationPath(Guid id);
        Task<string> GetFileOwner(Guid id);
        ICollection<File> GetByNameTypeFilter(string name);
        ICollection<File> GetByNameDateFilter(string name);
    }
}
