using BLL.Interfaces;
using BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BLL.Exceptions;

namespace FileStorage.Controllers
{
    /// <summary>
    /// FileController responsible for file managing
    /// </summary>
    [Authorize]
    public class FileController : BaseStorageController
    {
        private readonly IFileService _fileService;
        private readonly IUserService _userService;

        /// <summary>
        /// FileController constructor 
        /// </summary>
        /// <param name="fileService">Service instance for IFileService</param>
        /// <param name="userService">Service instance for IUserService</param>
        public FileController(IFileService fileService, IUserService userService)
        {
            _fileService = fileService;
            _userService = userService;
        }
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult Upload(Guid parentDirId)
        {
            ViewBag.DirectoryId = parentDirId;
            return View();
        }

        /// <summary>
        /// Upload files to file server local storage into directory path
        /// </summary>
        /// <param name="files">Uploaded files</param>
        /// <param name="parentDirId">directory id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> Upload(HttpPostedFileBase[] files, Guid parentDirId)
        {
            ViewBag.DirectoryId = parentDirId;
            try
            {
                var filesList = await _fileService.CreateAsync(files, PhysicalDirectory, parentDirId);
                ViewBag.Message = "File was uploaded successfully";
                return View(filesList);
            }
            catch (FileException e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View();
            }
        }

        /// <summary>
        /// Get file from server local storage
        /// </summary>
        /// <param name="id">file id</param>
        /// <returns>File from local storage</returns>
        [HttpGet]
        public async Task<ActionResult> Download(Guid id)
        {

            try
            {
                var file = await _fileService.GetAsync(id);
                var path = Server.MapPath("~/Files");
                var fileBytes = await _fileService.DownloadFile(id, path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name + file.Type);
            }
            catch
            {
                ViewBag.Error = "Cannot download file";
                return Redirect(Request.UrlReferrer?.ToString());
            }
        }

        /// <summary>
        /// Get information about specific file
        /// </summary>
        /// <param name="id">file id</param>
        /// <returns>View with file values</returns>
        [HttpGet]
        public async Task<ActionResult> Details(Guid id)
        {
            var file = await _fileService.GetAsync(id);
            return View(file);
        }

        /// <summary>
        /// Delete specific file from storage 
        /// </summary>
        /// <param name="id">file id</param>
        /// <returns>Previous view</returns>
        [HttpPost]
        [Authorize(Roles = "User, Manager")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _fileService.DeleteAsync(id, PhysicalDirectory);
                return Redirect(Request.UrlReferrer?.ToString());
            }
            catch (FileException e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return Redirect(Request.UrlReferrer?.ToString());
            }
        }

        /// <summary>
        /// Edit specific file
        /// </summary>
        /// <param name="id">file id</param>
        /// <returns>View with current file values</returns>
        [HttpGet]
        [Authorize(Roles = "User, Manager")]
        public async Task<ActionResult> Edit(Guid id)
        {
            var dir = await _fileService.GetAsync(id);
            return View(dir);
        }

        /// <summary>
        /// Edit specific file 
        /// </summary>
        /// <param name="file">file model</param>
        /// <returns>View with file new values</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User, Manager")]
        public async Task<ActionResult> Edit(FileModel file)
        {
            try
            {
                await _fileService.UpdateAsync(file, PhysicalDirectory);
                ViewBag.Message = "File was edited successfully";
                return View(file);
            }
            catch (FileException e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(file);
            }
        }

        /// <summary>
        /// Get important files
        /// </summary>
        /// <returns>View with list of important files</returns>
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult Important()
        {
            var files = _fileService.GetImportant(UserId);
            return View(files);
        }

        /// <summary>
        /// Get all files
        /// </summary>
        /// <returns>View with list of the files</returns>
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> SearchByName()
        {
            ViewBag.IsDateFilterOn = false;
            ViewBag.IsTypeFilterOn = false;

            var files = await _fileService.GetAll();
            return View(files);
        }

        /// <summary>
        /// Get files with specific name
        /// </summary>
        /// <param name="name">part of the file name</param>
        /// <returns>View with list of the files with specific name</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public ActionResult SearchByName(string name, string isDateFilter = "off", string isTypeFilter = "off")
        {
            var isDateSort = ViewBag.IsDateFilterOn = isDateFilter == "on";
            var isTypeSort = ViewBag.IsTypeFilterOn = isTypeFilter == "on";

            IEnumerable<FileModel> files;
            files = _fileService.GetByName(name, isDateSort, isTypeSort);
            
            return View(files);
        }

        /// <summary>
        /// Get link for sharing and it recipients
        /// </summary>
        /// <param name="id">file id</param>
        /// <returns>View with recipients and link</returns>
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> Share(Guid id)
        {
            var file = await _fileService.GetAsync(id);
            var users = _userService.GetAllUsers(UserId);

            ViewBag.ShareLink = await GenerateLink(file);
            ViewBag.FileId = file.Id;
            ViewBag.FileName = file.Name;

            var modelRecipients = new Recipients
            {
                AllUsers = users,
                SelectedUsersId = file.Recipients.Select(f => f.ApplicationUserId).ToList()
            };

            return View(modelRecipients);
        }

        /// <summary>
        /// Set new recipients of specific file
        /// </summary>
        /// <param name="id">file id</param>
        /// <param name="selectedUsersId">recipients id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> Share(Guid id, IEnumerable<string> selectedUsersId)
        {
            var file = await _fileService.GetAsync(id);

            ViewBag.ShareLink = await GenerateLink(file);
            ViewBag.FileId = file.Id;
            ViewBag.FileName = file.Name;

            var selectedUsers = _userService.GetUsersByIds(selectedUsersId);
            var users = _userService.GetAllUsers(UserId);
            var recipients = await _fileService.ShareForRecipientsAsync(id, selectedUsers, users);

            ViewBag.Message = "File was successfully shared";

            return View(recipients);
        }

        /// <summary>
        /// Get available file, that were shared for current user
        /// </summary>
        /// <returns>View with available files</returns>
        [HttpGet]
        [Authorize]
        public ActionResult Available()
        {
            if (User.IsInRole("Manager") && !User.IsInRole("User"))
            {
                throw new ForbiddenException();
            }
            var files = _fileService.GetAvailable(UserId);
            return View(files);
        }

        private async Task<string> GenerateLink(FileModel file)
        {
            var urn = file.Link?.ShortLink ?? await _fileService.CreateLinkAsync(file.Id);
            var url = Request.Url;

            return $"{url?.Scheme}{Uri.SchemeDelimiter}{url?.Host}:{url?.Port}/sharing/{urn}";
        }
    }
}