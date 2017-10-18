using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Domain.ResponseModels
{
    public abstract class ServiceResponseBase
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}