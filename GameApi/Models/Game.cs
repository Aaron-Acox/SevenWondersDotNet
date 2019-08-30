using System;

namespace GameApi.Models
{
    public class Game
    {
        public Guid Id { get; set; }
        public GameStatus Status { get; set; }
        
        public byte PlayerCount { get; set; }
    }
}