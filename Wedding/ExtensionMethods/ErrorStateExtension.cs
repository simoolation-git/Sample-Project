using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wedding.ExtensionMethods
{
    public static class ErrorStateExtension
    {
        public static dynamic GetErrorsForJson(this ModelStateDictionary modelStateDictionary)
        {
            var result = new ExpandoObject() as IDictionary<string, Object>;

            foreach (var item in modelStateDictionary)
            {
                if (item.Value.Errors.Count > 0)
                    result.Add(item.Key, item.Value.Errors[0].ErrorMessage);
            }
            return result;
        }
    }
}