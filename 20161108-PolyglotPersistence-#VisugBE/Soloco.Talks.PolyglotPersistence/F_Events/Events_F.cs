// ReSharper disable CheckNamespace

using System;
using Marten.Schema.Identity;
using Soloco.Talks.PolyglotPersistence.Infrastructure;
using Xunit;
using Xunit.Abstractions;
using Soloco.Talks.PolyglotPersistence.F_InlineTransformationWithDomainEvents.Domain;

namespace Soloco.Talks.PolyglotPersistence.F_InlineTransformationWithDomainEvents
{
    public class Events_F
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Events_F(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CreateAnInlineViewModelBasedOnDomainEvents()
        {
            var store = TestDocumentStore.Create();

            var streamId = CombGuidIdGeneration.NewGuid();

            using (var session = store.OpenSession())
            {
                session.Events.StartStream<Route>(streamId,
                    new RoutePlanned(streamId, DateTime.Now),
                    new StopCreated(Guid.NewGuid(), streamId, "Home", TestData.PositionHome),
                    new StopCreated(Guid.NewGuid(), streamId, "Visug", TestData.PositionVisug),
                    new StopCreated(Guid.NewGuid(), streamId, "Home", TestData.PositionHome)
                    );

                session.SaveChanges();
            }

            using (var session = store.OpenSession())
            {
                var events = session.Events.FetchStream(streamId);

                _testOutputHelper.WriteAsJson(events);
            }
        }
    }
}
