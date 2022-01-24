using BLL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using BLL.Exceptions;

namespace FileStorage.Controllers
{
    /// <summary>
    /// RoleController responsive for user roles managing
    /// </summary>
    [Authorize(Roles = "Manager")]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        /// <summary>
        /// RoleController constructor
        /// </summary>
        /// <param name="roleService">Service instance for IRoleService</param>
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Roles representation for specific user
        /// </summary>
        /// <param name="userId">selected user id</param>
        /// <returns>View with available roles</returns>
        [HttpGet]
        public async Task<ActionResult> Edit(string userId)
        {
            var model = await _roleService.GetRolesAsync(userId);
            return View(model);
        }

        /// <summary>
        /// EditAsync roles for specific user 
        /// </summary>
        /// <param name="userId">selected user id</param>
        /// <param name="roles">list of selected roles</param>
        /// <returns>View with selected roles</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string userId, List<string> roles)
        {
            try
            {
                await _roleService.EditAsync(userId, roles);
                var model = await _roleService.GetRolesAsync(userId);
                ViewBag.Result = "Roles was successfully changed.";
                return View(model);
            }
            catch (UserException e)
            {
               ModelState.AddModelError(string.Empty, e.Message);
                var model = await _roleService.GetRolesAsync(userId);
                return View(model);
            }
        }
    }
}