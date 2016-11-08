using System;
using Marten.Schema.Identity;
using Soloco.Talks.PolyglotPersistence.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence.H_EventSourcingWithProjections
{
    public class Examples_H
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Examples_H(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void EventSourcingWithInlineProjection()
        {
            var store = TestDocumentStore.Create(optionsHandler: options =>
            {
                options.Events.InlineProjections.Add(new RouteDetailsViewProjection());
            });

            var routeId = CombGuidIdGeneration.NewGuid();

            using (var session = store.OpenSession())
            {
                var route = new Route(routeId);            
                route.Plan(DateTime.Now.AddDays(1));
                route.AddSource(new StopName("Home"), new TimeOfDay(17, 30), new Position(51.197894m, 4.481736m));
                route.AddDestination(new StopName("WooRank"), new TimeOfDay(18, 30), new Position(50.828417m, 4.400963m));
                route.AddStop(new StopName("Filling station"), new TimeOfDay(17, 45), new Position(50.828417m, 4.400963m));
                route.Drive(new DateTime(2016, 05, 20, 17, 32, 0));

                var events = route.GetChanges();
                //_testOutputHelper.WriteAsJson(events);

                session.Events.StartStream<Route>(route.Id);
                session.Events.Append(route.Id, events);

                session.SaveChanges();
            }

            //load the view by route id
            using (var session = store.OpenSession())
            {
                var view = session.Load<RouteDetails>(routeId);

                _testOutputHelper.WriteAsJson(view);
            }
        }
    }
}
