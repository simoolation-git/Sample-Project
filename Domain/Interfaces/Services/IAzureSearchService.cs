using Domain.Models;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
    public interface IAzureSearchService
    {
        Task<IList<PostedItemViewModel>> Search(string term, int currentPageNumber, string userId, bool skipFirstPage = false, bool onlySpecifiedPageNumber = false, int items = 18);
        Task<PostedItemViewModel> GetSearchResult(string id);
    }
}
