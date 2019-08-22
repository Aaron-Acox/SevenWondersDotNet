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
        private AmazonDynamoDBConfig Config => new AmazonDynamoDBConfig {ServiceURL = "http://localhost:8000"};

        private IAmazonDynamoDB Client => new AmazonDynamoDBClient(Config);

        // GET
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpGet("api/game")]
        public async Task<IActionResult> Get()
        {
            var request = new ScanRequest
            {
                TableName = "game"
            };
            var response = await Client.ScanAsync(request);
            var games = response.Items.Select(i => new Game
            {
                Id = Guid.Parse(i["id"].S),
                Status = Enum.Parse<GameStatus>(i["status"].S)
            });
            return Ok(games);
        }

        [HttpPost("api/game")]
        public async Task<IActionResult> Post()
        {
            var id = Guid.NewGuid();
            var status = GameStatus.WaitingForPlayers;
            var request = new PutItemRequest
            {
                TableName = "game",
                Item = new Dictionary<string, AttributeValue>
                {
                    {"id", new AttributeValue{S = id.ToString()}},
                    {"status", new AttributeValue{S = status.ToString()}}
                }
            };
            var response = await Client.PutItemAsync(request);
            return Created($"api/{id}", new Game {Id = id, Status = status});
        }
    }
}