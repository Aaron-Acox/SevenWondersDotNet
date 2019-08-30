using System;

namespace GameApi.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public bool IsGameOwner { get; set; }
    }
}