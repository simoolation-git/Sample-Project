using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Wedding.CustomAttributes
{
    public class BrowserDetectionAttribute : ActionFilterAttribute, IActionFilter
    {
        public static HashSet<string> _supportedCrawlers = new HashSet<string> { "Googlebot", "Mozilla", "MSNBot", "BingPreview", "Mozilla/5.0" };

        public static Dictionary<string, string> _supportedBrowsers = new Dictionary<string, string> { { "Chrome41", "41.0" },
                                                                                                       { "Chrome40", "40.0" },
                                                                                                       { "Firefox37", "37.0" },
                                                                                                       { "Firefox36", "36.0" },
                                                                                                       { "Firefox35", "35.0" },
                                                                                                       { "Firefox34", "34.0" },
                                                                                                       { "Firefox38", "38.0" },
                                                                                                       { "Firefox39", "39.0" },
                                                                                                       { "Safari8", "8.0" },
                                                                                                       { "Safari7", "7.0" },
                                                                                                       { "InternetExplorer11", "11.0"},
                                                                                                       { "Chrome42", "42.0" },
                                                                                                       { "Chrome43", "43.0" },
                                                                                                       { "Chrome44", "44.0" }};

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var httpContext = filterContext.RequestContext.HttpContext;

            //var browser = httpContext.Request.Browser;

            //if (_supportedCrawlers.Contains(browser.Type))
            //    return;

            //var supportedBrowserVersion = _supportedBrowsers.ContainsKey(browser.Type) ? _supportedBrowsers[browser.Type] : null;

            //if (String.IsNullOrEmpty(supportedBrowserVersion) || (!String.IsNullOrEmpty(supportedBrowserVersion) && browser.Version != supportedBrowserVersion))
            //{
            //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Home" }, { "Action", "Browser" } });
            //}
        }
    }
}