using System;
using Marten;
using Soloco.Talks.PolyglotPersistence.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence.A_CreateStore
{
    public class Examples_A
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Examples_A(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CreateADocuementStoreAndCleanDb()
        {
            var documentStore = DocumentStore.For(options =>
            {
                options.Connection(ConnectionString.Local);
                options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
            });

            documentStore.Advanced.Clean.CompletelyRemoveAll();
        }

        [Fact]
        public void CreateADocuementStoreAndCreateSessions()
        {
            var documentStore = DocumentStore.For(options =>
            {
                options.Connection(ConnectionString.Local);
                options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                options.Serializer(new JsonNetWithPrivateSupportSerializer());
            });

            documentStore.Advanced.Clean.CompletelyRemoveAll();

            using (var session = documentStore.QuerySession())
            {
            }

            using (var session = documentStore.LightweightSession())
            {
            }

            using (var session = documentStore.DirtyTrackedSession())
            {
            }
        }

        [Fact]
        public void StoreAndLoadDocument()
        {
            var store = TestDocumentStore.Create(testOutputHelper: _testOutputHelper);

            var route = new Route { Date = DateTime.Now };
            route.Stops.Add(new Stop { Name = "Home", Position = new Position { Latitude = 51.197894m, Longitude = 4.481736m } });
            route.Stops.Add(new Stop { Name = "Visug", Position = new Position { Latitude = 51.142128m, Longitude = 4.43787m } });
            route.Stops.Add(new Stop { Name = "Home", Position = new Position { Latitude = 51.197894m, Longitude = 4.481736m } });

            using (var session = store.OpenSession())
            {
                session.Store(route);
                session.SaveChanges();
            }

            using (var session = store.QuerySession())
            {
                var json = session.Load<Route>(route.Id);

                _testOutputHelper.WriteAsJson(json);
            }
        }
    }
}
