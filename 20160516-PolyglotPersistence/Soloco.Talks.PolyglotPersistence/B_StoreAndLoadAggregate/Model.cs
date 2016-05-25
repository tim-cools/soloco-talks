using System;
using System.Collections.Generic;
using Soloco.Talks.PolyglotPersistence.Infrastructure;

namespace Soloco.Talks.PolyglotPersistence.B_StoreAndLoadAggregate
{
    public class Route
    {
        private readonly List<Stop> _stops = new List<Stop>();

        public Guid Id { get; private set; }
        public RouteStatus Status { get; private set; }
        public DateTime Date { get; private set; }

        public IEnumerable<Stop> Stops => _stops;

        public void Plan(DateTime date)
        {
            if (date < Tomorrow())
            {
                throw new InvalidOperationException("Route can only plan from tomorrow.");
            }

            Status = RouteStatus.Planned;
            Date = date;
        }

        private static DateTime Tomorrow()
        {
            return DateTime.Today.AddDays(1);
        }

        public void AddStop(string name, Position position)
        {
            if (Status != RouteStatus.Planned)
            {
                throw new InvalidOperationException("Route should be planned first.");
            }

            _stops.Add(new Stop(name, position));
        }

        public override string ToString()
        {
            return $"ID: {Id}, Status: {Status}, Date: {Date}{Environment.NewLine}{_stops.AsString()}";
        }
    }

    public enum RouteStatus
    {
        Created,
        Planned,
        Driving,
        Stopped
    }

    public class Stop
    {
        public string Name { get; }
        public Position Position { get; }

        public Stop(string name, Position position)
        {
            Name = name;
            Position = position;
        }

        public override string ToString()
        {
            return $"Name: {Name}, Position: {Position}";
        }
    }

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
}