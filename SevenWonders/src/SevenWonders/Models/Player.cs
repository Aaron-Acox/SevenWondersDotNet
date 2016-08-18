using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SevenWonders.Models
{
    public class Player
    {
        public long PlayerId { get; set; }
        public ApplicationUser User { get; set; }
        
        public List<PlayerGame> PlayerGames { get; set; } 
    }
}
