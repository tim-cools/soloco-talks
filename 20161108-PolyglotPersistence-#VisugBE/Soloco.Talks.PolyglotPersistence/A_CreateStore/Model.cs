using System;
using System.Collections.Generic;

namespace Soloco.Talks.PolyglotPersistence.A_CreateStore
{
    public class Route
    {
        public Guid Id { get; set; }
        public RouteStatus Status { get; set; }
        public DateTime Date { get; set; }

        public List<Stop> Stops { get; set; } = new List<Stop>();
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
        public string Name { get; set; }
        public Position Position { get; set; }
    }

    public class Position
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}