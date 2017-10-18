using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Wedding.Attributes;
using System.Security.Claims;

namespace Wedding.Controllers
{
    //[ClaimsAuthorize(ClaimTypes.Role, "User")]
    [Authorize]
    public class ProfileController : BaseController
    {
        // GET: Profile
        public ActionResult Index()
        {
            if (this.Session["UserFirstName"] == null)
            {
                var OwinContext = System.Web.HttpContext.Current.GetOwinContext();
                var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var user = OwinContext.GetUserManager<ApplicationUserManager>().FindById(userId);

                if (user != null)
                {
                    this.Session["UserFirstName"] = user.FirstName;
                }
            }

            ViewBag.UserFirstName = this.Session["UserFirstName"];
            ViewBag.HtmlSchema = "itemscope itemtype=\"http://schema.org/ProfilePage\"";
            
            return View();
        }
    }
}