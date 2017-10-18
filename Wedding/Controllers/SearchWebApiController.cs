using Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Wedding.Controllers
{
    public class SearchWebApiController : ApiController
    {
        private readonly IElasticSearchService _elasticSearchService;

        public SearchWebApiController(IElasticSearchService elasticSearchService)
        {
            _elasticSearchService = elasticSearchService;
        }

        [Route("SearchWebApi/Find/{term}")]
        public IHttpActionResult Find(string term)
        {
            var results = _elasticSearchService.Search(term);

            if (results == null)
            {
                return NotFound();
            }
            return Ok(results);
        }
    }
}
