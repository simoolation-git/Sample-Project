using Domain.Services;
using Domain.Services.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wedding.ActionFilters;
using Wedding.Attributes;
using Wedding.HttpFileResult;
using Wedding.Services;

namespace Wedding.Controllers
{
    [RequireHttps]
    [HandleError]
    [RoutePrefix("")]
    //[ClaimsAuthorize(ClaimTypes.Role, "User")]
    public class HomeController : BaseController
    {
        private readonly IAzureSearchService _azureSearchService;
        private readonly IPostedItemService _postedItemService;
        private readonly ISitemapService _sitemapService;
        private readonly IConfigurationService _configurationService;

        public HomeController(IAzureSearchService azureSearchService, IPostedItemService postedItemService, ISitemapService sitemapService, IConfigurationService configurationService) : base()
        {
            _sitemapService = sitemapService;
            _azureSearchService = azureSearchService;
            _postedItemService = postedItemService;
            _configurationService = configurationService;
        }

        //[LogActionFilter]
        //[OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public async Task<ActionResult> Index(int? pageNumber = null)
        {
            var userId = HttpContext.User.Identity.GetUserId();
            var results = await _azureSearchService.Search("", 1, userId);

            ViewBag.HtmlSchema = "";

            return View(results);
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public ActionResult Contact()
        {
            ViewBag.HtmlSchema = "";

            return View();
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public ActionResult Terms()
        {
            ViewBag.HtmlSchema = "";

            return View();
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public ActionResult Privacy()
        {
            ViewBag.HtmlSchema = "";

            return View();
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public ActionResult VerticalTimeLine()
        {
            ViewBag.HtmlSchema = "";

            return View();
        }

        //[OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult LoginRegisterModalPartial()
        {
            return PartialView("_LoginRegisterModalPartial");
        }

        /// <summary>
        /// 86400 is one day
        /// 172800 is two days
        /// 518400 is six days
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public ActionResult PrivacyPartial()
        {
            return PartialView("_PrivacyPartial");
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public ActionResult TermsPartial()
        {
            return PartialView("_TermsPartial");
        }

        [OutputCache(Duration = int.MaxValue, VaryByParam = "none")]
        public ActionResult FooterPartial()
        {
            return PartialView("_FooterPartial");
        }

        [OutputCache(Duration = 518400, VaryByParam = "none")]
        public ActionResult IndexPartial()
        {
            return PartialView("_IndexPartial");
        }

        //[OutputCache(Duration = 300, VaryByParam = "none")]
        //public ActionResult RenderServerGeneratedPostedItemsPartial()
        //{
        //    var userId = HttpContext.User.Identity.GetUserId();
        //    var results = _azureSearchService.Search("", 1, userId).ConfigureAwait(true);

        //    return PartialView("_ServerGeneratedPostedItemsPartial", results);
        //}

        [HttpGet]
        public ActionResult RenderLoginPartial()
        {

            return PartialView("~/Views/Shared/_LoginPartial.cshtml");
        }

        [HttpGet]
        public ActionResult AddNewPostedItemButton()
        {

            return PartialView("_AddNewPostedItemButtonPartial");
        }

        [HttpGet]
        public JsonResult IsAuthenticated()
        {
            return Json(User.Identity.IsAuthenticated, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult RefreshToken()
        {
            return PartialView("_AntiForgeryTokenPartial");
        }

        [HttpGet]
        public async Task<ActionResult> Feed()
        {
            var items = new List<SyndicationItem>();

            var postedItems = await _postedItemService.GetPostedItems(_configurationService.GetPostedItemCountPerPage());

            foreach (var postedItem in postedItems)
            {
                string feedTitle = postedItem.Title;

                var helper = new UrlHelper(this.Request.RequestContext);
                //var url = helper.Action("Index", "Home", new { }, Request.IsSecureConnection ? "https" : "http");

                var url = helper.Action("Index", "PostedItems", new { }, this.Request.Url.Scheme);

                url = String.Format("{0}/{1}/{2}", url, postedItem.Id, postedItem.Slug);

                var feedPackageItem = new SyndicationItem(feedTitle, feedTitle, new Uri(url));
                feedPackageItem.PublishDate = DateTime.Now;
                items.Add(feedPackageItem);
            }

            return new RssResult("zenzoy.com feed", items);
        }

        [Route("sitemap.xml")]
        public async Task<ActionResult> SitemapXml()
        {
            //System.IO.TextWriter writer = new System.IO.StringWriter();
            //var h = new HtmlHelper(new ViewContext(ControllerContext, new WebFormView(ControllerContext, "omg"), new ViewDataDictionary(), new TempDataDictionary(), writer), new ViewPage());

            var canonicalLinkUrl = ViewBag.MetaDataViewModel.BaseAddress;

            var xml = await _sitemapService.GetSitemapDocument(canonicalLinkUrl);
            
            return this.Content(xml, "xml", Encoding.UTF8);
        }

        [Route("news-sitemap.xml")]
        public async Task<ActionResult> NewsSitemapXml()
        {
            var canonicalLinkUrl = ViewBag.MetaDataViewModel.BaseAddress;

            var xml = await _sitemapService.GetNewsSitemapDocument(canonicalLinkUrl);

            return this.Content(xml, "xml", Encoding.UTF8);
        }
    }
}