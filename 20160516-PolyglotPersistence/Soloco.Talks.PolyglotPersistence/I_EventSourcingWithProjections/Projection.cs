using System;
using System.Collections.Generic;
using System.Linq;
using Marten;
using Soloco.Talks.PolyglotPersistence.I_EventSourcingWithProjections.Domain;
using Soloco.Talks.PolyglotPersistence.Infrastructure;

namespace Soloco.Talks.PolyglotPersistence.I_EventSourcingWithProjections
{
    public class RouteDetailsViewProjection : BaseProjection
    {
        public RouteDetailsViewProjection()
        {
            Register<RouteCreated>(RouteCreated);
            Register<RoutePlanned>(RoutePlanned);
            Register<RouteSourceAdded>(RouteSourceAdded);
            Register<RouteDestinationAdded>(RouteDestinationAdded);
            Register<RouteStopAdded>(RouteStopAdded);
            Register<RouteDriving>(RouteDriving);
        }

        private void RouteCreated(IDocumentSession session, RouteCreated @event)
        {
            var routeDetails = GetRouteDetails(session, @event.RouteId);

            routeDetails.Status = "Created";
        }

        private void RoutePlanned(IDocumentSession session, RoutePlanned @event)
        {
            var routeDetails = GetRouteDetails(session, @event.RouteId);

            routeDetails.Status = "Planned";
            routeDetails.PlannedDate = @event.Date;
            routeDetails.PlannedSince = DateTime.Now;
        }

        private void RouteDriving(IDocumentSession session, RouteDriving @event)
        {
            var routeDetails = GetRouteDetails(session, @event.RouteId);

            routeDetails.Status = "Driving";
            routeDetails.DrivingDate = @event.Date;
        }

        private void RouteSourceAdded(IDocumentSession session, RouteSourceAdded @event)
        {
            var routeId = @event.RouteId;
            var value = @event.Name.Value;
            var position = @event.Position;
            var routeDetails = GetRouteDetails(session, routeId);

            var stopDetails = new RouteStopDetails
            {
                StopId = routeId,
                Name = value,
                Position = new Position(position.Latitude, position.Longitude)
            };

            routeDetails.From = stopDetails;
            routeDetails.StopsArea = CalculateArea(routeDetails);
        }

        private void RouteDestinationAdded(IDocumentSession session, RouteDestinationAdded @event)
        {
            var routeDetails = GetRouteDetails(session, @event.RouteId);

            var stopDetails = new RouteStopDetails
            {
                StopId = @event.RouteId,
                Name = @event.Name.Value,
                Position = new Position(@event.Position.Latitude, @event.Position.Longitude)
            };

            routeDetails.To = stopDetails;
            routeDetails.StopsArea = CalculateArea(routeDetails);
        }

        private void RouteStopAdded(IDocumentSession session, RouteStopAdded @event)
        {
            var routeDetails = GetRouteDetails(session, @event.RouteId);

            var stopDetails = new RouteStopDetails
            {
                StopId = @event.RouteId,
                Name = @event.Name.Value,
                Position = new Position(@event.Position.Latitude, @event.Position.Longitude)
            };

            routeDetails.Stops.Add(stopDetails);
            routeDetails.StopsArea = CalculateArea(routeDetails);
        }

        private static Area CalculateArea(RouteDetails route)
        {
            var stops = new List<RouteStopDetails>(route.Stops);
            if (route.From != null)
            {
                stops.Add(route.From);
            }
            if (route.To != null)
            {
                stops.Add(route.To);
            }

            return new Area(
                stops.Select(stop => stop.Position.Latitude).Min(),
                stops.Select(stop => stop.Position.Longitude).Min(),
                stops.Select(stop => stop.Position.Latitude).Max(),
                stops.Select(stop => stop.Position.Longitude).Max()
                );
        }

        private static RouteDetails GetRouteDetails(IDocumentSession session, Guid routeId)
        {
            var routeDetails = session.Load<RouteDetails>(routeId) ?? new RouteDetails { Id = routeId };
            session.Store(routeDetails);
            return routeDetails;
        }
    }
}