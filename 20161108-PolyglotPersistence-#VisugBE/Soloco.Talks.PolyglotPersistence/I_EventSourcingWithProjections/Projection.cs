using System;
using System.Collections.Generic;
using System.Linq;
using Marten.Events.Projections;
using Soloco.Talks.PolyglotPersistence.I_EventSourcingWithProjections.Domain;

namespace Soloco.Talks.PolyglotPersistence.I_EventSourcingWithProjections
{
    public class RouteDetailsViewProjection : ViewProjection<RouteDetails>
    {
        public RouteDetailsViewProjection()
        {
            ProjectEvent<RouteCreated>(RouteCreated);
            ProjectEvent<RoutePlanned>(RoutePlanned);
            ProjectEvent<RouteSourceAdded>(RouteSourceAdded);
            ProjectEvent<RouteDestinationAdded>(RouteDestinationAdded);
            ProjectEvent<RouteStopAdded>(RouteStopAdded);
            ProjectEvent<RouteDriving>(RouteDriving);
        }

        private void RouteCreated(RouteDetails routeDetails, RouteCreated @event)
        {
            routeDetails.Status = "Created";
        }

        private void RoutePlanned(RouteDetails routeDetails, RoutePlanned @event)
        {
            routeDetails.Status = "Planned";
            routeDetails.PlannedDate = @event.Date;
            routeDetails.PlannedSince = DateTime.Now;
        }

        private void RouteDriving(RouteDetails routeDetails, RouteDriving @event)
        {
            routeDetails.Status = "Driving";
            routeDetails.DrivingDate = @event.Date;
        }

        private void RouteSourceAdded(RouteDetails routeDetails, RouteSourceAdded @event)
        {
            var stopDetails = new RouteStopDetails
            {
                StopId = @event.RouteId,
                Name = @event.Name.Value,
                Position = new Position(@event.Position.Latitude, @event.Position.Longitude)
            };

            routeDetails.From = stopDetails;
            routeDetails.StopsArea = CalculateArea(routeDetails);
        }

        private void RouteDestinationAdded(RouteDetails routeDetails, RouteDestinationAdded @event)
        {
            var stopDetails = new RouteStopDetails
            {
                StopId = @event.RouteId,
                Name = @event.Name.Value,
                Position = new Position(@event.Position.Latitude, @event.Position.Longitude)
            };

            routeDetails.To = stopDetails;
            routeDetails.StopsArea = CalculateArea(routeDetails);
        }

        private void RouteStopAdded(RouteDetails routeDetails, RouteStopAdded @event)
        {
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
    }
}