using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Domain.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private int _postedItemCountPerPage;

        public ConfigurationService()
        {
            _postedItemCountPerPage = default(int);
        }

        public int GetPostedItemCountPerPage()
        {
            if (_postedItemCountPerPage != default(int))
            {
                return _postedItemCountPerPage;
            }

            var value = ConfigurationManager.AppSettings["PostedItemCountPerPage"];

            if (!string.IsNullOrEmpty(value))
            {
                int parsedPostedItemCountPerPage;
                if (int.TryParse(value, out parsedPostedItemCountPerPage))
                {
                    _postedItemCountPerPage = parsedPostedItemCountPerPage;
                }

                return _postedItemCountPerPage;
            }

            //this is default value (if we didn't have the configuration)
            return 5;
        }
    }
}