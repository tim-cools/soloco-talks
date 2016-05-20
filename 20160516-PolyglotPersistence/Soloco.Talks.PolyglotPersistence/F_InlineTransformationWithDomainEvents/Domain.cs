// ReSharper disable CheckNamespace

using System;
using System.Collections.Generic;
using System.Linq;
using Soloco.Talks.PolyglotPersistence.Infrastructure;

namespace Soloco.Talks.PolyglotPersistence.F_InlineTransformationWithDomainEvents.Domain
{
    public class AggregateRoot
    {
        private readonly IList<object> _events = new List<object>();

        protected void Publish<TEvent>(TEvent @event)
        {
            _events.Add(@event);
        }

        public object[] GetChanges()
        {
            return _events.ToArray();
        }
    }

    public class Route : AggregateRoot
    {
        private readonly List<Stop> _stops = new List<Stop>();

        public Guid Id { get; private set; }
        public RouteStatus Status { get; private set; }
        public DateTime Date { get; private set; }

        public void Plan(DateTime date)
        {
            if (date < DateTime.Today.AddDays(1))
            {
                throw new InvalidOperationException("Route can only plan from tomorrow.");
            }

            Status = RouteStatus.Planned;
            Date = date;

            Publish(new RoutePlanned(Id, date));
        }

        public override string ToString()
        {
            return $"ID: {Id}, Status: {Status}, Date: {Date}{_stops.AsString()}";
        }
    }

    public enum RouteStatus
    {
        Created,
        Planned,
        Driving,
        Stopped
    }

    public class Stop : AggregateRoot
    {
        public Guid Id { get; private set; }
        public string Name { get; }
        public Guid RouteId { get; }
        public Position Position { get; }

        protected Stop()
        {
        }

        public Stop(Guid routeId, string name, Position position)
        {
            Id = Guid.NewGuid();
            RouteId = routeId;
            Name = name;
            Position = position;

            Publish(new StopCreated(Id, routeId, name, position));
        }

        public override string ToString()
        {
            return $"Route: {RouteId} Name: {Name}, Position: {Position}";
        }

    }

    public class RoutePlanned
    {
        public Guid RouteId { get; }
        public DateTime Date { get; }

        public RoutePlanned() // todo currently a Marten constraint
        {
        }

        public RoutePlanned(Guid routeId, DateTime date)
        {
            RouteId = routeId;
            Date = date;
        }
    }

    public class StopCreated
    {
        public Guid ID { get; }
        public Guid RouteId { get; }
        public string Name { get; }
        public Position Position { get; }

        public StopCreated() // todo currently a Marten constraint
        {
        }

        public StopCreated(Guid id, Guid routeId, string name, Position position)
        {
            ID = id;
            RouteId = routeId;
            Name = name;
            Position = position;
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
