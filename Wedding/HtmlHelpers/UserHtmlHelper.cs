using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wedding.HtmlHelpers
{
    public static class UserHtmlHelper
    {
        private const string navTitle = "Your ZenZoy";

        public static MvcHtmlString GetUserNameHelper(this HtmlHelper helper)
        {
            var username = navTitle;

            if (HttpContext.Current.Session["UserFirstName"] == null)
            {
                var OwinContext = HttpContext.Current.GetOwinContext();
                var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var user = OwinContext.GetUserManager<ApplicationUserManager>().FindById(userId);

                if (user != null)
                {
                    HttpContext.Current.Session["UserFirstName"] =
                        String.IsNullOrEmpty(user.FirstName) ? null : (user.FirstName.Length > 8 ? String.Format("{0}...", user.FirstName.Substring(0, 8)) : user.FirstName);
                }
            }

            if (HttpContext.Current.Session["UserFirstName"] != null)
            {
                username = HttpContext.Current.Session["UserFirstName"].ToString();
            }



            return new MvcHtmlString(username);
        }
    }
}