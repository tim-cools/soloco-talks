using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Marten;
using Marten.Linq;
using Soloco.Talks.PolyglotPersistence.Infrastructure;
using Soloco.Talks.PolyglotPersistence.TestData;
using Xunit;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence
{
    public class QueryExamples
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public QueryExamples(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Query()
        {
            var store = TestDocumentStore.Create();
            store.AddRoutes(100);

            using (var session = store.QuerySession())
            {
                var routes = session.Query<Route>()
                    .Where(route => route.Status == RouteStatus.Planned)
                    .ToList();

                _testOutputHelper.WriteLine(routes.AsString());
            }
        }

        [Fact]
        public void QueryWithProjection()
        {
            var store = TestDocumentStore.Create();
            store.AddRoutes(100);

            using (var session = store.QuerySession())
            {
                var routes = session.Query<Route>()
                    .Where(route => route.Status == RouteStatus.Planned)
                    .Select(route => new { route.ID, route.Date })
                    .ToList();

                foreach (var route in routes)
                {
                    _testOutputHelper.WriteLine($"ID: {route.ID} Date: {route.Date}");
                }
            }
        }

        [Fact]
        public void QueryExplaind()
        {
            var store = TestDocumentStore.Create();
            store.AddRoutes(100);

            using (var session = store.QuerySession())
            {
                var queryPlan = session.Query<Route>()
                    .Where(route => route.Status == RouteStatus.Planned)
                    .Explain();

                _testOutputHelper.WriteLine(queryPlan.AsString());
            }
        }

        public class RoutesPlannedAfter : ICompiledQuery<Route, IEnumerable<Route>>
        {
            public DateTime DateTime { get; }

            public RoutesPlannedAfter(DateTime dateTime)
            {
                DateTime = dateTime;
            }

            public Expression<Func<IQueryable<Route>, IEnumerable<Route>>> QueryIs()
            {
                return query => query.Where(route => route.Status == RouteStatus.Planned && route.Date > DateTime);
            }
        }

        [Fact]
        public void QueryCompiled()
        {
            var store = TestDocumentStore.Create();
            store.AddRoutes(100);

            using (var session = store.QuerySession())
            {
                var routes = session.Query(new RoutesPlannedAfter(DateTime.Now.AddDays(5))).ToList();

                _testOutputHelper.WriteLine(routes.AsString());
            }
        }

        public class RoutesPlannedAfterJson : ICompiledQuery<Route, IEnumerable<string>>
        {
            public DateTime DateTime { get; }

            public RoutesPlannedAfterJson(DateTime dateTime)
            {
                DateTime = dateTime;
            }

            public Expression<Func<IQueryable<Route>, IEnumerable<string>>> QueryIs()
            {
                return query => query.Where(route => route.Status == RouteStatus.Planned && route.Date > DateTime)
                    .AsJson();
            }
        }

        [Fact]
        public void QueryCompiledJson()
        {
            var store = TestDocumentStore.Create();
            store.AddRoutes(100);

            using (var session = store.QuerySession())
            {
                var routes = session.Query(new RoutesPlannedAfterJson(DateTime.Now.AddDays(5))).ToList();

                _testOutputHelper.WriteLine(routes.AsString());
            }
        }
    }    
}