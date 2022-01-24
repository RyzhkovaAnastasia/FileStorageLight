using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class
    {
        /// <summary>
        /// Get all items from collection
        /// </summary>
        /// <returns>Collections of items</returns>
        Task<ICollection<T>> GetAll();

        /// <summary>
        /// Get all items by users id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Collection of items</returns>
        ICollection<T> GetAllByUserId(string userId);

        /// <summary>
        /// Get item
        /// </summary>
        /// <param name="id">Item id</param>
        /// <returns>Item</returns>
        Task<T> GetAsync(Guid id);

        /// <summary>
        /// Create and add item to collection
        /// </summary>
        /// <param name="item">New item</param>
        Task CreateAsync(T item);

        /// <summary>
        /// Update item values
        /// </summary>
        /// <param name="item">Changed item with the same id</param>
        Task UpdateAsync(T item);

        /// <summary>
        /// Delete item from collection
        /// </summary>
        /// <param name="id">Item id</param>
        Task DeleteAsync(Guid id);
    }
}
