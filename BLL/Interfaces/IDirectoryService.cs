using BLL.Models;
using System;
using System.Threading.Tasks;
using BLL.Exceptions;

namespace BLL.Interfaces
{
    public interface IDirectoryService
    {
        /// <summary>
        /// GetAsync specific directory 
        /// </summary>
        /// <param name="id">directory id</param>
        /// <returns>directory model instance</returns>
        /// <exception cref="NotFoundException">directory was not found</exception>
        Task<DirectoryModel> GetAsync(Guid id);

        /// <summary>
        /// CreateAsync new directory in storage
        /// </summary>
        /// <param name="directory">directory model instance</param>
        /// <param name="physicalPath">full server local path to current directory</param>
        /// <param name="userId">current user id</param>
        /// <exception cref="DirectoryException"></exception>
        Task CreateAsync(DirectoryModel directory, string physicalPath, string userId);

        /// <summary>
        /// UpdateAsync directory model
        /// </summary>
        /// <param name="directory">new directory instance</param>
        /// <param name="physicalPath">physical server path to user directory</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="DirectoryException"></exception>
        Task UpdateAsync(DirectoryModel directory, string physicalPath);

        /// <summary>
        /// DeleteAsync directory with items inside
        /// </summary>
        /// <param name="id">directory id</param>
        /// <param name="physicalPath">physical server path to user directory</param>
        /// <exception cref="DirectoryException"></exception>
        /// <exception cref="NotFoundException"></exception>
        Task DeleteAsync(Guid id, string physicalPath);
    }
}
