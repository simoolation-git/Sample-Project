using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Domain.Models
{
    public class PostedItem : EntityBase
    {
        public PostedItem()
        {
            Tags = new List<Tag>();
            Opinions = new List<Opinion>();
        }

        public string Title { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual Photo Photo { get; set; }
        public string PhotoUrl { get; set; }

        public string TagsTokenized { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public PostedItemType PostedItemType { get; set; }

        public string AzureBlobRSSRowKey { get; set; }

        public virtual ICollection<Opinion> Opinions { get; set; }

        public string OpinionsTokenized { get; set; }

        public int TotalLike { get; set; }
        public int TotalDislike { get; set; }

        /// <summary>
        /// This is the source of this item. It can be the rss URL or some other things
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// this is used for creating SEO friendly URL
        /// </summary>
        public string Slug { get; set; }

        public string VideoSourceUrl { get; set; }

        public string VideoSourceName { get; set; }

        public string Summary { get; set; }

        public string YoutubeVideoUrl { get; set; }
    }
}