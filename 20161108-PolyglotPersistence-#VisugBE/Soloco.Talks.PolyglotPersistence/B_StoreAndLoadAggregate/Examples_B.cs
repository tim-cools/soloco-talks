using System;
using Marten;
using Soloco.Talks.PolyglotPersistence.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence.B_StoreAndLoadAggregate
{
    public class Examples_B
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Examples_B(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void LoadAggregateById()
        {
            var store = TestDocumentStore.CreateSimple();
            var id = AddRoute(store);

            using (var session = store.QuerySession())
            {
                var route = session.Load<Route>(id);

                _testOutputHelper.WriteAsJson(route);
            }
        }

        [Fact]
        public void FindAggregateAsJson()
        {
            var store = TestDocumentStore.Create();
            var id = AddRoute(store);

            using (var session = store.QuerySession())
            {
                var json = session.Json.FindById<Route>(id);

                _testOutputHelper.WriteAsJson(json);
            }
        }

        private static Guid AddRoute(IDocumentStore store)
        {
            using (var session = store.OpenSession())
            {
                var route = new Route();
                route.Plan(new DateTime(2016, 05, 27));
                route.AddStop("Home", new Position(51.197894m, 4.481736m));
                route.AddStop("WooRank", new Position(50.828417m, 4.400963m));
                route.AddStop("Home", new Position(51.197894m, 4.481736m));

                session.Store(route);
                session.SaveChanges();

                return route.Id;
            }
        }
    }
}