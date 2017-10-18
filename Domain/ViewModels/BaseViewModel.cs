using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Domain.ViewModels
{
    public class BaseViewModel
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}