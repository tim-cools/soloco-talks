using System;

namespace Soloco.Talks.PolyglotPersistence.I_EventSourcingWithProjections.Domain
{
    public class RouteCreated
    {
        public Guid RouteId { get; private set; }

        public RouteCreated(Guid routeId)
        {
            RouteId = routeId;
        }
    }

    public class RoutePlanned
    {
        public Guid RouteId { get; private set; }
        public DateTime Date { get; private set; }

        public RoutePlanned(Guid routeId, DateTime date)
        {
            RouteId = routeId;
            Date = date;
        }
    }

    public class RouteSourceAdded
    {
        public Guid RouteId { get; private set; }
        public StopName Name { get; private set; }
        public TimeOfDay TimeOfDay { get; private set; }
        public Position Position { get; private set; }

        public RouteSourceAdded(Guid routeId, StopName name, TimeOfDay timeOfDay, Position position)
        {
            RouteId = routeId;
            Name = name;
            TimeOfDay = timeOfDay;
            Position = position;
        }
    }

    public class RouteDestinationAdded
    {
        public Guid RouteId { get; private set; }
        public StopName Name { get; private set; }
        public TimeOfDay TimeOfDay { get; private set; }
        public Position Position { get; private set; }

        public RouteDestinationAdded(Guid routeId, StopName name, TimeOfDay timeOfDay, Position position)
        {
            RouteId = routeId;
            Name = name;
            TimeOfDay = timeOfDay;
            Position = position;
        }
    }

    public class RouteStopAdded
    {
        public Guid RouteId { get; private set; }
        public StopName Name { get; private set; }
        public TimeOfDay TimeOfDay { get; private set; }
        public Position Position { get; private set; }

        public RouteStopAdded(Guid routeId, StopName name, TimeOfDay timeOfDay, Position position)
        {
            RouteId = routeId;
            Name = name;
            TimeOfDay = timeOfDay;
            Position = position;
        }
    }

    public class RouteDriving
    {
        public Guid RouteId { get; private set; }
        public DateTime Date { get; private set; }

        public RouteDriving(Guid routeId, DateTime date)
        {
            RouteId = routeId;
            Date = date;
        }
    }
}
