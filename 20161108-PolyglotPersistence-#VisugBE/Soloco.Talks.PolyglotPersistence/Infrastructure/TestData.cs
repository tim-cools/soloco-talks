namespace Soloco.Talks.PolyglotPersistence.Infrastructure
{
    public class Position
    {
        public decimal Latitude { get; }
        public decimal Longitude { get; }

        public Position(decimal longitude, decimal latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public override string ToString()
        {
            return $"Latitude: {Latitude}, Longitude: {Longitude}";
        }
    }

    public static class TestData
    {
        public static Position PositionHome = new Position(51.197894m, 4.481736m);
        public static Position PositionVisug = new Position(51.142128m, 4.43787m);
    }
}