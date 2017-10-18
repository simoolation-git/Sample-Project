using Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Wedding.Repository;
using System.Threading.Tasks;
using System.Data.Entity;
using Domain.Models;
using Newtonsoft.Json;
using Domain.Interfaces.Services;

namespace Services.Implementation
{
    public class SocialService : ISocialService
    {
        private ApplicationDbContext _applicationDbContext;
        private IOpinionsTableStorage _opinionsTableStorage;
        private ICacheService _cacheService;

        public SocialService(IOpinionsTableStorage opinionsTableStorage, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _opinionsTableStorage = opinionsTableStorage;
            _applicationDbContext = new ApplicationDbContext();
        }

        public async Task PersistLikeDislike(int postedItemId, string userId, bool? liked)
        {
            if (postedItemId <= 0)
                return;

            var user = await _applicationDbContext.Users.FirstOrDefaultAsync(p => p.Id == userId);
            if (user == null)
                return;

            var postedItem = await _applicationDbContext.PostedItems.FirstOrDefaultAsync(p => p.Id == postedItemId);
            if (postedItem == null)
                return;

            var existingOpinion = user.Opinions.FirstOrDefault(o => o.PostedItem.Id == postedItemId);
            var timeNow = DateTime.UtcNow;

            Dictionary<string, int> output = null;
            if (!String.IsNullOrEmpty(postedItem.OpinionsTokenized))
                output = JsonConvert.DeserializeObject<Dictionary<string, int>>(postedItem.OpinionsTokenized);
            else
                output = new Dictionary<string, int>();


            if (liked == null && existingOpinion != null)
            {
                //lets remove opinion from database
                user.Opinions.Remove(existingOpinion);
                postedItem.Opinions.Remove(existingOpinion);
                output.Remove(user.Id);
                _applicationDbContext.Opinions.Remove(existingOpinion);
            }
            else
            {
                if (existingOpinion == null)
                {
                    existingOpinion = new Opinion { ApplicationUser = user, PostedItem = postedItem, OpinionType = GetOpinion(liked), Inserted = timeNow };
                    postedItem.Opinions.Add(existingOpinion);
                    user.Opinions.Add(existingOpinion);
                }

                existingOpinion.OpinionType = GetOpinion(liked);
                output[user.Id] = (int)existingOpinion.OpinionType;
            }

            UpdatePostedItem(postedItem, user.Id, output);

            postedItem.Updated = timeNow;
            existingOpinion.Updated = timeNow;

            await _applicationDbContext.SaveChangesAsync();

            await UpdateTableStorage(userId, postedItemId, liked);

            await _cacheService.RemovePostedItemFromCache(postedItemId.ToString());
        }

        private async Task UpdateTableStorage(string userId, int postedItemId, bool? liked)
        {
            var existingOpinions = await _opinionsTableStorage.GetOpinion(userId);

            ////1. remove it
            //if (liked == null && existingOpinions.ContainsKey(postedItemId))
            //{
            //    existingOpinions.Remove(postedItemId);
            //}
            //else
            //{
            //2. add/update
            existingOpinions[postedItemId] = (int)GetOpinion(liked);
            // }

            await _opinionsTableStorage.UpdateOpinion(userId, existingOpinions);
        }

        private OpinionType GetOpinion(bool? liked)
        {
            return (liked != null) ? (liked.Value ? OpinionType.Like : OpinionType.Dislike) : OpinionType.NoOpinion;
        }

        private void UpdatePostedItem(PostedItem postedItem, string userId, Dictionary<string, int> output)
        {
            if (postedItem == null)
                return;

            //int totalLike = 0;
            //int totalDislike = 0;

            //for (int index = 0; index < postedItem.Opinions.Count(); index++)
            //{
            //    var opinionType = postedItem.Opinions.ElementAt(index).OpinionType;

            //    if (opinionType == OpinionType.NoOpinion)
            //        continue;

            //    if (opinionType == OpinionType.Like)
            //        totalLike++;
            //    else if (opinionType == OpinionType.Dislike)
            //        totalDislike++;
            //}

            var totalLike = postedItem.Opinions.Count(o => o.OpinionType == OpinionType.Like);
            var totalDislike = postedItem.Opinions.Count(o => o.OpinionType == OpinionType.Dislike);

            string json = JsonConvert.SerializeObject(output);

            postedItem.TotalLike = totalLike;
            postedItem.TotalDislike = totalDislike;
            postedItem.OpinionsTokenized = json;
        }
    }
}