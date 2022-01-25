using System;
using BLL.Interfaces;
using BLL.Models;
using Microsoft.Owin.Security;
using BLL.Exceptions;
using Microsoft.AspNet.Identity;
using AutoMapper;
using DAL.Entities;
using DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    /// <summary>
    /// UserService responsive for user managing
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IDirectoryRepository _directoryRepository;

        /// <summary>
        /// UserService constructor
        /// </summary>
        /// <param name="authUnitOfWork">IAuthUnitOfWork instance</param>
        /// <param name="mapper">IMapper instance</param>
        /// <param name="authenticationManager">IAuthenticationManager instance</param>
        public UserService(IAuthUnitOfWork authUnitOfWork, IMapper mapper, IAuthenticationManager authenticationManager, IDirectoryRepository directoryRepository)
        {
            _authUnitOfWork = authUnitOfWork;
            _mapper = mapper;
            _authenticationManager = authenticationManager;
            _directoryRepository = directoryRepository;
        }

        /// <inheritdoc />
        public async Task LoginAsync(AuthModel authModel)
        {
            var user = await _authUnitOfWork.UserManager.FindAsync(authModel.UserName, authModel.Password);
            if (user == null)
            {
                throw new NotFoundException("Invalid credentials. Please, try again.");
            }

            if (user.LockoutEnabled)
            {
                throw new ForbiddenException("User was blocked by manager.");
            }

            await SignIn(user);
        }

        /// <inheritdoc />
        public async Task RegisterAsync(RegisterModel registerModel, bool isSignIn)
        {
            var user = _mapper.Map<RegisterModel, ApplicationUser>(registerModel);

            var isEmailExist = await _authUnitOfWork.UserManager.FindByEmailAsync(registerModel.Email) != null;
            if (isEmailExist)
            {
                throw new UserException("Email is already taken");
            }
            var isNameExist = _authUnitOfWork.UserManager.Users.Where(u=>u.UserName == registerModel.Username).Count() > 0;
            if (isNameExist)
            {
                throw new UserException("Username is already taken");
            }

            var creationResult = await _authUnitOfWork.UserManager.CreateAsync(user, registerModel.Password);
            var directory = new Directory() {Name = "MyStorage", ApplicationUserId = user.Id};
            await _directoryRepository.CreateAsync(directory);

            if (creationResult != IdentityResult.Success)
            {
                var errorsList = string.Join("\n", creationResult.Errors);
                throw new UserException(errorsList);
            }

            await _authUnitOfWork.UserManager.AddToRoleAsync(user.Id, "User");

            if (isSignIn)
            {
                await SignIn(user);
            }
        }

        /// <inheritdoc />
        public void Logout()
        {
            _authenticationManager.SignOut();
        }

        /// <inheritdoc />
        public IEnumerable<UserModel> GetAllUsers(string userId)
        {
            var allAccounts = _authUnitOfWork.UserManager.Users?.ToList();
            var roles = _authUnitOfWork.RoleManager.Roles?.ToList();

            var users = allAccounts?
                .Where(user => (user.Roles.Count == 0 || user.Roles.Any(role => role.RoleId == roles.FirstOrDefault(r => r.Name != "Manager")?.Id))
                && !user.LockoutEnabled 
                && user.Id != userId).ToList();

            return _mapper.Map<List<UserModel>>(users);
        }
        /// <inheritdoc />
        public async Task<EditUserModel> FindUserByIdAsync(string userId)
        {
            var user = await _authUnitOfWork.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User was not found");
            }
            return _mapper.Map<EditUserModel>(user);
        }

        /// <inheritdoc />
        public async Task EditUserAsync(EditUserModel editedUser)
        {
            var applicationUser = await _authUnitOfWork.UserManager.FindByIdAsync(editedUser.Id);
            if (applicationUser == null)
            {
                throw new NotFoundException();
            }
            try
            {
                applicationUser.UserName = editedUser.Username;
                applicationUser.Email = editedUser.Email;
                applicationUser.PasswordHash = null;

                await _authUnitOfWork.UserManager.UpdateAsync(applicationUser);
                await _authUnitOfWork.UserManager.AddPasswordAsync(applicationUser.Id, editedUser.Password);
            }
            catch
            {
                throw new UserException("Cannot edit user");
            }
        }

        /// <inheritdoc />
        public async Task DeleteUserAsync(string userId)
        {
            var user = await _authUnitOfWork.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException();
            }
            var result = await _authUnitOfWork.UserManager.UpdateAsync(user);
            await _authUnitOfWork.UserManager.UpdateSecurityStampAsync(userId);
            await _authUnitOfWork.UserManager.SetLockoutEnabledAsync(user.Id, true);
            if (!result.Succeeded)
            {
                var errorsList = string.Empty;
                if (!result.Succeeded)
                {
                    errorsList += string.Join("\n", result.Errors);
                }
                throw new UserException(errorsList);
            }


        }

        /// <inheritdoc />
        public IEnumerable<UserModel> GetUsersByIds(IEnumerable<string> id)
        {
            if (id == null)
            {
                return new List<UserModel>();
            }
            var selectedUsers = _authUnitOfWork.UserManager.Users?.Where(u => id.Contains(u.Id));
            return _mapper.Map<IEnumerable<UserModel>>(selectedUsers);
        }

        private async Task SignIn(ApplicationUser user)
        {
            var claims = await _authUnitOfWork.UserManager.CreateIdentityAsync(
                user, DefaultAuthenticationTypes.ApplicationCookie);

            var authProperties = new AuthenticationProperties()
            {
                IsPersistent = true

            };

            _authenticationManager.SignIn(authProperties, claims);
            
        }

        public IEnumerable<UserModel> SearchByName(string name)
        {
            var users = _authUnitOfWork.UserManager.Users.Where(f => f.UserName.ToLower().Contains(name.ToLower())).ToList();
            return _mapper.Map<List<UserModel>>(users);
        }
    }
}
