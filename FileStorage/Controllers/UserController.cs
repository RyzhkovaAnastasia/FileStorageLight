using System;
using System.Threading.Tasks;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Models;
using System.Web.Mvc;
using System.Collections.Generic;

namespace FileStorage.Controllers
{
    /// <summary>
    /// UserController responsive for user managing
    /// </summary>
    public class UserController : BaseStorageController
    {
        private readonly IUserService _userService;

        /// <summary>
        /// UserController constructor
        /// </summary>
        /// <param name="userService">Service instance for IUserService</param>
        public UserController(IUserService userService)
        {
            _userService = userService;

        }
        /// <summary>
        /// RegisterAsync representation for new user
        /// </summary>
        /// <returns>View of registration form</returns>
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// RegisterAsync new user
        /// </summary>
        /// <param name="registerModel">user register model</param>
        /// <returns>Storage view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel registerModel)
        {
            try
            {
                await _userService.RegisterAsync(registerModel, true);
            }
            catch (UserException e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(registerModel);
            }
            return RedirectToAction("Index", "Storage", new { });
        }

        /// <summary>
        /// LoginAsync representation for user
        /// </summary>
        /// <returns>View for login</returns>
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// LoginAsync user
        /// </summary>
        /// <param name="authModel">login user model</param>
        /// <returns>Storage view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(AuthModel authModel)
        {
            try
            {
                await _userService.LoginAsync(authModel);
            }
            catch (NotFoundException e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View();
            }
            catch (ForbiddenException e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View();
            }
            return RedirectToAction("Index", "Storage", new { });

        }

        /// <summary>
        /// Logout user account
        /// </summary>
        /// <returns>LoginAsync view</returns>
        [HttpGet]
        [Authorize]
        public ActionResult Logout()
        {
            _userService.Logout();

                Response.Cookies.Clear();
            Session.Clear();

            return RedirectToAction("Login");
        }

        /// <summary>
        /// Representation of user list except current
        /// </summary>
        /// <returns>View of users except current</returns>
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public ActionResult Users()
        {
            var users = _userService.GetAllUsers(UserId);
            return View(users);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public ActionResult Users(string name)
        {
            IEnumerable<UserModel> users;
            if (string.IsNullOrEmpty(name))
            {
                users = _userService.GetAllUsers(UserId);
            }
            users = _userService.SearchByName(name);
            return View(users);
        }


        /// <summary>
        /// Representation for user registration
        /// </summary>
        /// <returns>View for register new user</returns>
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public ActionResult Create()
        {
            return View(new RegisterModel());
        }

        /// <summary>
        /// RegisterAsync new user
        /// </summary>
        /// <param name="newUser">register user model</param>
        /// <returns>View with current user values</returns>
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterModel newUser)
        {
            try
            {
                await _userService.RegisterAsync(newUser, false);
                ViewBag.Result = "User was successfully created";
            }
            catch (UserException e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View(newUser);
        }

        /// <summary>
        /// Representation for user editing
        /// </summary>
        /// <param name="userId">selected user id</param>
        /// <returns>View for editing</returns>
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> Edit(string userId)
        {
            var user = await _userService.FindUserByIdAsync(userId);
            return View(user);
        }

        /// <summary>
        /// User editing
        /// </summary>
        /// <param name="editedUser">user model</param>
        /// <returns>View with current user values</returns>
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserModel editedUser)
        {
            try
            {
                await _userService.EditUserAsync(editedUser);
                ViewBag.Result = "User was successfully edited";
            }
            catch (UserException e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View(editedUser);
        }

        /// <summary>
        /// Block current user forever
        /// </summary>
        /// <param name="userId">selected user id</param>
        /// <returns>Previous view</returns>
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> Delete(string userId)
        {
            await _userService.DeleteUserAsync(userId);
            return Redirect(Request.UrlReferrer?.ToString());
        }


    
    }
}