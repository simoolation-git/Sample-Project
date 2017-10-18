using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Domain.RequestModels
{
    public struct PhotoDimensionInfo
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsOriginal { get; set; }

        /// <summary>
        /// If set true the system will try to resize the image using the given width and height
        /// If set false it will reszie to width only
        /// </summary>
        public bool ReSizeAndCrop { get; set; }
    }
}