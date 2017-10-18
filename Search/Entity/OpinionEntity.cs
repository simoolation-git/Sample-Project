using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wedding.Search
{
    public class OpinionEntity : TableEntity
    {
        public OpinionEntity(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        public OpinionEntity() { }

        public string OpinionsJson { get; set; }

        public DateTime Updated { get; set; }

        public DateTime Inserted { get; set; }
    }
}