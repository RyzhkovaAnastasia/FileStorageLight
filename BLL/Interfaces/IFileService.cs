using BLL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using BLL.Exceptions;

namespace BLL.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// Get specific file
        /// </summary>
        /// <param name="id">file id</param>
        /// <returns>file model instance</returns>
        Task<FileModel> GetAsync(Guid id);

        /// <summary>
        /// Get all files from all users
        /// </summary>
        /// <returns>files list</returns>
        Task<IEnumerable<FileModel>> GetAll();
        /// <summary>
        /// Get important files by specific user
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns>important file list</returns>
        IEnumerable<FileModel> GetImportant(string userId);
        /// <summary>
        /// Get files that were shared to specific user
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns>available files list</returns>
        IEnumerable<FileModel> GetAvailable(string userId);
        /// <summary>
        /// Get files by name
        /// </summary>
        /// <param name="name">part of file name</param>
        /// <returns>file list</returns>
        IEnumerable<FileModel> GetByName(string name, bool isDateSort, bool isTypeSort);
        /// <summary>
        /// GetAsync file by short query URN
        /// </summary>
        /// <param name="shortQuery">short query from URN</param>
        /// <returns>file model instance</returns>
        /// <exception cref="NotFoundException"></exception>
        FileModel GetFileByShortLink(string shortQuery);

        /// <summary>
        /// Upload files to storage
        /// </summary>
        /// <param name="files">files to uploading</param>
        /// <param name="physicalPath">full server local path to directory</param>
        /// <param name="parentDir">directory </param>
        /// <returns>list of uploaded files</returns>
        /// <exception cref="FileException"></exception>
        Task<IEnumerable<FileModel>> CreateAsync(HttpPostedFileBase[] files, string physicalPath, Guid parentDirId);
        /// <summary>
        /// CreateAsync short URN for file
        /// </summary>
        /// <param name="id">file id</param>
        /// <returns>short URN query</returns>
        Task<string> CreateLinkAsync(Guid id);
        /// <summary>
        /// Edit specific file
        /// </summary>
        /// <param name="file">file model instance</param>
        /// <param name="physicalPath">physical server local path to user directory</param>
        Task UpdateAsync(FileModel file, string physicalPath);
        /// <summary>
        /// Delete specific file
        /// </summary>
        /// <param name="id">file id</param>
        /// <param name="physicalPath">physical server local path to user directory</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="FileException"></exception>
        Task DeleteAsync(Guid id, string physicalPath);
        /// <summary>
        /// Share file for specific users
        /// </summary>
        /// <param name="id">file id</param>
        /// <param name="recipients">selected users for file sharing</param>
        /// <param name="allUsers">all users</param>
        /// <returns>file recipients</returns>
        Task<Recipients> ShareForRecipientsAsync(Guid id, IEnumerable<UserModel> recipients, IEnumerable<UserModel> allUsers);
        /// <summary>
        /// Set shared file to available list
        /// </summary>
        /// <param name="id">file id</param>
        /// <param name="userId">user id</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="ForbiddenException">cannot set file to available by owner</exception>
        Task ShareByLinkAsync(Guid id, string userId);

        /// <summary>
        /// Get bytes of the file by its id and physical path
        /// </summary>
        /// <param name="id">file id</param>
        /// <param name="physicalPath"></param>
        /// <returns>bytes array of the file</returns>
        Task<byte[]> DownloadFile(Guid id, string physicalPath);
    }
}
