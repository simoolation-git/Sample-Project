//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Nest;
//using Wedding.Repository;
//using Newtonsoft.Json;
//using Domain.Models;
//using Domain.Services.Interfaces;

//namespace Wedding.Services
//{
//    public class ElasticSearchService : IElasticSearchService
//    {
//        private ElasticClient _elasticClient;

//        public ElasticSearchService()
//        {
//            var node = new Uri("http://localhost:9200");

//            var settings = new ConnectionSettings(node).BasicAuthentication("wedding", "green5296");

//            _elasticClient = new ElasticClient(settings);

//            _elasticClient.Flush(Indices.All);

//            //var context = new ApplicationContext();
            
//            //foreach (var item in context.PostedItems)
//            //{
//            //    _elasticClient.Index<PostedItem>(item, i => i
//            //        .Index("posteditem-index")
//            //        //.Type("PostedItem")
//            //        //.Id("title")
//            //        .Refresh()
//            //    //.Ttl("1m")
//            //    );
//            //}
//        }

//        public IList<PostedItem> Search(string term)
//        {
//            if (String.IsNullOrEmpty(term))
//                return new List<PostedItem>();

          

//            // var results = _elasticClient.Search<PostedItem>(t => t.From(0).Size(10).Query(a => a.Term(b => b.Title, term)));

//            //var results = _elasticClient.Search<PostedItem>(s => s.Index("posteditem-index").Query(a => a.Term(b => b.Title, term)));

//           var results = _elasticClient.Search<PostedItem>(s => s.From(0).Size(5).Query(q => q.QueryString(d => d.Query(term))));

//            //results.Hits;

//            return results.Documents.ToList();
//        }
//    }
//}