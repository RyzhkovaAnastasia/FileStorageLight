using Microsoft.AspNet.Identity;
using System.IO;
using System.Web.Mvc;
using NLog;

namespace FileStorage.Controllers
{
    /// <summary>
    /// BaseStorageController responsible for handling errors and basic functions for child controllers
    /// </summary>
    
    public class BaseStorageController : Controller
    {
        protected string UserId => User.Identity.GetUserId();
        protected string PhysicalDirectory => GetPhysicalUserDirectory();
        /// <summary>
        /// Throw custom exceptions with it's logging
        /// </summary>
        /// <param name="filterContext">exception details</param>
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            var logger = LogManager.GetCurrentClassLogger();
            logger.Error(filterContext.Exception.Message);

            var responseCode = Response.StatusCode;
            var exception = filterContext.Exception;
            switch (exception.GetType().ToString())
            {
                case "BLL.Exceptions.NotFoundException":
                    responseCode = 404;
                    break;

                case "BLL.Exceptions.ForbiddenException":
                    responseCode = 403;
                    break;
                default:
                    responseCode = 500;
                    break;
            }
            Response.StatusCode = responseCode;
        }

        /// <summary>
        /// Create user directory for file and folders saving
        /// </summary>
        protected void SetUserDirectory()
        {
            var initPath = Server.MapPath("~/Files");
            if (!Directory.Exists(UserId))
            {
                var userDir = Path.Combine(initPath, UserId);
                Directory.CreateDirectory(userDir);
            }
        }

        /// <summary>
        /// Get local server directory of current user
        /// </summary>
        /// <returns>server user directory</returns>
        protected string GetPhysicalUserDirectory()
        {
            var serverUserDir = Server.MapPath("~/Files");
            var userDir = Path.Combine(serverUserDir, UserId);
            return userDir;
        }
    }
}