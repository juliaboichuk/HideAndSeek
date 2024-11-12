using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideAndSeek
{
    public class SavedGame
    {
        public string CurrentLocation { get; set; }
        public int MoveNumber { get; set; }
        public Dictionary<string, string> OpponentsWithHidingLocations { get; set; }
        public List<string> FoundOpponents { get; set; }

        public SavedGame() 
        {
            OpponentsWithHidingLocations = new Dictionary<string, string>();
            FoundOpponents = new List<string>();
        }
    }
}
