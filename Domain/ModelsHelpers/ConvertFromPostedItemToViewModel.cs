using Domain.Models;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ModelsHelpers
{
    public class ConvertFromPostedItemToViewModel
    {
        private static string _usedOnlyForLocking = "lock";

        /// <summary>
        /// Idea is from here : https://msdn.microsoft.com/en-us/library/mt679037.aspx
        /// </summary>
        /// <param name="postedItem"></param>
        /// <returns></returns>
        public static PostedItemViewModel Convert(PostedItem postedItem, string userId = null)
        {
            object obj = (object)_usedOnlyForLocking;
            System.Threading.Monitor.Enter(obj);
            try
            {
                var viewModel = new PostedItemViewModel
                {
                    Title = postedItem.Title,
                    Id = postedItem.Id,
                    ApplicationUserId = postedItem.ApplicationUserId,
                    OpinionsTokenized = postedItem.OpinionsTokenized,
                    PhotoUrl = postedItem.PhotoUrl,
                    VideoSourceUrl = postedItem.VideoSourceUrl,
                    Slug = postedItem.Slug,
                    Source = postedItem.Source,
                    MetadataKeywords = string.Join(",", postedItem.Tags.Select(t => t.Name)),
                    TagsTokenized = postedItem.TagsTokenized,
                    TotalDislike = postedItem.TotalDislike,
                    TotalLike = postedItem.TotalLike,
                    Updated = postedItem.Updated
                };

                UpdateIsLiked(viewModel, postedItem, userId);

                return viewModel;
            }
            finally
            {
                System.Threading.Monitor.Exit(obj);
            }
        }

        private static void UpdateIsLiked(PostedItemViewModel postedItemViewModel, PostedItem postedItem, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                return;

            if (!postedItem.Opinions.Any(o => o.ApplicationUser.Id == userId) || postedItem.Opinions.Any(o => o.ApplicationUser.Id == userId && o.OpinionType == OpinionType.NoOpinion))
                postedItemViewModel.IsLiked = default(bool?);
            else
                postedItemViewModel.IsLiked = postedItem.Opinions.Any(o => o.ApplicationUser.Id == userId && o.OpinionType == OpinionType.Like);
        }

    }
}
