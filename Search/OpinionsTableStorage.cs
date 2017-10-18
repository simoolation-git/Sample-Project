using Domain.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Wedding.Search
{
    public class OpinionsTableStorage : IOpinionsTableStorage
    {
        private CloudTable _table;
        private const string _partitionKey = "UserOpinions";

        public OpinionsTableStorage()
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            _table = tableClient.GetTableReference("UserSettings"); //we can have other things under user settings

            _table.CreateIfNotExists();
        }

        public async Task UpdateOpinion(string userId, Dictionary<long, int> opinions)
        {
            if (String.IsNullOrEmpty(userId))
                return;

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<OpinionEntity>(_partitionKey, userId);

            // Execute the operation.
            TableResult retrievedResult = await _table.ExecuteAsync(retrieveOperation);

            // Assign the result to a CustomerEntity object.
            var updateEntity = (OpinionEntity)retrievedResult.Result;

            var now = DateTime.UtcNow;

            if (updateEntity != null)
            {
                // Change the phone number.
                updateEntity.Updated = now;
                updateEntity.OpinionsJson = (opinions == null) ? String.Empty : JsonConvert.SerializeObject(opinions);

                // Create the Replace TableOperation.
                var updateOperation = TableOperation.Replace(updateEntity);

                // Execute the operation.
                await _table.ExecuteAsync(updateOperation);
            }
            else
                await InsertEntity(userId, now, opinions);
        }

        private async Task InsertEntity(string userId, DateTime now, Dictionary<long, int> opinions)
        {
            var entity = new OpinionEntity(_partitionKey, userId);
            entity.OpinionsJson = (opinions == null) ? String.Empty : JsonConvert.SerializeObject(opinions);
            entity.Inserted = now;
            entity.Updated = now;

            var insertOperation = TableOperation.Insert(entity);

            await _table.ExecuteAsync(insertOperation);
        }

        public async Task<Dictionary<long, int>> GetOpinion(string userId)
        {
            if (String.IsNullOrEmpty(userId))
                return new Dictionary<long, int>();

            var retrieveOperation = TableOperation.Retrieve<OpinionEntity>(_partitionKey, userId);

            // Execute the operation.
            TableResult retrievedResult = await _table.ExecuteAsync(retrieveOperation);

            var entity = retrievedResult.Result as OpinionEntity;

            if (entity == null || String.IsNullOrEmpty(entity.OpinionsJson))
                return new Dictionary<long, int>();

            TimeSpan span = DateTime.UtcNow.Subtract(entity.Updated);

            //the search is updating every 5 minutes so we will only have to keep the cache for 5 minutes otherwise lets get rid of it
            if (span.Minutes > 5)
            {
                await RemoveOpinion(entity);
                return new Dictionary<long, int>();
            }

            return JsonConvert.DeserializeObject<Dictionary<long, int>>(entity.OpinionsJson);
        }


        private async Task UpdateOpinion(OpinionEntity updateEntity, string opinionsJson)
        {
            // Change the phone number.
            updateEntity.Updated = DateTime.UtcNow;
            updateEntity.OpinionsJson = opinionsJson;

            // Create the Replace TableOperation.
            var updateOperation = TableOperation.Replace(updateEntity);

            // Execute the operation.
            await _table.ExecuteAsync(updateOperation);
        }

        private async Task RemoveOpinion(OpinionEntity deleteEntity)
        {
            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                // Execute the operation.
                await _table.ExecuteAsync(deleteOperation);
            }
        }

    }
}