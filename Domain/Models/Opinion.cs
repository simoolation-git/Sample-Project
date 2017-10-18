using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public enum OpinionType
    {
        NoOpinion = -1,
        Like = 1,
        Dislike = 2
    }

    public class Opinion : EntityBase
    {
        public OpinionType OpinionType { get; set; }

        public virtual PostedItem PostedItem { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
