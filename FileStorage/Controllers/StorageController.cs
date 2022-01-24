using System;
using System.Threading.Tasks;
using BLL.Interfaces;
using System.Web.Mvc;

namespace FileStorage.Controllers
{
    /// <summary>
    /// StorageController responsive for storage representation
    /// </summary>
    [Authorize]
    public class StorageController : BaseStorageController
    {
        private readonly IStorageService _storageService;

        /// <summary>
        /// StorageController constructor
        /// </summary>
        /// <param name="storageService">Service instance for IStorageService</param>
        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        /// <summary>
        /// Represent files and directories in current directory path
        /// </summary>
        /// <param name="itemId">current directory id</param>
        /// <returns>View with list of files and folders</returns>
        [HttpGet]
        public async Task<ActionResult> Index(Guid? itemId = null)
        {
            SetUserDirectory();
            var items = await _storageService.GetItemsByUserId(itemId.GetValueOrDefault(), UserId);
            return View(items);
        }
    }
}