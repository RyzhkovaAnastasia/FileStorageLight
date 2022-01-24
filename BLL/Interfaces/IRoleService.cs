using BLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.Exceptions;

namespace BLL.Interfaces
{
    public interface IRoleService
    {
        /// <summary>
        /// GetAsync specific user roles
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns>role model instance</returns>
        /// <exception cref="NotFoundException"></exception>
        Task<RoleModel> GetRolesAsync(string userId);
        /// <summary>
        /// EditAsync roles for specific user
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="roles">list of roles</param>
        /// <exception cref="UserException"></exception>
        /// <exception cref="NotFoundException"></exception>
        Task EditAsync(string userId, List<string> roles);
    }
}
