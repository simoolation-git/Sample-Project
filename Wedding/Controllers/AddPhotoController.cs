using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wedding.App_Start;
using Wedding.CustomAttributes;
using Wedding.ExtensionMethods;
using Domain.Models;
using Domain.Services.Interfaces;
using Domain.ViewModels;

namespace Wedding.Controllers
{
    [Authorize]
    public class AddPhotoController : Controller
    {
        IPostedItemService _postedItemService;
        private readonly string _azureBaseBlobUrl;

        public AddPhotoController(IPostedItemService postedItemService)
        {
            _postedItemService = postedItemService;
            _azureBaseBlobUrl = _postedItemService.GetAzureBaseBlobUrl();
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public async Task<JsonResult> AddPhoto(AddPhotoViewModel addPhotoViewModel)
        {
            //1. if new image is added, PostedId is null but PhotoData is not null
            //2. if existing image is updated, PostedId is not null but PhotoData is null
            if (addPhotoViewModel.PostedId == null && String.IsNullOrEmpty(addPhotoViewModel.PhotoData))
            {
                ModelState.AddModelError("PhotoData", "Please add an image.");
            }

            //1. Is Model valid
            if (addPhotoViewModel != null && ModelState.IsValid)
            {
                var id = HttpContext.User.Identity.GetUserId();

                var result = await _postedItemService.SavePostedItem(addPhotoViewModel, id);

                if (result.Success)
                {
                    var postedItem = GetPostedItem(result.PostedItem);
                    return Json(postedItem);
                }
                else
                {
                    ModelState.AddModelError("SavingError", result.ErrorMessage);
                }
            }

            var modelErrors = ModelState.GetErrorsForJson();
            return Json(new { status = "error", errors = modelErrors, success = false, message = "Could not save/update this post." });
        }

        private dynamic GetPostedItem(PostedItem postedItem)
        {
            return new { Id = postedItem.Id, Tags = postedItem.Tags.Select(GetTag), Title = postedItem.Title, PhotoUrl = postedItem.PhotoUrl /*(postedItem.Photo)*/ };
        }

        private string GetPhotoUrl(Photo photo)
        {
            if (photo == null)
                return String.Empty;

            return String.Format("{0}/{1}/{2}.{3}", _azureBaseBlobUrl, photo.Container, photo.Name, photo.FileExtention);
        }

        private dynamic GetTag(Tag tag)
        {
            return new { Id = tag.Id, Name = tag.Name };
        }

        [HttpGet]
        public async Task<JsonResult> GetPhotos()
        {
            var id = HttpContext.User.Identity.GetUserId();

            var result = await _postedItemService.GetPostedItemByUserId(id);

            var response = result.Select(GetPostedItem);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public async Task<JsonResult> RemovePostedItem(long id)
        {
            if (id > 0)
            {
                var result = await _postedItemService.RemovePostedItem(id);

                return Json(new { status = result ? "success" : "error", success = result });
            }

            return Json(new { status = "error", errors = new { }, success = false, message = "Could't remove post." });
        }
    }
}