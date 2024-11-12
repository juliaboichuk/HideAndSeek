using HideAndSeek;
using System.Text.Json;

namespace HideAndSeelTests
{
    [TestClass]
    public class SavedGameTests
    {
        GameController gameController;

        [TestInitialize]
        public void Initialize()
        {
            gameController = new GameController();
        }

        [TestMethod]
        public void TestSavingToFile()
        {
            var filename = "my_saved_game";
            Assert.IsFalse(gameController.GameOver);
            // Clear the hiding places and hide the opponents in specific rooms
            House.ClearHidingPlaces();
            var joe = gameController.Opponents.ToList()[0];
            (House.GetLocationByName("Garage") as LocationWithHidingPlace).Hide(joe);
            var bob = gameController.Opponents.ToList()[1];
            (House.GetLocationByName("Kitchen") as LocationWithHidingPlace).Hide(bob);
            var ana = gameController.Opponents.ToList()[2];
            (House.GetLocationByName("Attic") as LocationWithHidingPlace).Hide(ana);
            var owen = gameController.Opponents.ToList()[3];
            (House.GetLocationByName("Attic") as LocationWithHidingPlace).Hide(owen);
            var jimmy = gameController.Opponents.ToList()[4];
            (House.GetLocationByName("Kitchen") as LocationWithHidingPlace).Hide(jimmy);
            Assert.AreEqual(5, gameController.Opponents.Count());
            // Play game
            gameController.ParseInput("Check");
            gameController.ParseInput("Out");
            gameController.ParseInput("check");
            gameController.ParseInput("In");
            gameController.ParseInput("East");
            gameController.ParseInput("North");
            gameController.ParseInput("South");
            gameController.ParseInput("Fail");
            gameController.ParseInput("Northwest");
            gameController.ParseInput("check");

            Assert.AreEqual(10, gameController.MoveNumber);
            Assert.AreEqual("Kitchen", gameController.CurrentLocation.Name);
            Assert.AreEqual("You are in the Kitchen. You see the following exits:" +
                Environment.NewLine + " - the Hallway is to the Southeast" +
                Environment.NewLine + "Someone could hide next to the stove" +
                Environment.NewLine + "You have found 3 of 5 opponents: Joe, Bob, Jimmy",
                gameController.Status);
            Assert.AreEqual($"Saved current game to {filename}",
                gameController.ParseInput($"save {filename}"));

            string savedGameFromFile = File.ReadAllText($"{filename}.json");
            Assert.IsTrue(File.Exists($"{filename}.json"));
            File.Delete($"{filename}.json");
            Assert.IsTrue(!File.Exists($"{filename}.json"));
            SavedGame savedGame = JsonSerializer.Deserialize<SavedGame>(savedGameFromFile);
            Assert.AreEqual("Kitchen", savedGame.CurrentLocation);
            Assert.AreEqual(10, savedGame.MoveNumber);
            CollectionAssert.AreEqual(new List<string> { "Joe", "Bob", "Jimmy" }, savedGame.FoundOpponents);
        }

        [TestMethod]
        public void TestLoadFromFile()
        {
            var filename = "my_saved_game";
            Assert.AreEqual(1, gameController.MoveNumber);
            Assert.AreEqual("Entry", gameController.CurrentLocation.Name);
            Assert.AreEqual("You are in the Entry. You see the following exits:" +
                Environment.NewLine + " - the Hallway is to the East" +
                Environment.NewLine + " - the Garage is Out" +
                Environment.NewLine + "You have not found any opponents",
                gameController.Status);
            // create and load file
            string textToSave = "{"
                + "\"CurrentLocation\":\"Kitchen\","
                + "\"MoveNumber\":10,"
                + "\"OpponentsWithHidingLocations\":{\"Joe\":\"Garage\",\"Bob\":\"Kitchen\","
                + "\"Ana\":\"Attic\",\"Owen\":\"Attic\",\"Jimmy\":\"Kitchen\"},"
                + "\"FoundOpponents\":[\"Joe\",\"Bob\",\"Jimmy\"]"+
                "}";
            File.WriteAllText($"{filename}.json", textToSave);

            Assert.AreEqual($"Loaded game from {filename}",
                gameController.ParseInput($"load {filename}"));
            Assert.AreEqual(10, gameController.MoveNumber);
            Assert.AreEqual("Kitchen", gameController.CurrentLocation.Name);

            var attic = House.GetLocationByName("Attic") as LocationWithHidingPlace;
            var hidingOpponents = attic.CheckHidingPlace().ToList();
            Assert.AreEqual("You are in the Kitchen. You see the following exits:" +
                Environment.NewLine + " - the Hallway is to the Southeast" +
                Environment.NewLine + "Someone could hide next to the stove" +
                Environment.NewLine + "You have found 3 of 5 opponents: Joe, Bob, Jimmy",
                gameController.Status);
            CollectionAssert.AreEqual(
                new List<Opponent> { new Opponent("Ana"), new Opponent("Owen") },
                hidingOpponents);
            File.Delete($"{filename}.json");
            Assert.IsTrue(!File.Exists($"{filename}.json"));
        }
    }
}
