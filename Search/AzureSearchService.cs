using Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Domain.ViewModels;
using System.Configuration;
using Newtonsoft.Json;
using Domain.Interfaces.Repositories;
using Domain.Services;

namespace Search
{
    public class AzureSearchService : IAzureSearchService
    {
        private const string _searchServiceName = "zenzoy";
        private readonly SearchServiceClient _serviceClient;
        private readonly SearchIndexClient _indexClient;
        private int _numberOfSearchResultsPerPage;
        private IOpinionsTableStorage _opinionsTableStorage;
        private ISearchTermRepository _searchTermRepository;
        private IConfigurationService _configurationService;

        public AzureSearchService(IOpinionsTableStorage opinionsTableStorage, ISearchTermRepository searchTermRepository, IConfigurationService configurationService)
        {
            _opinionsTableStorage = opinionsTableStorage;
            _searchTermRepository = searchTermRepository;
            _configurationService = configurationService;

            var apiKey = ConfigurationManager.AppSettings["AzureSearchPrimaryAdminKey"];
            var indexName = ConfigurationManager.AppSettings["PostedItemSearchIndexName"];

            _serviceClient = new SearchServiceClient(_searchServiceName, new SearchCredentials(apiKey));
            _indexClient = _serviceClient.Indexes.GetClient(indexName);
        }


        public async Task<IList<PostedItemViewModel>> Search(string term, int currentPageNumber, string userId, bool skipFirstPage = false, bool onlySpecifiedPageNumber = false, int items = 18)
        {
            //reset this, since this class is singleton once this value is changed to 4 (for slug page) it stays 4. so we have to reset it to 18 every time.
            _numberOfSearchResultsPerPage = _configurationService.GetPostedItemCountPerPage();

            var postedItems = await SearchDocuments(term, currentPageNumber, userId, skipFirstPage, onlySpecifiedPageNumber, items);

            await AddUpdateSearchedTerm(term);

            return postedItems;
        }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task AddUpdateSearchedTerm(string term)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
#if DEBUG
            //if (!string.IsNullOrEmpty(term))
            //{

            //    var existingSearchTerm = await _searchTermRepository.FindSearchedTerm(term.Trim());

            //    if (existingSearchTerm == null)
            //    {
            //        await _searchTermRepository.AddNewSearchedTerm(new SearchedTerm { Term = term });
            //    }
            //    else
            //    {
            //        existingSearchTerm.Count++;
            //        await _searchTermRepository.UpdateSearchedTerm(existingSearchTerm);
            //    }
            //}
#else
            if (!string.IsNullOrEmpty(term))
            {
                
                var existingSearchTerm = await _searchTermRepository.FindSearchedTerm(term.Trim());

                if (existingSearchTerm == null)
                {
                    await _searchTermRepository.AddNewSearchedTerm(new SearchedTerm { Term = term });
                }
                else
                {
                    existingSearchTerm.Count++;
                    await _searchTermRepository.UpdateSearchedTerm(existingSearchTerm);
                }
            }
#endif
        }


        private async Task<List<PostedItemViewModel>> SearchDocuments(string searchText, int currentPageNumber, string userId, bool skipFirstPage = false, bool onlySpecifiedPageNumber = false, int items = 18, string filter = null, IList<string> facets = null)
        {
            // Execute search based on search text and optional filter
            var sp = new SearchParameters();

            if (string.IsNullOrEmpty(searchText) && skipFirstPage)
                sp.Skip = _numberOfSearchResultsPerPage;
            else if (onlySpecifiedPageNumber)
            {
                if (items != _numberOfSearchResultsPerPage)
                    _numberOfSearchResultsPerPage = items;

                if (currentPageNumber > 1)
                    sp.Skip = (currentPageNumber - 1) * _numberOfSearchResultsPerPage;
            }

            if (!onlySpecifiedPageNumber)
                sp.Top = currentPageNumber * _numberOfSearchResultsPerPage;
            else
                sp.Top = _numberOfSearchResultsPerPage;

            if (!String.IsNullOrEmpty(filter))
            {
                sp.Filter = filter;
            }

            if (facets != null)
            {
                sp.Facets = facets;
            }

            if (String.IsNullOrEmpty(searchText))
                sp.OrderBy = new List<string> { "Inserted desc" };

            DocumentSearchResult<PostedItemViewModel> response = await _indexClient.Documents.SearchAsync<PostedItemViewModel>(searchText, sp);

            var existingOpinions = await _opinionsTableStorage.GetOpinion(userId);

            return response.Results.Select(r => GetPostedItemViewModel(r.Document, userId, existingOpinions)).ToList();
        }

        private PostedItemViewModel GetPostedItemViewModel(PostedItemViewModel viewModel, string userId, Dictionary<long, int> existingOpinions)
        {
            Dictionary<string, int> opinions = null;

            if (!String.IsNullOrEmpty(userId) && !String.IsNullOrEmpty(viewModel.OpinionsTokenized))
                opinions = JsonConvert.DeserializeObject<Dictionary<string, int>>(viewModel.OpinionsTokenized);

            if (existingOpinions != null && existingOpinions.ContainsKey(viewModel.Id))
            {
                if (opinions == null)
                    opinions = new Dictionary<string, int>();

                //lets update the old opinion with temporary one
                var existingOpinion = existingOpinions[viewModel.Id];

                if (opinions.Count > 0)
                    opinions[userId] = existingOpinion;

                int totalLike = 0;
                int totalDislike = 0;

                for (int index = 0; index < opinions.Count(); index++)
                {
                    var opinionType = opinions.ElementAt(index);

                    if (opinionType.Value == 1)
                        totalLike++;
                    else if (opinionType.Value == 2)
                        totalDislike++;
                }

                viewModel.TotalLike = totalLike;
                viewModel.TotalDislike = totalDislike;

                switch (existingOpinion)
                {
                    case 1:
                        viewModel.IsLiked = true;
                        break;
                    case 2:
                        viewModel.IsLiked = false;
                        break;
                    case -1:
                        viewModel.IsLiked = null;
                        break;
                }
            }
            else if (opinions != null)
            {
                //here we figure out if user like/dislike a news item or not. if IsLiked is null it means user has no opinion
                if (opinions.ContainsKey(userId))
                    viewModel.IsLiked = opinions[userId] == 1 ? true : false;
            }

            return viewModel;
        }

        public async Task<PostedItemViewModel> GetSearchResult(string id)
        {
            var sp = new SearchParameters();
            sp.SearchMode = SearchMode.All;
            sp.QueryType = QueryType.Full;
            sp.SearchFields = new[] { "id" };

            DocumentSearchResult<PostedItemViewModel> response = await _indexClient.Documents.SearchAsync<PostedItemViewModel>(id);

            var result = response.Results.FirstOrDefault();

            if (result != null)
                return result.Document;

            return null;

        }
    }

}
