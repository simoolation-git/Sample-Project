using Domain.Models;
using Domain.ResponseModels;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
    public interface IPostedItemService
    {
        Task<SavePostedItemResponse> SavePostedItem(AddPhotoViewModel addPhotoViewModel, string userId);
        Task<ICollection<PostedItem>> GetPostedItemByUserId(string userId);
        string GetAzureBaseBlobUrl();
        Task<bool> RemovePostedItem(long id);
        Task<PostedItemViewModel> GetPostedItemViewModelById(long id, string userId = null);
        Task<ICollection<PostedItem>> GetPostedItems(int numberOfPostedItems);

        Task<ICollection<PostedItem>> GetPostedItemsByPageNumber(int pageNumber, int numberOfPostedItems);
    }
}
