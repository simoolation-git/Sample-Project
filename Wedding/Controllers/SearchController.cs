using Domain.Services.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Wedding.Controllers
{
    //[RoutePrefix("Search")]
    public class SearchController : Controller
    {
        private readonly IAzureSearchService _azureSearchService;
        private string currentPageSessionKey = "CurrentPage";
        private string currentTermSearchedKey = "CurrentTermSearched";

        public SearchController(IAzureSearchService azureSearchService)
        {
            _azureSearchService = azureSearchService;
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (!Request.IsAjaxRequest())
            {
                Session[currentTermSearchedKey] = "";
                Session[currentPageSessionKey] = null;
            }
        }

        [HttpPost]
        [Route("Search/FindPost")]
        public async Task<JsonResult> FindPost(string term)
        {
            var oldterm = GetCurrentTermSearched();

            //Lets reset the counter
            if (term != oldterm)
                Session[currentPageSessionKey] = null;

            //reset the session
            Session[currentTermSearchedKey] = term;

            if (term == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }

            var userId = HttpContext.User.Identity.GetUserId();

            //always get first page only when user search for a term
            var results = await _azureSearchService.Search(term, 1, userId, true);

            IncreaseCurrentPageNumber();

            return Json(results);
        }

        [HttpGet]
        [Route("Search/NextPage")]
        public async Task<JsonResult> NextPage()
        {
            var term = GetCurrentTermSearched();

            if (term == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }

            var userId = HttpContext.User.Identity.GetUserId();

            var results = await _azureSearchService.Search(term, GetCurrentPageNumber(), userId, true);

            IncreaseCurrentPageNumber();

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        private string GetCurrentTermSearched()
        {
            if (Session[currentTermSearchedKey] != null)
            {
                return (string)Session[currentTermSearchedKey];
            }

            return String.Empty;
        }

        private int GetCurrentPageNumber()
        {
            if (Session[currentPageSessionKey] != null)
            {
                return (int)Session[currentPageSessionKey];
            }

            return 1;
        }

        private void IncreaseCurrentPageNumber()
        {
            Session[currentPageSessionKey] = GetCurrentPageNumber() + 1;
        }
    }
}