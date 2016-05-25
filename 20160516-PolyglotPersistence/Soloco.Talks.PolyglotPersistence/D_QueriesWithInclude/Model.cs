using System;
using System.Collections.Generic;
using Marten;

namespace Soloco.Talks.PolyglotPersistence.D_QueriesWithInclude
{
    public class Customer
    {
        public Guid Id { get; private set; }

        public string Name { get; }

        public Customer(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"Customer ({Id}): Name: {Name}";
        }
    }

    public class Order
    {
        private readonly List<OrderLine> _lines = new List<OrderLine>();

        public Guid Id { get; private set; }
        public Guid CustomerId { get; }
        public OrderStatus Status { get; private set; }

        public IEnumerable<OrderLine> Lines => _lines;

        public Order(Guid customerID)
        {
            CustomerId = customerID;
        }

        public void AddProduct(Product product, int amount)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (amount <= 0)
            {
                throw new InvalidOperationException("Amount must be greather than 0.");
            }

            var line = new OrderLine(product.Id, amount);
            _lines.Add(line);
        }

        public void Complete()
        {
            Status = OrderStatus.Completed;
        }

        public override string ToString()
        {
            return $"Order ({Id}): CustomerID: {CustomerId}, Status: {Status}";
        }
    }

    public enum OrderStatus
    {
        Open,
        Completed
    }

    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }


        public Product(string name)
        {
            Name = name;
        }
    }

    public class OrderLine
    {
        public Guid ID { get; private set; }
        public Guid ProductId { get; private set; }
        public int Amount { get; private set; }

        public OrderLine(Guid productId, int amount)
        {
            ProductId = productId;
            Amount = amount;
        }
    }

    public static class TestDataExtensions
    {
        internal static void AddProductsAndOrders(this IDocumentStore store)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));

            const int numberOfCustomers = 10;
            const int numberOfProducts = 10;
            const int numberOfOrders = 10;

            var customers = new Customer[numberOfCustomers];
            var products = new Product[numberOfProducts];

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
                    var order = new Order(customers[index % numberOfCustomers].Id);
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
