using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
    public interface IElasticSearchService
    {
        IList<PostedItem> Search(string term);
    }
}
