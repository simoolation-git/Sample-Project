using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Wedding
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            //routes.MapRoute(
            //    name: "Oops",
            //    url: "Oops/{action}/{id}",
            //    defaults: new { controller = "Error", action = "Oops", id = UrlParameter.Optional }
            //);

            //routes.MapRoute(
            //    name: "NotFound",
            //    url: "NotFound/{action}/{id}",
            //    defaults: new { controller = "Error", action = "NotFound", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
                    name: "Sitemap",
                    url: "sitemap.xml",
                    defaults: new { controller = "Home", action = "SitemapXml" }
            );

            routes.MapRoute(
                    name: "NewsSitemap",
                    url: "news-sitemap.xml",
                    defaults: new { controller = "Home", action = "NewsSitemapXml" }
            );

            //This is for SEO we will add slug this will help our articles to be index easier
            routes.MapRoute(name: "GetPostedItemDetails", // Route name
                            url: "PostedItems/GetPostedItemDetails/{id}",
                            defaults: new
                            {
                                controller = "PostedItems",
                                action = "GetPostedItemDetails",
                                id = ""
                            });

            routes.MapRoute(name: "GetPostedItemsByPage", // Route name
                 url: "PostedItems/Page/{pageNumber}",
                 defaults: new
                 {
                     controller = "PostedItems",
                     action = "Page",
                     pageNumber = ""
                 });

            routes.MapRoute(name: "RefreshPostedItemPartial", // Route name
                url: "PostedItems/RefreshPostedItemPartial/{id}/{isPostedItemPage}",
                defaults: new
                {
                    controller = "PostedItems",
                    action = "RefreshPostedItemPartial",
                    id = UrlParameter.Optional,
                    isPostedItemPage = false
                });

            routes.MapRoute(name: "PostedItems", // Route name
                url: "PostedItems/{id}/{slug}",
                defaults: new
                {
                    controller = "PostedItems",
                    action = "Index",
                    id = "",
                    slug = UrlParameter.Optional
                });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{pageNumber}",
                defaults: new { controller = "Home", action = "Index", pageNumber = UrlParameter.Optional }
            );


        }
    }

}
