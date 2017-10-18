using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Wedding.Controllers
{
    // [HandleError]
    public class ErrorController : BaseController
    {
        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public ActionResult Index()
        {

            var httpException = new HandleErrorInfo(new HttpException(403, "Dont allow access the error pages"), "ErrorController", "Index");

            return RedirectToAction("GenericError", httpException);
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public ActionResult GenericError(HandleErrorInfo exception)
        {
            return View("Error", exception);
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public ViewResult NotFound(string url)
        {
            ViewBag.Title = "Page Not Found";
            return View("NotFound");
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public ViewResult Oops(string url)
        {
            ViewBag.Title = "Oops";
            return View("Oops");
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Forbidden()
        {
            //403
            Response.StatusCode = (int)HttpStatusCode.Forbidden;

            ViewBag.Title = "403 Forbidden";
            return View("Oops");
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult NotAuthorized()
        {
            //401
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            ViewBag.Title = "401 Unauthorized";
            return View("Oops");
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public ActionResult ServerError()
        {
            //500
            Response.StatusCode = (int)HttpStatusCode.NotFound;

            ViewBag.Title = "500 NotFound";
            return View("Oops");
        }
    }

}