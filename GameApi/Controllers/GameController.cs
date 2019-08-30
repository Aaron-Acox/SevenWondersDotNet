using System;
using System.Collections;
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
        public async Task<IActionResult> Get([FromQuery]GameStatus? status)
        {
            IEnumerable<Game> games;
            if (status.HasValue)
            {
                var request = new QueryRequest
                {
                    TableName = "game",
                    IndexName = "statusIndex",
                    KeyConditionExpression = "#gameStatus = :v_status",
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        {"#gameStatus", "status"}
                    },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {":v_status", new AttributeValue {S= status.ToString()}} 
                    },
                    ProjectionExpression = "id, #gameStatus, playerCount"
                };
                var response = await Client.QueryAsync(request);
                games = response.Items.Select(i => new Game
                {
                    Id = Guid.Parse(i["id"].S),
                    Status = Enum.Parse<GameStatus>(i["status"].S),
                    PlayerCount = Byte.Parse(i["playerCount"].N)
                });
            }
            else
            {
                var request = new ScanRequest
                {
                    TableName = "game"
                };
                var response = await Client.ScanAsync(request);
                games = response.Items.Select(i => new Game
                {
                    Id = Guid.Parse(i["id"].S),
                    Status = Enum.Parse<GameStatus>(i["status"].S),
                    PlayerCount = Byte.Parse(i["playerCount"].N)
                });
            }
           
            return Ok(games);
        }

        [HttpPost("api/game")]
        public async Task<IActionResult> Post([FromBody]Game game)
        {
            if (game.PlayerCount < 3 || game.PlayerCount > 7)
            {
                return BadRequest();
            }
            var id = Guid.NewGuid();
            var status = GameStatus.WaitingForPlayers;
            var request = new PutItemRequest
            {
                TableName = "game",
                Item = new Dictionary<string, AttributeValue>
                {
                    {"id", new AttributeValue{S = id.ToString()}},
                    {"status", new AttributeValue{S = status.ToString()}},
                    {"playerCount", new AttributeValue {N = game.PlayerCount.ToString() }}
                }
            };
            var response = await Client.PutItemAsync(request);
            return Created($"api/{id}", new Game {Id = id, Status = status});
        }
    }
}