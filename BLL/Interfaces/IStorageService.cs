using System;
using BLL.Models;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IStorageService
    {
        /// <summary>
        /// GetAsync all items from specific path
        /// </summary>
        /// <param name="itemId">directory id, that was opened</param>
        /// <param name="userId">current user id</param>
        /// <returns>list of files and directories into current directory path</returns>
        Task<StorageItemsModel> GetItemsByUserId(Guid itemId, string userId);
    }
}
