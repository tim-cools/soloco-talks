using System;
using System.Collections.Generic;
using System.Linq;
using Soloco.Talks.PolyglotPersistence.Infrastructure;

namespace Soloco.Talks.PolyglotPersistence.G_EventSourcing
{
    public class AggregateRoot
    {
        private readonly IList<object> _events = new List<object>();

        protected void Publish<TEvent>(TEvent @event)
        {
            _events.Add(@event);

            EventPlayer.Apply(this, @event);
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

        public Stop Source { get; private set; }
        public Stop Destination { get; private set; }

        public IEnumerable<Stop> Stops => _stops;

        public Route()
        {
        }

        public Route(Guid id)
        {
            Publish(new RouteCreated(id));
        }

        public void Apply(RouteCreated @event)
        {
            Status = RouteStatus.Created;
            Id = @event.RouteId;
        }

        public void Plan(DateTime date)
        {
            if (date < Tomorrow())
            {
                throw new InvalidOperationException("Can only plan route from tomorrow.");
            }

            Publish(new RoutePlanned(Id, date));
        }

        private static DateTime Tomorrow()
        {
            return DateTime.Today.AddDays(1);
        }

        public void Apply(RoutePlanned @event)
        {
            Status = RouteStatus.Planned;
            Date = @event.Date;
        }

        public void AddSource(StopName name, TimeOfDay timeOfDay, Position position)
        {
            if (Status != RouteStatus.Planned)
            {
                throw new InvalidOperationException("Can only set source planned routes.");
            }
            if (Destination != null && timeOfDay < Destination.TimeOfDay)
            {
                throw new InvalidOperationException("Route source time should come before destination time.");
            }

            Publish(new RouteSourceAdded(Id, name, timeOfDay, position));
        }

        public void Apply(RouteSourceAdded @event)
        {
            Source = new Stop(@event.Name, @event.TimeOfDay, @event.Position);
        }

        public void AddDestination(StopName name, TimeOfDay timeOfDay, Position position)
        {
            if (Status != RouteStatus.Planned)
            {
                throw new InvalidOperationException("Can only add destination to planned routes.");
            }
            if (Source == null)
            {
                throw new InvalidOperationException("Route source should be set first.");
            }
            if (timeOfDay <= Source.TimeOfDay)
            {
                throw new InvalidOperationException("Route destination time should come after source time.");
            }

            Publish(new RouteDestinationAdded(Id, name, timeOfDay, position));
        }

        public void Apply(RouteDestinationAdded @event)
        {
            Destination = new Stop(@event.Name, @event.TimeOfDay, @event.Position);
        }

        public void AddStop(StopName name, TimeOfDay timeOfDay, Position position)
        {
            if (Status != RouteStatus.Planned)
            {
                throw new InvalidOperationException("Can only set source planned routes.");
            }
            if (Source == null || Destination == null)
            {
                throw new InvalidOperationException("Route source and destination should be set first.");
            }
            if (timeOfDay <= Source.TimeOfDay || timeOfDay >= Destination.TimeOfDay)
            {
                throw new InvalidOperationException("Route stop time should be between planned source and destination time.");
            }

            Publish(new RouteStopAdded(Id, name, timeOfDay, position));
        }

        public void Apply(RouteStopAdded @event)
        {
            _stops.Add(new Stop(@event.Name, @event.TimeOfDay, @event.Position));
        }

        public void Drive(DateTime date)
        {
            if (Status != RouteStatus.Planned)
            {
                throw new InvalidOperationException("Can only drive planned routes.");
            }
            if (Source == null)
            {
                throw new InvalidOperationException("Route should have a source stop.");
            }
            if (Destination == null)
            {
                throw new InvalidOperationException("Route should have a destination stop.");
            }
            if (_stops.Count < 1)
            {
                throw new InvalidOperationException("Route should have at least one stops before you can drive.");
            }

            Publish(new RouteDriving(Id, date));
        }

        public void Apply(RouteDriving @event)
        {
            Status = RouteStatus.Driving;
        }
    }

    public class Stop
    {
        public StopName Name { get; }
        public TimeOfDay TimeOfDay { get; }
        public Position Position { get; }

        public Stop(StopName name, TimeOfDay timeOfDay, Position position)
        {
            Name = name;
            TimeOfDay = timeOfDay;
            Position = position;
        }
    }

    public class StopName
    {
        public string Value { get; private set; }

        public StopName(string value)
        {
            Value = value;
        }
    }

    public class TimeOfDay
    {
        public int Hours { get; private set; }
        public int Minutes { get; private set; }

        public TimeOfDay(int hours, int minutes)
        {
            Hours = hours;
            Minutes = minutes;
        }

        public int TotalMinutes()
        {
            return Hours*60 + Minutes;
        }

        protected bool Equals(TimeOfDay other)
        {
            return TotalMinutes() == other.TotalMinutes();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TimeOfDay) obj);
        }

        public override int GetHashCode()
        {
            return TotalMinutes();
        }

        public static bool operator ==(TimeOfDay left, TimeOfDay right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TimeOfDay left, TimeOfDay right)
        {
            return !Equals(left, right);
        }

        public static bool operator <(TimeOfDay left, TimeOfDay right)
        {
            return left.TotalMinutes() < right.TotalMinutes();
        }

        public static bool operator >(TimeOfDay left, TimeOfDay right)
        {
            return left.TotalMinutes() > right.TotalMinutes();
        }
        public static bool operator <=(TimeOfDay left, TimeOfDay right)
        {
            return left.TotalMinutes() <= right.TotalMinutes();
        }

        public static bool operator >=(TimeOfDay left, TimeOfDay right)
        {
            return left.TotalMinutes() >= right.TotalMinutes();
        }
    }

    public enum RouteStatus
    {
        Created,
        Planned,
        Driving,
        Stopped
    }
}
