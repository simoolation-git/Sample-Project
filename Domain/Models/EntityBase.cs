using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Domain.Models
{
    public abstract class EntityBase
    {
        public virtual long Id { get; set; }
        public virtual DateTime? Inserted { get; set; }
        public virtual DateTime? Updated { get; set; }
        public bool IsDeleted { get; set; }

        public virtual bool IsTransient()
        {
            return Id == default(long);
        }
    }
}