using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class MetaDataViewModel
    {
        public MetaDataViewModel()
        {
            FacebookOpenGraph = new FacebookOpenGraph();
        }

        public FacebookOpenGraph FacebookOpenGraph { get; set; }

        public string MetadataKeywords { get; set; }

        public string Description { get; set; }

        public string ImageSource { get; set; }

        public string CanonicalLink { get; set; }

        public string BaseAddress { get; set; }

        public string VideoSource { get; set; }
    }

    public class FacebookOpenGraph
    {
        public string Type { get; set; }
    }
}
