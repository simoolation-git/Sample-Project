using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface ISearchTermRepository
    {
        Task<int> AddNewSearchedTerm(SearchedTerm searchedTerm);
        Task<SearchedTerm> FindSearchedTerm(string term);

        Task UpdateSearchedTerm(SearchedTerm searchedTerm);

    }
}
