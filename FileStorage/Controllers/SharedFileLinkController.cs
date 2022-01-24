using BLL.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FileStorage.Controllers
{
    /// <summary>
    /// SharedFileLinkController responsive for sharing file by link
    /// </summary>
    [Authorize]
    public class SharedFileLinkController : BaseStorageController
    {
        private readonly IFileService _fileService;

        /// <summary>
        /// SharedFileLinkController constructor
        /// </summary>
        /// <param name="fileService">Service instance for IFileService</param>
        public SharedFileLinkController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Shared file representation 
        /// </summary>
        /// <param name="link">query part from URN</param>
        /// <returns>View for file information</returns>
        [HttpGet]
        public ActionResult SharedByLink(string link)
        {
            var file = _fileService.GetFileByShortLink(link);
            return View(file);
        }

        /// <summary>
        /// Add file to available list
        /// </summary>
        /// <param name="id">file id</param>
        /// <returns>User's available view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetToAvailable(Guid id)
        {
            await _fileService.ShareByLinkAsync(id, UserId);
            return RedirectToAction("Available", "File");
        }
    }
}