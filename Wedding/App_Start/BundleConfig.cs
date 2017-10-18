using System.Web;
using System.Web.Optimization;

namespace Wedding
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.1.1.min.js",
                        "~/Scripts/amplify.min.js",
                        "~/Scripts/toastr.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/node_modules/tether/dist/js/tether.min.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/ie10-viewport-bug-workaround.js",
                      "~/Scripts/respond.min.js",
                      "~/Scripts/app/login-register-modal-helper.js",
                      "~/Scripts/app/pop-up-menu.js"));

            bundles.Add(new ScriptBundle("~/bundles/semantic").Include(
                      "~/Vendors/Semantic-UI-CSS/semantic.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/materialdesignicons.min.css",
                      "~/Vendors/Semantic-UI-CSS/semantic.min.css",
                      "~/Content/toastr.min.css",
                      "~/Content/ie10-viewport-bug-workaround.css",
                      "~/Content/site.css",
                      "~/Content/sticky-footer.css",
                      "~/Content/header.css",
                      "~/Content/search.css",
                      "~/ComponentAngular1/timeline/timeline-component.css",
                      "~/Content/carousel.css"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                                         "~/Scripts/angular.js",
                                         "~/Scripts/app/searchbar-component.js",
                                         "~/Scripts/ng-infinite-scroll.min.js",
                                         "~/Scripts/app/view-post-mainpage-controller.js",
                                         "~/ComponentAngular1/timeline/timeline-component.js"));

            bundles.Add(new ScriptBundle("~/bundles/react").Include(
                                        "~/Scripts/react/react.min.js",
                                        "~/Scripts/react/react-dom.min.js",
                                        "~/Scripts/lib/immutable-js/immutable.min.js",
                                        "~/Scripts/app-react/display-search-results.js"));

        }
    }
}
