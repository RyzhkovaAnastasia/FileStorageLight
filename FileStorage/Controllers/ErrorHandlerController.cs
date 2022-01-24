using System.Web.Mvc;
using BLL.Models;

namespace FileStorage.Controllers
{
    /// <summary>
    /// ErrorHandlerController responsible for error view handling
    /// </summary>
    public class ErrorHandlerController : Controller
    {
        /// <summary>
        /// Base view for errors
        /// </summary>
        /// <param name="error">error info</param>
        /// <returns>View with current error values</returns>
        public ActionResult Error(ErrorModel error)
        {
            return View(error);
        }

        /// <summary>
        /// CreateAsync model for Not found 404 error
        /// </summary>
        /// <returns>Redirect to base view with model</returns>
        public ActionResult NotFound()
        {
            ErrorModel error = new ErrorModel()
            {
                ErrorCode = 404,
                Title = "Not found",
                Message = "The resource could not be found on this server"
            };
            return RedirectToAction("Error", "ErrorHandler", error);
        }

        /// <summary>
        /// CreateAsync model for Not found 403 error
        /// </summary>
        /// <returns>Redirect to base view with model</returns>
        public ActionResult Forbidden()
        {
            ErrorModel error = new ErrorModel()
            {
                ErrorCode = 403,
                Title = "Forbidden",
                Message = "You do not have sufficient privileges to do this operation"
            };
            return RedirectToAction("Error", "ErrorHandler", error);
        }

        /// <summary>
        /// CreateAsync model for Not found 500 error
        /// </summary>
        /// <returns>Redirect to base view with model</returns>
        public ActionResult InternalError()
        {
            ErrorModel error = new ErrorModel()
            {
                ErrorCode = 500,
                Title = "Internal error",
                Message = "Server side error occurred"
            };
            return RedirectToAction("Error", "ErrorHandler", error);
        }

        /// <summary>
        /// CreateAsync model for Not found 413 error
        /// </summary>
        /// <returns>Redirect to base view with model</returns>
        public ActionResult FileTooLarge()
        {
            ErrorModel error = new ErrorModel()
            {
                ErrorCode = 413,
                Title = "Request entity too large",
                Message = "File was more than 100MB size"
            };
            return RedirectToAction("Error", "ErrorHandler", error);
        }
    }
}