// ReSharper disable CheckNamespace

using System;
using Marten;
using Soloco.Talks.PolyglotPersistence.Infrastructure;
using Xunit;
using Xunit.Abstractions;
using Soloco.Talks.PolyglotPersistence.F_InlineTransformationWithDomainEvents.Domain;

namespace Soloco.Talks.PolyglotPersistence.F_InlineTransformationWithDomainEvents
{
    public class AggregateWithProjectionExamples_F
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AggregateWithProjectionExamples_F(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CreateAnInlineViewModelBasedOnDomainEvents()
        {
            var store = TestDocumentStore.Create(optionsHandler: options =>
            {
                options.Events.InlineProjections.Add(new RouteDetailsViewProjection());
            });

            var routeId = CreateRoute(store);
            AddStop(store, routeId, "Home", new Domain.Position(51.197894m, 4.481736m));
            AddStop(store, routeId, "WooRank", new Domain.Position(50.828417m, 4.400963m));

            //load the view by route id
            using (var session = store.OpenSession())
            {
                var route = session.Load<RouteDetails>(routeId);

                _testOutputHelper.WriteAsJson(route);
            }
        }

        private static Guid CreateRoute(IDocumentStore store)
        {
            using (var session = store.OpenSession())
            {
                var route = new Route();

                route.Plan(DateTime.Now.AddDays(1));

                session.Store(route);
                session.Events.Append(route.Id, route.GetChanges());
                session.SaveChanges();

                return route.Id;
            }
        }

        private static void AddStop(IDocumentStore store, Guid routeId, string name, Domain.Position position)
        {
            using (var session = store.OpenSession())
            {
                var stop = new Stop(routeId, name, position);

                session.Store(stop);
                session.Events.Append(stop.Id, stop.GetChanges());
                session.SaveChanges();
            }
        }
    }
}
