using System.Security.Claims;
using System.Web.Mvc;
using Wedding.Attributes;

namespace Wedding.Controllers
{
    //[ClaimsAuthorize(ClaimTypes.Role, "User")]
    [Authorize]
    public class SettingsController : BaseController
    {
        // GET: Settings
        public ActionResult Index()
        {
            ViewBag.HtmlSchema = "";

            return View();
        }
    }
}