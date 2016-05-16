using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Marten;
using Soloco.Talks.PolyglotPersistence.Infrastructure;
using Soloco.Talks.PolyglotPersistence.TestData;
using Xunit;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence
{
   
    public class AggregateExamples
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AggregateExamples(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }


        [Fact]
        public void AggregateById()
        {
            var store = TestDocumentStore.Create();
            var id = AddRoute(store);

            using (var session = store.QuerySession())
            {
                var route = session.Load<Route>(id);

                _testOutputHelper.WriteLine(route.ToString());
            }
        }

        [Fact]
        public void AggregateAsJson()
        {
            var store = TestDocumentStore.Create();
            var id = AddRoute(store);

            using (var session = store.QuerySession())
            {
                var json = session.FindJsonById<Route>(id);

                _testOutputHelper.WriteLine(json);
            }
        }

        private static Guid AddRoute(IDocumentStore store)
        {
            using (var session = store.OpenSession())
            {
                var route = new Route();
                route.Plan(new DateTime(2016, 05, 26));
                route.AddStop("Home", new Position(51.197894m, 4.481736m));
                route.AddStop("WooRank", new Position(50.828417m, 4.400963m));
                route.AddStop("Home", new Position(51.197894m, 4.481736m));

                session.Store(route);
                session.SaveChanges();

                return route.ID;
            }
        }
    }    
}