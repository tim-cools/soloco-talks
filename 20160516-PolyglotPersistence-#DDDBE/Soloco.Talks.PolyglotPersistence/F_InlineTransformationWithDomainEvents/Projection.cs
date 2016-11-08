// ReSharper disable CheckNamespace

using System;
using System.Collections.Generic;
using System.Linq;
using Marten;
using Soloco.Talks.PolyglotPersistence.F_InlineTransformationWithDomainEvents.Domain;
using Soloco.Talks.PolyglotPersistence.Infrastructure;

namespace Soloco.Talks.PolyglotPersistence.F_InlineTransformationWithDomainEvents
{
    public class RouteDetailsViewProjection : BaseProjection
    {
        public RouteDetailsViewProjection()
        {
            Register<RoutePlanned>(RoutePlanned);
            Register<StopCreated>(StopCreated);
        }

        private void RoutePlanned(IDocumentSession session, RoutePlanned @event)
        {
            var routeDetails = GetRouteDetails(session, @event.RouteId);

            routeDetails.Status = "Planned";
            routeDetails.PlannedDate = @event.Date;
            routeDetails.PlannedSince = DateTime.Now;
        }

        private void StopCreated(IDocumentSession session, StopCreated @event)
        {
            var routeDetails = GetRouteDetails(session, @event.RouteId);

            var stopDetails = new RouteStopDetails
            {
                StopId = @event.ID,
                Name = @event.Name,
                Position = new Position(@event.Position.Latitude, @event.Position.Longitude)
            };

            routeDetails.Stops.Add(stopDetails);
            routeDetails.StopsArea = CalculateArea(routeDetails.Stops);
        }

        private static Area CalculateArea(IList<RouteStopDetails> stops)
        {
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