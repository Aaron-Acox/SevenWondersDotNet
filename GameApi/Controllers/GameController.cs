using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameApi.Models;
using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace GameApi.Controllers
{
    public class GameController : ControllerBase
    {
        private AmazonDynamoDBConfig config = new AmazonDynamoDBConfig {ServiceURL = "http://localhost:8000"};
        
        // GET
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpGet("api/game")]
        public async Task<IActionResult> Get()
        {
            var client = new AmazonDynamoDBClient(config);
            var request = new ScanRequest
            {
                TableName = "game"
            };
            var response = await client.ScanAsync(request);
            var games = response.Items.Select(i => new Game{Id = Guid.Parse(i["id"].S)});
            return Ok(games);
        }

        [HttpPost("api/game")]
        public async Task<IActionResult> Post()
        {
            var client = new AmazonDynamoDBClient(config);
            var id = Guid.NewGuid();
            var request = new PutItemRequest
            {
                TableName = "game",
                Item = new Dictionary<string, AttributeValue>
                {
                    {"id", new AttributeValue{S = id.ToString()}}
                }
            };
            var response = await client.PutItemAsync(request);
            return Created($"api/{id}", new Game {Id = id});
        }
    }
}