// ReSharper disable CheckNamespace

using System;
using System.Collections.Generic;
using System.Linq;
using Marten.Events.Projections;
using Soloco.Talks.PolyglotPersistence.F_InlineTransformationWithDomainEvents.Domain;

namespace Soloco.Talks.PolyglotPersistence.F_InlineTransformationWithDomainEvents
{
    public class RouteDetailsViewProjection : ViewProjection<RouteDetails>
    {
        public RouteDetailsViewProjection()
        {
            ProjectEvent<RoutePlanned>(RoutePlanned);
            ProjectEvent<StopCreated>(StopCreated);
        }

        private void RoutePlanned(RouteDetails routeDetails, RoutePlanned @event)
        {
            routeDetails.Status = "Planned";
            routeDetails.PlannedDate = @event.Date;
            routeDetails.PlannedSince = DateTime.Now;
        }

        private void StopCreated(RouteDetails routeDetails, StopCreated @event)
        {
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
    }
}