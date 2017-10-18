using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponseModels
{
    public class SavePostedItemResponse : ServiceResponseBase
    {
        public PostedItem PostedItem { get; set; }
    }
}
