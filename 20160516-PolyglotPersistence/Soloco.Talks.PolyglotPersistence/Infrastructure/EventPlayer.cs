using System;

namespace Soloco.Talks.PolyglotPersistence.Infrastructure
{
    internal abstract class EventPlayer
    {
        public static void Apply(object aggregate, object @event)
        {
            if (aggregate == null) throw new ArgumentNullException(nameof(aggregate));
            if (@event == null) throw new ArgumentNullException(nameof(@event));
            
            //todo we should generate some code here to invoke the methed and cache it here
            var aggregateType = aggregate.GetType();
            var eventType = @event.GetType();

            var method = aggregateType.GetMethod("Apply", new[] { eventType});
            if (method == null)
            {
                throw new InvalidOperationException($"No 'Apply' method found for aggregate '{aggregateType}' and event '{eventType}");
            }
            method.Invoke(aggregate, new[] {@event});
        }
    }
}