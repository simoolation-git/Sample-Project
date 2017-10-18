using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Comment : EntityBase
    {
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string Content { get; set; }
    }
}
