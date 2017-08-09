using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace core.Services
{
    public abstract class AzureTableService<T>
        where T : TableEntity, new()
    {
        protected CloudTable Table { get; set; }
        
        protected AzureTableService(CloudTable table)
        {
            Table = table;
        }
        
        // create if not exists
        public async Task Create()
        {
            // Create the CloudTable if it does not exist
            await Table.CreateIfNotExistsAsync();
        }
        
        // add entity
        public async Task Add(T entity)
        {
            // create table if not exits
            await Create();
            
            // Create the TableOperation that inserts the entity.
            TableOperation insertOperation = TableOperation.Insert(entity);

            // Execute the insert operation.
            await Table.ExecuteAsync(insertOperation);
        }
        
        // function to get all entities of type 
        public async Task<IEnumerable<T>> GetAll()
        {
            // create table if not exits
            await Create();
            
            // Construct the query operation for all entities 
            TableQuery<T> query = new TableQuery<T>();
            
            // define return list
            var entities = new List<T>();
            
            // get all.
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<T> resultSegment = await Table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;
                
                // add entities to return list
                entities.AddRange(resultSegment.Results);
                
            } while (token != null);
            
            // return results
            return entities;
        }
        
        // get single entity
        public async Task<T> GetSingle(string partitionKey, string rowKey)
        {
            // create table if not exits
            await Create();
            
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            // Execute the retrieve operation.
            TableResult retrievedResult = await Table.ExecuteAsync(retrieveOperation);
            
            // return entity
            return (T)retrievedResult.Result;
        }
        
        // delete entity
        public async Task DeleteSingle(string partitionKey, string rowKey)
        {
            // create table if not exits
            await Create();
            
            // Create a retrieve operation that expects a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            // Execute the operation.
            TableResult retrievedResult = await Table.ExecuteAsync(retrieveOperation);

            // Assign the result to a entity object.
            var deleteEntity = (T)retrievedResult.Result;

            // Create the Delete TableOperation and then execute it.
            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                // Execute the operation.
                await Table.ExecuteAsync(deleteOperation);
            }
        }
    }
}