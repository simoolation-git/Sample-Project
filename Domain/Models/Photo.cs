using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Domain.Models
{
    public enum PhotoType
    {
        PostedItem,
        Avatar
    }

    public class Photo
    {
        public virtual DateTime? Inserted { get; set; }
        public virtual DateTime? Updated { get; set; }

        public string FileExtention { get; set; }
        public string Name { get; set; }
        public string Container { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public PhotoType PhotoType { get; set; }

        [Key, ForeignKey("PostedItem")]
        public long PostedItemId { get; set; }

        public virtual PostedItem PostedItem { get; set; }
    }
}