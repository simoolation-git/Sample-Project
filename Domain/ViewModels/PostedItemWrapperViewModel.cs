using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class PostedItemWrapperViewModel
    {
        public PostedItemViewModel MainPostedItemViewModel { get; set; }

        public IList<PostedItemViewModel> RelatedPostedItems { get; set; }
    }
}
