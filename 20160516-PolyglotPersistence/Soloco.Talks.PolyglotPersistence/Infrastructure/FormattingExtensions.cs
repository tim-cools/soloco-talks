using System;
using System.Collections.Generic;
using System.IO;
using Marten.Linq;
using Soloco.Talks.PolyglotPersistence.TestData;

namespace Soloco.Talks.PolyglotPersistence.Infrastructure
{
    public static class FormattingExtensions
    {
        public static string AsString(this IList<Route> routes)
        {
            if (routes == null) throw new ArgumentNullException(nameof(routes));

            using (var writer = new StringWriter())
            {
                writer.WriteLine($"Routes: ({routes.Count})");

                foreach (var route in routes)
                {
                    writer.WriteLine(route);
                }
                return writer.ToString();
            }
        }

        public static string AsString<T>(this IList<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            using (var writer = new StringWriter())
            {
                foreach (var item in items)
                {
                    writer.WriteLine(item);
                }
                return writer.ToString();
            }
        }

        public static string AsString<TKey, TItem>(this IDictionary<TKey, TItem> dictionary)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));

            using (var writer = new StringWriter())
            {
                foreach (var item in dictionary)
                {
                    writer.WriteLine(item.Value);
                }
                return writer.ToString();
            }
        }

        public static string AsString(this IList<Stop> stops)
        {
            if (stops == null) throw new ArgumentNullException(nameof(stops));

            using (var writer = new StringWriter())
            {
                writer.WriteLine($"  Stops: ({stops.Count})");
                foreach (var stop in stops)
                {
                    writer.WriteLine("  - " + stop);
                }
                return writer.ToString();
            }
        }

        public static string AsString(this IList<string> strings)
        {
            if (strings == null) throw new ArgumentNullException(nameof(strings));

            using (var writer = new StringWriter())
            {
                foreach (var @string in strings)
                {
                    writer.WriteLine(@string);
                }
                return writer.ToString();
            }
        }

        public static string AsString(this QueryPlan queryPlan)
        {
            if (queryPlan == null) throw new ArgumentNullException(nameof(queryPlan));

            using (var writer = new StringWriter())
            {
                writer.WriteLine($"QueryPlan: '{queryPlan.Alias}' NodeType: '{queryPlan.NodeType}' PlanRows:{queryPlan.PlanRows} PlanWidth:{queryPlan.PlanWidth} RelationName: '{queryPlan.RelationName}' StartupCost:{queryPlan.StartupCost} TotalCost:{queryPlan.TotalCost}");
                return writer.ToString();
            }
        }
    }
}