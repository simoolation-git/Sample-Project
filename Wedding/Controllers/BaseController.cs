using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.ViewModels;

namespace Wedding.Controllers
{
    public abstract partial class BaseController : Controller
    {

        protected MetaDataViewModel MetaDataViewModel;

        public BaseController()
        {

            MetaDataViewModel = new MetaDataViewModel();

            MetaDataViewModel.Description = "Fun new way to get the Latest News in form of short videos! Enjoy browsing through Todays Top Trending News, Customized to Your taste! World, Tech, Sports, Arts, Celeb News and much more!";
            MetaDataViewModel.MetadataKeywords = "zenzoy,news,latest,today,breaking,daily,top,trending,world,celebrity,events,recent,headlines,hot";
            MetaDataViewModel.ImageSource = "https://www.zenzoy.com/images/logo.png";
            MetaDataViewModel.FacebookOpenGraph.Type = "site";
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var host = !Request.Headers["host"].Contains("www") ? Request.Headers["host"] : String.Format("www.{0}", Request.Headers["host"]);
            var host = Request.Headers["host"];

            MetaDataViewModel.CanonicalLink = string.Format("{0}://{1}{2}", Request.Url.Scheme, host, Request.RawUrl);

            MetaDataViewModel.BaseAddress = string.Format("{0}://{1}", Request.Url.Scheme, host);

            ViewBag.MetaDataViewModel = MetaDataViewModel;

            base.OnActionExecuting(filterContext);
        }
    }
}