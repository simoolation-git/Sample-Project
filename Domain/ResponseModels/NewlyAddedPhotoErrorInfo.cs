using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Domain.ResponseModels
{
    public struct NewlyAddedPhotoErrorInfo
    {
        public string ErrorMessage { get; set; }
        public bool IsValid { get; set; }
    }
}