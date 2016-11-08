using System;
using System.Collections.Generic;

namespace Soloco.Talks.PolyglotPersistence.I_EventSourcingWithProjections
{
    public class RouteDetails
    {
        public Guid Id { get; set; }
        public DateTime PlannedDate { get; set; }
        public DateTime PlannedSince { get; set; }
        public DateTime DrivingDate { get; set; }

        public string Status { get; set; }

        public IList<RouteStopDetails> Stops { get; } = new List<RouteStopDetails>();
        public Area StopsArea { get; set; }
        public RouteStopDetails To { get; set; }
        public RouteStopDetails From { get; set; }
    }

    public class RouteStopDetails
    {
        public Guid StopId { get; set; }
        public StopType Type { get; set; }
        public string Name { get; set; }
        public Position Position { get; set; }
    }

    public enum StopType
    {
        Source,
        Stop,
        Destination
    }

    public class Area
    {
        public decimal MinLatitude { get; }
        public decimal MinLongitude { get; }
        public decimal MaxLatitude { get; }
        public decimal MaxLongitude { get; }

        public Area(decimal minLatitude, decimal minLongitude, decimal maxLatitude, decimal maxLongitude)
        {
            MinLatitude = minLatitude;
            MinLongitude = minLongitude;
            MaxLatitude = maxLatitude;
            MaxLongitude = maxLongitude;
        }
    }

    public class Position
    {
        public decimal Latitude { get; }
        public decimal Longitude { get; }

        public Position(decimal latitude, decimal longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}