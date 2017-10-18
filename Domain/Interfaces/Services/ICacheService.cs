using Domain.Models;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface ICacheService
    {
        Task AddPostedItemToCache(string key, PostedItemViewModel postedItem);
        Task<PostedItemViewModel> GetPostedItemFromCache(string key, string userId = null);
        Task RemovePostedItemFromCache(string key);

        Task AddToCache(string key, string payload);
        Task<string> GetFromCache(string key);
    }
}
