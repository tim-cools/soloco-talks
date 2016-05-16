using System;
using Marten;
using Soloco.Talks.PolyglotPersistence.TestData;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Soloco.Talks.PolyglotPersistence.Infrastructure
{
    public static class TestDocumentStore
    {
        public static IDocumentStore Create(ITestOutputHelper testOutputHelper = null, Action<StoreOptions> optionsHandler = null)
        {
            var documentStore = DocumentStore.For(options =>
            {
                options.Connection(Connection.String);
                options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                if (testOutputHelper != null)
                {
                    options.Logger(new TestOutputLogger(testOutputHelper));
                }
                optionsHandler?.Invoke(options);
            });
            documentStore.Advanced.Clean.CompletelyRemoveAll();
            return documentStore;
        }
    }
}