using Domain.Services.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wedding.Attributes;

namespace Wedding.Controllers
{
    public class SocialController : Controller
    {
        private ISocialService _socialService;

        public SocialController(ISocialService socialService)
        {
            _socialService = socialService;
        }


        [HttpPost]
        [ValidateJsonAntiForgeryTokenAttribute]
        public async Task<JsonResult> LikeDislike(int postedItemId, bool? liked)
        {
            if (postedItemId == 0)
                return Json(new { });

            var userId = HttpContext.User.Identity.GetUserId();

            if (String.IsNullOrEmpty(userId))
                return Json(new { });

            await _socialService.PersistLikeDislike(postedItemId, userId, liked);

            return Json(new { success = true });
        }
    }
}