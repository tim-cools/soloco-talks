using System;
using Marten;

namespace Soloco.Talks.PolyglotPersistence.C_Queries
{
    public static class TestDataExtensions
    {
        internal static void AddRoutes(this IDocumentStore store, int number)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));

            using (var session = store.OpenSession())
            {
                for (var index = 0; index < number; index++)
                {
                    var route = new Route();
                    if (index % 2 == 0)
                    {
                        route.Plan(new DateTime(2016, 05, 26).AddDays(index));
                    }
                    route.AddStop("Home", new Position(51.197894m, 4.481736m));
                    route.AddStop("WooRank", new Position(50.828417m, 4.400963m));
                    route.AddStop("Home", new Position(51.197894m, 4.481736m));

                    session.Store(route);
                }

                session.SaveChanges();
            }
        }

        internal static void AddDinners(this IDocumentStore store, int number)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));

            using (var session = store.OpenSession())
            {
                for (var index = 0; index < number; index++)
                {
                    var route = new Dinner
                    {
                        Title = "Dinner " + index,
                        Address = "some street "+ 123,
                        EventDate = DateTime.Now.AddDays(index),
                        HostedBy = index % 2 == 0 ? "John" : "Peter",
                    };

                    session.Store(route);
                }

                session.SaveChanges();
            }
        }

    }
}