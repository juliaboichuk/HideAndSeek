using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideAndSeek
{
    public class LocationWithHidingPlace : Location
    {
        /// <summary>
        /// The name of the hiding place in this location
        /// </summary>
        public readonly string HidingPlace;

        /// <summary>
        /// The opponents hidden in this location's hiding place
        /// </summary>
        private List<Opponent> hiddenOpponents;

        /// <summary>
        /// Constructor that sets the location name and hiding place name
        /// </summary>
        public LocationWithHidingPlace(string name, string hidingPlace) : base(name)
        {
            HidingPlace = hidingPlace;
            hiddenOpponents = new List<Opponent>();
        }

        /// <summary>
        /// Hides an opponent in the hiding place
        /// </summary>
        /// <param name="opponent">Opponent to hide</param>
        public void Hide(Opponent opponent) => hiddenOpponents.Add(opponent);
        
        public IEnumerable<Opponent> CheckHidingPlace()
        {
            var foundOpponents = new List<Opponent>(hiddenOpponents);
            hiddenOpponents.Clear();
            return foundOpponents;
        }
    }
}
