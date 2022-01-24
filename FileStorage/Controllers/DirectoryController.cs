using BLL.Interfaces;
using BLL.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using BLL.Exceptions;

namespace FileStorage.Controllers
{
    /// <summary>
    /// DirectoryController responsible for directory managing
    /// </summary>
    [Authorize(Roles = "User")]
    public class DirectoryController : BaseStorageController
    {
        private readonly IDirectoryService _directoryService;
        /// <summary>
        /// DirectoryController constructor
        /// </summary>
        /// <param name="directoryService">Service instance for IDirectoryService</param>
        public DirectoryController(IDirectoryService directoryService)
        {
            _directoryService = directoryService;
        }
        /// <summary>
        /// Get view for creating directory
        /// </summary>
        /// <returns>View for directory creation</returns>
        [HttpGet]
        public ActionResult Create(Guid parentDirId)
        {
            var newDirectory = new DirectoryModel() { ParentDirectoryId = parentDirId };
            return View(newDirectory);
        }

        /// <summary>
        /// Create directory
        /// </summary>
        /// <param name="directory">Directory model for creating</param>
        /// <returns>View for directory creation with current values</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DirectoryModel directory)
        {
            try
            {
                await _directoryService.CreateAsync(directory, PhysicalDirectory, UserId);
                ViewBag.Message = "Directory was created successfully";
                ViewBag.Location = directory.Location;
                return View();
            }
            catch (DirectoryException e)
            {
               ModelState.AddModelError(string.Empty, e.Message);
                return View();
            }
        }

        /// <summary>
        /// Get view for editing directory values
        /// </summary>
        /// <param name="id">id for current directory</param>
        /// <returns>View for directory editing with current values</returns>
        [HttpGet]
        public async Task<ActionResult> Edit(Guid id)
        {
            var dir = await _directoryService.GetAsync(id);
            return View(dir);
        }

        /// <summary>
        /// Edit directory values
        /// </summary>
        /// <param name="directory">new directory model</param>
        /// <returns>View for directory editing with new values</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DirectoryModel directory)
        {
            try
            {
                await _directoryService.UpdateAsync(directory, PhysicalDirectory);
                ViewBag.Message = "Directory was edited successfully";
                return View();
            }
            catch (DirectoryException e)
            {
               ModelState.AddModelError(string.Empty, e.Message);
                return View();
            }
        }

        /// <summary>
        /// Directory information with current values 
        /// </summary>
        /// <param name="id">id for current directory</param>
        /// <returns>View for directory with current values </returns>
        [HttpGet]
        public async Task<ActionResult> Details(Guid id)
        {
            var dir = await _directoryService.GetAsync(id);
            return View(dir);
        }
        /// <summary>
        /// Delete directory
        /// </summary>
        /// <param name="id">id for current directory</param>
        /// <returns>Previous view</returns>
        [HttpPost]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _directoryService.DeleteAsync(id, PhysicalDirectory);
            }
            catch (DirectoryException e)
            {
               ModelState.AddModelError(string.Empty, e.Message);
            }
            return Redirect(Request.UrlReferrer?.ToString());
        }
    }
}