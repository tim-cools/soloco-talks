using System;
using Marten;
using Soloco.Talks.PolyglotPersistence.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence.B_StoreAndLoadAggregate
{
   
    public class Examples
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Examples(ITestOutputHelper testOutputHelper)
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

                _testOutputHelper.WriteLine(route.ToString());
            }
        }

        [Fact]
        public void FindAggregateAsJson()
        {
            var store = TestDocumentStore.Create();
            var id = AddRoute(store);

            using (var session = store.QuerySession())
            {
                var json = session.FindJsonById<Route>(id);

                _testOutputHelper.WriteFormattedJson(json);
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