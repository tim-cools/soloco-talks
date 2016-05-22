using System;
using Marten;

namespace Soloco.Talks.PolyglotPersistence.TestData
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

        internal static void AddProductsAndOrders(this IDocumentStore store)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));

            const int numberOfCustomers = 10;
            const int numberOfProducts = 10;
            const int numberOfOrders = 10;

            var customers = new Customer[numberOfCustomers];
            var products = new Product[numberOfOrders];

            using (var session = store.OpenSession())
            {
                for (var index = 0; index < numberOfCustomers; index++)
                {
                    var customer = new Customer("customer-" + index);
                    customers[index] = customer;
                    session.Store(customer);
                }

                for (var index = 0; index < numberOfProducts; index++)
                {
                    var product = new Product("product-" + index);
                    products[index] = product;
                    session.Store(product);
                }

                for (var index = 0; index < numberOfOrders; index++)
                {
                    var order = new Order(customers[index % numberOfCustomers].ID);
                    order.AddProduct(products[index % numberOfProducts], index + 1);
                    if (index % 2 == 0)
                    {
                        order.Complete();
                    }
                    session.Store(order);
                }

                session.SaveChanges();
            }
        }
    }
}