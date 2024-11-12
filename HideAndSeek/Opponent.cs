namespace HideAndSeek
{
    public class Opponent
    {
        public readonly string Name;

        public Opponent(string name) => Name = name;

        public Location Hide()
        {
            var currentLocation = House.Entry;
            var locationNumber = House.Random.Next(10, 50);
            for (int i = 0; i < locationNumber; i++) 
            {
                currentLocation = House.RandomExit(currentLocation);
            }
            while (true)
            {
                if (currentLocation is LocationWithHidingPlace hidinglocation)
                {
                    hidinglocation.Hide(this);
                    return currentLocation;
                }
                currentLocation = House.RandomExit(currentLocation);
            }
        }

        public override string ToString() => Name;

        public override bool Equals(object? obj)
        {
            if (!(obj is Opponent)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var opponent = obj as Opponent;
            return opponent.Name.Equals(Name);
        }
    }
}
