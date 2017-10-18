using Domain.Models;
using Domain.Services.Interfaces;
using Domain.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Wedding.Controllers
{
    public class PostedItemsController : BaseController
    {
        private readonly IAzureSearchService _azureSearchService;
        private readonly IPostedItemService _postedItemService;

        public PostedItemsController(IAzureSearchService azureSearchService, IPostedItemService postedItemService)
        {
            _azureSearchService = azureSearchService;
            _postedItemService = postedItemService;
        }

        // GET: PostedItem
        //[OutputCache(Duration = int.MaxValue, VaryByParam = "slug")]
        public async Task<ActionResult> Index(long id, string slug)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var postedItem = await _postedItemService.GetPostedItemViewModelById(id, userId);

            if (postedItem == null)
                return new HttpNotFoundResult();


            if (string.IsNullOrEmpty(slug))
                return RedirectPermanent(Url.Action("Index", "PostedItems", new { id = id, slug = postedItem.Slug }));
            else
            {
                MetaDataViewModel.MetadataKeywords = postedItem.MetadataKeywords;
                MetaDataViewModel.Description = postedItem.Title;
                MetaDataViewModel.ImageSource = postedItem.PhotoUrl;
                MetaDataViewModel.VideoSource = postedItem.VideoSourceUrl;
                MetaDataViewModel.FacebookOpenGraph.Type = "video";

                ViewBag.MetaDataViewModel = MetaDataViewModel;

                ViewBag.HtmlSchema = "itemscope itemtype=\"http://schema.org/QAPage\"";

                var postedItemWrapperViewModel = await GetPostedItemWrapperViewModel(postedItem, userId);

                return View("Index", postedItemWrapperViewModel);
            }

        }

        private async Task<PostedItemWrapperViewModel> GetPostedItemWrapperViewModel(PostedItemViewModel postedItem, string userId)
        {
            var postedItemWrapperViewModel = new PostedItemWrapperViewModel();
            postedItemWrapperViewModel.MainPostedItemViewModel = postedItem;

            if (postedItem.Tags != null && postedItem.Tags.Length > 0)
            {
                var relatedPosts = await _azureSearchService.Search(string.Join(" ", postedItem.Tags), 1, userId, true, true, 4);
                postedItemWrapperViewModel.RelatedPostedItems = relatedPosts.Where(r => r.Id != postedItem.Id).ToList();
            }
            return postedItemWrapperViewModel;
        }

        public async Task<JsonResult> GetPostedItemDetails(long id)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var postedItem = await _postedItemService.GetPostedItemViewModelById(id, userId);

            return Json(postedItem.IsLiked, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RenderPostedItemPartial(PostedItemViewModel postedItemViewModel)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            return PartialView("_PostedItemCardPartial", postedItemViewModel);
        }

        [HttpGet]
        public async Task<ActionResult> RefreshPostedItemPartial(long id, bool isPostedItemPage)
        {
            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var postedItem = await _postedItemService.GetPostedItemViewModelById(id, userId);

            postedItem.IsPostedItemPage = isPostedItemPage;

            return View("_PostedItemCardPartial", postedItem);
        }


        ///// <summary>
        ///// 86400 is one day
        ///// 172800 is two days
        ///// 518400 is six days
        ///// </summary>
        ///// <returns></returns>
        //[OutputCache(Duration = 518400, VaryByParam = "id")]
        //public ActionResult IndexPartial(string id)
        //{
        //    if (!String.IsNullOrEmpty(id))
        //    {
        //        var postedItem = await _azureSearchService.GetSearchResult(id);
        //        return PartialView("_IndexPartial");
        //    }

        //    return PartialView("_IndexPartial");
        //}

        public async Task<ActionResult> Page(int? pageNumber)
        {
            IList<PostedItemViewModel> postedItems = null;

            var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (pageNumber == null)
            {
                ViewBag.PageNumber = 1;
                postedItems = await _azureSearchService.Search("", 1, userId);
                return View(postedItems);
            }

            ViewBag.PageNumber = pageNumber.Value;
            postedItems = postedItems = await _azureSearchService.Search("", pageNumber.Value, userId, false, true);

            return View("Page", postedItems);
        }


        public ActionResult PageLet1()
        {
            System.Threading.Thread.Sleep(new Random().Next(3000, 10000));
            return View("~/Views/Pagelets/_PageletPartialPage1.cshtml");
        }

    }
}