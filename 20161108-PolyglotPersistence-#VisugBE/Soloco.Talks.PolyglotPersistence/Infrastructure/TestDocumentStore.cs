using System;
using Marten;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence.Infrastructure
{
    public static class TestDocumentStore
    {
        public static IDocumentStore CreateSimple()
        {
            var documentStore = DocumentStore.For(options =>
            {
                options.Connection(ConnectionString.Local);
                options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                options.Serializer(new JsonNetWithPrivateSupportSerializer());
            });

            documentStore.Advanced.Clean.CompletelyRemoveAll();

            return documentStore;
        }

        public static IDocumentStore Create(string connectionString = null, ITestOutputHelper testOutputHelper = null, Action<StoreOptions> optionsHandler = null, bool clear = true)
        {
            var documentStore = DocumentStore.For(options =>
            {
                options.Connection(connectionString ?? ConnectionString.Local);
                options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                options.Serializer(new JsonNetWithPrivateSupportSerializer());
                
                if (testOutputHelper != null)
                {
                    options.Logger(new TestOutputLogger(testOutputHelper));
                }

                optionsHandler?.Invoke(options);
            });

            if (clear)
            {
                documentStore.Advanced.Clean.CompletelyRemoveAll();
            }

            return documentStore;
        }
    }
}