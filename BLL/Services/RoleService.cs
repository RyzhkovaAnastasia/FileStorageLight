using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Models;
using DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    /// <summary>
    /// RoleService responsive for role managing
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly IAuthUnitOfWork _authUnitOfWork;

        /// <summary>
        /// RoleService constructor
        /// </summary>
        /// <param name="authUnitOfWork">IAuthUnitOfWork instance</param>
        public RoleService(IAuthUnitOfWork authUnitOfWork)
        {
            _authUnitOfWork = authUnitOfWork;
        }

        /// <inheritdoc />
        public async Task EditAsync(string userId, List<string> roles)
        {
            if(roles == null)
            {
                roles = new List<string>();
            }

            var user = await _authUnitOfWork.UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _authUnitOfWork.UserManager.GetRolesAsync(user.Id);

                var addedRoles = roles.Except(userRoles).ToArray();
                var removedRoles = userRoles.Except(roles).ToArray();

                var addRoleResult = await _authUnitOfWork.UserManager.AddToRolesAsync(userId, addedRoles);
                var removeRoleResult = await _authUnitOfWork.UserManager.RemoveFromRolesAsync(userId, removedRoles);

                if (!(addRoleResult.Succeeded && removeRoleResult.Succeeded))
                {
                    throw new UserException("Cannot change roles.");
                }
            }
            else
            {
                throw new NotFoundException();
            }
        }
        /// <inheritdoc />
        public async Task<RoleModel> GetRolesAsync(string userId)
        {
            var user = await _authUnitOfWork.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException();
            }
            var userRoles = await _authUnitOfWork.UserManager.GetRolesAsync(userId);
            var allRoles = _authUnitOfWork.RoleManager.Roles?.ToList();

            var roleModel = new RoleModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserRoles = userRoles,
                AllRoles = allRoles
            };

            return roleModel;
        }
    }
}
