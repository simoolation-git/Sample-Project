using Domain.Interfaces.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class CommentTableStorage : ICommentTableStorage
    {
        private CloudTable _table;

        public CommentTableStorage()
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            _table = tableClient.GetTableReference("UserSettings"); //we can have other things under user settings

            _table.CreateIfNotExists();
        }

    }
}
