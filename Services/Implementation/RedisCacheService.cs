using Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using StackExchange.Redis;
using System.Configuration;
using Newtonsoft.Json;
using Domain.ViewModels;

namespace Services.Implementation
{
    public class RedisCacheService : ICacheService
    {
        private string _enviornment;
        private static string _redisConfiguration;

        public RedisCacheService()
        {
            _redisConfiguration = ConfigurationManager.AppSettings["RedisConfiguration"];

#if DEBUG
            _enviornment = "dev";
#else
            _enviornment = "production";
#endif
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(_redisConfiguration);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        /// <summary>
        /// This is the Cache instance that we will have to intract with
        /// </summary>
        private static IDatabase Cache
        {
            get
            {
                return Connection.GetDatabase();
            }
        }

        private string SafePostedItemKey(string key)
        {
            return String.Format("{0}-posted-item-{1}", _enviornment, key);
        }

        private string SafeKey(string key)
        {
            return String.Format("{0}-{1}", _enviornment, key);
        }

        public async Task AddPostedItemToCache(string key, PostedItemViewModel postedItem)
        {
            if (String.IsNullOrEmpty(key) || postedItem == null)
                return;

            await RemovePostedItemFromCache(key);

            await Cache.StringSetAsync(SafePostedItemKey(key), JsonConvert.SerializeObject(postedItem), TimeSpan.FromDays(7));
        }

        public async Task RemovePostedItemFromCache(string key)
        {
            if (String.IsNullOrEmpty(key))
                return;

            await Cache.KeyDeleteAsync(SafePostedItemKey(key));
        }

        public async Task<PostedItemViewModel> GetPostedItemFromCache(string key, string userId = null)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var serializedJson = await Cache.StringGetAsync(SafePostedItemKey(key));

            if (string.IsNullOrEmpty(serializedJson))
                return null;

            var viewModel = JsonConvert.DeserializeObject<PostedItemViewModel>(serializedJson);

            UpdateIsLiked(viewModel, userId);

            return viewModel;
        }

        private void UpdateIsLiked(PostedItemViewModel postedItemViewModel, string userId = null)
        {
            if (postedItemViewModel == null)
                return;

            if (!string.IsNullOrEmpty(userId) && postedItemViewModel.OpinionsTokenized != null)
            {
                Dictionary<string, int> opinions = JsonConvert.DeserializeObject<Dictionary<string, int>>(postedItemViewModel.OpinionsTokenized);

                if (opinions == null || opinions.Count == 0 || !opinions.ContainsKey(userId))
                    postedItemViewModel.IsLiked = null;
                else
                {
                    postedItemViewModel.IsLiked = opinions[userId] == 1 ? true : opinions[userId] == -1 ? default(bool?) : false;
                }
            }
            else
            {
                postedItemViewModel.IsLiked = null;
            }
        }

        public async Task AddToCache(string key, string payload)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(payload))
                return;

            await Cache.KeyDeleteAsync(SafeKey(key));

            await Cache.StringSetAsync(SafeKey(key), payload, TimeSpan.FromDays(1));
        }

        public async Task<string> GetFromCache(string key)
        {
            var payload = await Cache.StringGetAsync(SafeKey(key));

            return payload;
        }
    }
}
