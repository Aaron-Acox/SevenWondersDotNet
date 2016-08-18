using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SevenWonders.Models
{
    public class PlayerGame
    {
        public long PlayerId { get; set; }
        public Player Player { get; set; }
        public long GameId { get; set; }
        public Game Game { get; set; }
    }
}
