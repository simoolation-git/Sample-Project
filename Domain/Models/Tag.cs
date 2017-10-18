using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Domain.Models
{
    public class Tag : EntityBase
    {
        public string Name { get; set; }
        public virtual ICollection<PostedItem> PostedItems { get; set; }
    }
}