using System;
using BLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.Exceptions;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="registerModel">register user model</param>
        /// <param name="isSignIn">true - sing in new user, false - not sing in</param>
        Task RegisterAsync(RegisterModel registerModel, bool isSignIn);
        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="authModel">login user model</param>
        Task LoginAsync(AuthModel authModel);
        /// <summary>
        /// Logout user
        /// </summary>
        void Logout();

        /// <summary>
        /// Get all users  except current in User role or without role
        /// </summary>
        /// <param name="userId">current user id</param>
        /// <returns>list of users ang guests except current user</returns>
        IEnumerable<UserModel> GetAllUsers(string userId);
        /// <summary>
        /// Get users by theirs id
        /// </summary>
        /// <param name="id">users id</param>
        /// <returns>selected users list</returns>
        IEnumerable<UserModel> GetUsersByIds(IEnumerable<string> id);
        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns>edit user model instance</returns>
        /// <exception cref="NotFoundException"></exception>
        Task<EditUserModel> FindUserByIdAsync(string userId);
        /// <summary>
        /// Edit user
        /// </summary>
        /// <param name="editedUser">edit user model instance</param>
        /// <exception cref="UserException"></exception>
        Task EditUserAsync(EditUserModel editedUser);
        /// <summary>
        /// Block user
        /// </summary>
        /// <param name="userId">user id</param>
        Task DeleteUserAsync(string userId);

        IEnumerable<UserModel> SearchByName(string name);

    }
}
