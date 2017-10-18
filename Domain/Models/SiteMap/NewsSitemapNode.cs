using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Domain.Models.SiteMap
{

    public class NewsSitemapNode
    {
        public string Url { get; set; }
        public string PublicationName { get; set; }
        public string Language { get; set; }
        public string Generes { get; set; }
        public DateTime? Publication_date { get; set; }
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string Stock_tickers { get; set; }

        public string CoverPhotoUrl { get; set; }

        public string VideoUrl { get; set; }
    }
}
