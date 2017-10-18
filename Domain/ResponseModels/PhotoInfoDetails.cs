using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Domain.ResponseModels
{
    public struct PhotoInfoDetails
    {
        public string ImageFormat { get; set; }
        public string AddedPhotoName { get; set; }
        public string PhotoNameInAzure { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsOriginal { get; set; }
    }
}