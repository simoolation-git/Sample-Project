using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wedding.ExtensionMethods
{
    public static class ControllerExtensions
    {
        public static bool IsJsonRequest(this Controller controller)
        {
            foreach (string a in controller.Request.AcceptTypes)
                if (a.ToLower().Contains("json"))
                    return true;
            return false;
        }
        public static bool IsAjaxRequest(this Controller controller)
        {
            return controller.Request.IsAjaxRequest();
        }
    }
}