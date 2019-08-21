﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DbSetup
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new AmazonDynamoDBConfig {ServiceURL = "http://localhost:8000"};
            var client = new AmazonDynamoDBClient(config);

            var tables = await client.ListTablesAsync();
            if (tables.TableNames.Contains("game"))
            {
                var deleteRequest = new DeleteTableRequest("game");
                await client.DeleteTableAsync(deleteRequest);
            }
            
            var request = new CreateTableRequest
            {
                TableName = "game",
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement("id", KeyType.HASH)
                },
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition("id", ScalarAttributeType.S)
                },
                ProvisionedThroughput = new ProvisionedThroughput(5, 5)
            };
            var result = await  client.CreateTableAsync(request);
            Console.WriteLine(result);
        }
    }
}