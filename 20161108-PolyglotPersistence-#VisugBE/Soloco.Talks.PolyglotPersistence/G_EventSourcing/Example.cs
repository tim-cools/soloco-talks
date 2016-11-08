using System;
using Marten.Schema.Identity;
using Soloco.Talks.PolyglotPersistence.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence.G_EventSourcing
{
    public class Examples_G
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Examples_G(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void EventSourcingShowTheEvents()
        {
            var store = TestDocumentStore.Create();
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

            using (var session = store.OpenSession())
            {
                var eventStream = session.Events.FetchStream(routeId);

                _testOutputHelper.WriteAsJson(eventStream);
            }
        }

        [Fact]
        public void EventSourcingShowAggregate()
        {
            var store = TestDocumentStore.Create();
            var routeId = CombGuidIdGeneration.NewGuid();

            using (var session = store.OpenSession())
            {
                var route = new Route(routeId);
                route.Plan(DateTime.Now.AddDays(1));
                route.AddSource(new StopName("Home"), new TimeOfDay(17, 30), new Position(51.197894m, 4.481736m));
                route.AddDestination(new StopName("WooRank"), new TimeOfDay(18, 30), new Position(50.828417m, 4.400963m));
                route.AddStop(new StopName("Filling station"), new TimeOfDay(17, 45), new Position(50.828417m, 4.400963m));
                route.Drive(new DateTime(2016, 05, 20, 17, 32, 0));

                session.Events.StartStream<Route>(route.Id);
                session.Events.Append(route.Id, route.GetChanges());

                session.SaveChanges();
            }

            using (var session = store.OpenSession())
            {
                var route = session.Events.AggregateStream<Route>(routeId);

                _testOutputHelper.WriteAsJson(route);
            }
        }

        [Fact]
        public void EventSourcingWithInlinieAggregation()
        {
            var store = TestDocumentStore.Create(optionsHandler: options =>
            {
                options.Events.InlineProjections.AggregateStreamsWith<Route>();
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

                session.Events.StartStream<Route>(route.Id, route.GetChanges());

                session.SaveChanges();
            }

            using (var session = store.OpenSession())
            {
                var route = session.Load<Route>(routeId);

                _testOutputHelper.WriteAsJson(route);
            }
        }
    }
}
