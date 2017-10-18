using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class PostedItemViewModel
    {
        private int _totalLike;
        private int _totalDislike;

        public PostedItemViewModel()
        {
            RadialProgress = new RadialProgress();
            IsLiked = null;
        }

        public virtual long Id { get; set; }

        public virtual DateTime? Updated { get; set; }

        public DateTime? Inserted { get; set; }

        public string Title { get; set; }

        public string PhotoUrl { get; set; }

        public string VideoSourceUrl { get; set; }


        private string[] _tags;

        public string[] Tags
        {
            get
            {
                if (TagsTokenized != null && _tags == null)
                {
                    var des = (string[])JsonConvert.DeserializeObject(TagsTokenized.ToString(), typeof(string[]));
                    _tags = des;
                }

                return _tags;
            }
            private set { _tags = value; }
        }

        public string MetadataKeywords { get; set; }

        public dynamic TagsTokenized { get; set; }

        public string ApplicationUserId { get; set; }

        public int TotalLike
        {
            get
            {
                return _totalLike;
            }
            set
            {
                _totalLike = value;
                RadialProgress.TotalLike = _totalLike;
            }
        }

        public int TotalDislike
        {
            get
            {
                return _totalDislike;
            }
            set
            {
                _totalDislike = value;
                RadialProgress.TotalDislike = _totalDislike;
            }
        }

        public bool? IsLiked { get; set; }

        public dynamic OpinionsTokenized { get; set; }

        public string Source { get; set; }

        public string Slug { get; set; }

        public bool IsPostedItemPage { get; set; }

        public RadialProgress RadialProgress { get; set; }
    }



    public class RadialProgress
    {
        private double _percentage;

        public RadialProgress()
        {
            Radius = 14;
        }

        public int TotalLike { get; set; }
        public int TotalDislike { get; set; }

        public int Radius { get; private set; }

        public double Circumference
        {
            get
            {
                return 2 * (double)Radius * Math.PI;
            }
        }

        public double Percentage
        {
            get
            {
                double total = (double)TotalLike + (double)TotalDislike;
                _percentage = (TotalLike == 0 || total == 0) ? 0 : (TotalLike / total);

                if (_percentage == 0)
                    return 0;

                return _percentage;
            }
        }

        public double PercentageDisplay
        {
            get
            {
                if (_percentage == 0)
                    _percentage = Percentage;

                return Math.Round(_percentage * 100);
            }
        }

        public double Offset
        {
            get
            {
                return Circumference * Percentage;
            }
        }

        public string Color
        {
            get
            {
                if (TotalDislike > 0)
                    return "red";

                return "#DEDEDE";
            }
        }
    }
}
