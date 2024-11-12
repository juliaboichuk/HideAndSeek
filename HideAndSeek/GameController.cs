using System.Text.Json;

namespace HideAndSeek
{
    public class GameController
    {
        /// <summary>
        /// The player's current location in the house
        /// </summary>
        public Location CurrentLocation { get; private set; }

        /// <summary>
        /// Returns the the current status to show to the player
        /// </summary>
        public string Status
        {
            get
            {
                var message = $"You are in the {CurrentLocation}. You see the following exits:"
                    + Environment.NewLine
                    + $" - {string.Join(Environment.NewLine + " - ", CurrentLocation.ExitList)}";

                message += CurrentLocation is LocationWithHidingPlace hidingLocation ?
                    Environment.NewLine + $"Someone could hide {hidingLocation.HidingPlace}" : "";

                if (foundOpponents.Count() > 0)
                    return message += Environment.NewLine
                        + $"You have found {foundOpponents.Count()} of 5 opponents: {string.Join(", ", foundOpponents)}";
                return message += Environment.NewLine + "You have not found any opponents";
            }
        }

        /// <summary>
        /// The number of moves the player has made
        /// </summary>
        public int MoveNumber { get; private set; } = 1;

        /// <summary>
        /// Private list of opponents the player needs to find
        /// </summary>
        public readonly IEnumerable<Opponent> Opponents = new List<Opponent>()
        {
            new Opponent("Joe"),
            new Opponent("Bob"),
            new Opponent("Ana"),
            new Opponent("Owen"),
            new Opponent("Jimmy"),
        };

        /// <summary>
        /// Private list of opponents the player has found so far
        /// </summary>
        private readonly List<Opponent> foundOpponents = new List<Opponent>();

        /// <summary>
        /// A Dictionary to keep track of the opponent locations
        /// </summary>
        private Dictionary<string, string> opponentLocations = new Dictionary<string, string>();


        /// <summary>
        /// Returns true if the game is over
        /// </summary>
        public bool GameOver => Opponents.Count() == foundOpponents.Count();

        /// <summary>
        /// A prompt to display to the player
        /// </summary>
        public string Prompt => $"{MoveNumber}: Which direction do you want to go (or type 'check'): ";

        public GameController()
        {
            House.ClearHidingPlaces();
            foreach (var opponent in Opponents)
                opponentLocations.Add(opponent.Name, opponent.Hide().Name);
            CurrentLocation = House.Entry;
        }

        /// <summary>
        /// Move to the location in a direction
        /// </summary>
        /// <param name="direction">The direction to move</param>
        /// <returns>True if the player can move in that direction, false oterwise</returns>
        public bool Move(Direction direction)
        {
            var hasExit = CurrentLocation.Exits.ContainsKey(direction) ? true : false;
            CurrentLocation = CurrentLocation.GetExit(direction);
            return hasExit;

        }

        /// <summary>
        /// Parses input from the player and updates the status
        /// </summary>
        /// <param name="input">Input to parse</param>
        /// <returns>The results of parsing the input</returns>
        public string ParseInput(string input)
        {
            var result = "That's not a valid direction";

            if (input.ToLower().Equals("check"))
            {
                result = CheckHidingLocation();
            }
            else if (Enum.TryParse(typeof(Direction), input, out var direction))
            {
                if (!Move((Direction)direction))
                    result = "There's no exit in that direction";
                else
                {
                    MoveNumber++;
                    result = $"Moving {direction}";
                }
            }
            else if (input.ToLower().Trim().StartsWith("save"))
            {
                var filename = input.Split(' ').Last();
                result = SaveGameStateToFile(filename);
            }
            else if (input.ToLower().Trim().StartsWith("load"))
            {
                var filename = input.Split(' ').Last();
                result = LoadGameStateFromFile(filename);
            }
            return result;
        }

        /// <summary>
        /// Check location for hiding opponents
        /// </summary>
        /// <returns>Return message with count of founded opponents</returns>
        private string CheckHidingLocation()
        {
            string result;
            MoveNumber++;
            if (CurrentLocation is LocationWithHidingPlace hidingLocation)
            {
                var hiddenOpponents = hidingLocation.CheckHidingPlace();
                if (hiddenOpponents.Count() == 0)
                    result = $"Nobody was hiding {hidingLocation.HidingPlace}";
                else
                {
                    foundOpponents.AddRange(hiddenOpponents);
                    var s = hiddenOpponents.Count() > 1 ? "s" : "";
                    result = $"You found {hiddenOpponents.Count()} opponent{s} hiding {hidingLocation.HidingPlace}";
                }
            }
            else
            {
                result = $"There is no hiding place in the {CurrentLocation}";
            }

            return result;
        }

        /// <summary>
        /// Save a game to a file
        /// </summary>
        /// <param name="filename">Name of the file (without extension)</param>
        /// <returns>Results of the save to display to the player</returns>
        private string SaveGameStateToFile(string filename)
        {
            if (filename.Contains("/") || filename.Contains("\\") || filename.Contains(" "))
                return "Please enter a filename without slashes or spaces.";
            else
            {
                var savedGame = new SavedGame()
                {
                    CurrentLocation = CurrentLocation.Name,
                    OpponentsWithHidingLocations = opponentLocations,
                    FoundOpponents = foundOpponents.Select(opponent => opponent.Name).ToList(),
                    MoveNumber = this.MoveNumber,
                };

                var json = JsonSerializer.Serialize<SavedGame>(savedGame);
                File.WriteAllText($"{filename}.json", json);
                return $"Saved current game to {filename}";
            }
        }

        /// <summary>
        /// Load a game from a file
        /// </summary>
        /// <param name="filename">Name of the file (without extension)</param>
        /// <returns>Results of the save to display to the player</returns>
        private string LoadGameStateFromFile(string filename)
        {
            if (filename.Contains("/") || filename.Contains("\\") || filename.Contains(" "))
                return "Please enter a filename without slashes or spaces.";
            else if (!File.Exists($"{filename}.json"))
                return "That save file does not exist.";
            else
            {
                string savedGameFromFile = File.ReadAllText($"{filename}.json");
                var savedGame = JsonSerializer.Deserialize<SavedGame>(savedGameFromFile);
                CurrentLocation = House.GetLocationByName(savedGame.CurrentLocation);
                MoveNumber = savedGame.MoveNumber;

                House.ClearHidingPlaces();
                foreach (var opponentName in savedGame.OpponentsWithHidingLocations.Keys)
                {
                    var opponent = new Opponent(opponentName);
                    var locationName = savedGame.OpponentsWithHidingLocations[opponentName];
                    if (House.GetLocationByName(locationName) is LocationWithHidingPlace location)
                        location.Hide(opponent);
                }
                foundOpponents.Clear();
                foundOpponents.AddRange(savedGame.FoundOpponents.Select(name => new Opponent(name)));
                return $"Loaded game from {filename}";
            }
        }
    }
}
