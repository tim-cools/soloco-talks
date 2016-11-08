using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Marten.Events;
using Marten.Events.Projections;

namespace Soloco.Talks.PolyglotPersistence.Infrastructure
{
    public class BaseProjection : IProjection
    {
        private readonly IDictionary<Type, Action<IDocumentSession, object>> _handlers = new ConcurrentDictionary<Type, Action<IDocumentSession, object>>();
        private readonly IDictionary<Type, Func<IDocumentSession, object, Task>> _asyncHandlers = new ConcurrentDictionary<Type, Func<IDocumentSession, object, Task>>();

        protected void Register<T>(Action<IDocumentSession, T> handler) where T : class
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            _handlers.Add(typeof(T), (session, @event) => handler(session, @event as T));
        }

        protected void Register<T>(Func<IDocumentSession, object, Task> handler) where T : class
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            _asyncHandlers.Add(typeof(T), (session, @event) => handler(session, @event as T));
        }

        public void Apply(IDocumentSession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            var events = GetEvents(session);

            foreach (var @event in events)
            {
                Action<IDocumentSession, object> handler;
                if (_handlers.TryGetValue(@event.GetType(), out handler))
                {
                    handler(session, @event);
                }
            }
        }

        public async Task ApplyAsync(IDocumentSession session, CancellationToken token)
        {
            var events = GetEvents(session);

            foreach (var @event in events)
            {
                Func<IDocumentSession, object, Task> handler;
                if (_asyncHandlers.TryGetValue(@event.GetType(), out handler))
                {
                    await handler(session, @event);
                }
            }
        }

        private static IEnumerable<object> GetEvents(IDocumentSession session)
        {
            return session
                .PendingChanges.AllChangedFor<EventStream>()
                .SelectMany(stream => stream.Events)
                .Select(@event => @event.Data);
        }
    }
}