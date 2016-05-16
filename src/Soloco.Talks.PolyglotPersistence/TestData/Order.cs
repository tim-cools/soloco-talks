using System;
using System.Collections.Generic;

namespace Soloco.Talks.PolyglotPersistence.TestData
{
    public class Customer
    {
        public Guid ID { get; set; }

        public string Name { get; }

        public Customer(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"Customer ({ID}): Name: {Name}";
        }
    }

    public class Order
    {
        private readonly List<OrderLine> _lines = new List<OrderLine>();

        public Guid ID { get; set; }
        public Guid CustomerID { get; private set; }
        public OrderStatus Status { get; private set; }

        public IEnumerable<OrderLine> Lines => _lines;

        public Order(Guid customerID)
        {
            CustomerID = customerID;
        }

        public void AddProduct(Product product, int amount)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (amount <= 0)
            {
                throw new InvalidOperationException("Amount must be greather than 0.");
            }

            var line = new OrderLine(product.ID, amount);
            _lines.Add(line);
        }

        public void Complete()
        {
            Status = OrderStatus.Completed;
        }

        public override string ToString()
        {
            return $"Order ({ID}): CustomerID: {CustomerID}, Status: {Status}";
        }
    }

    public enum OrderStatus
    {
        Open,
        Completed
    }

    public class Product
    {
        public Guid ID { get; set; }
        public string Name { get; private set; }

        public Product(string name)
        {
            Name = name;
        }
    }

    public class OrderLine
    {
        public Guid ID { get; set; }
        public Guid ProductId { get; private set; }
        public int Amount { get; private set; }

        public OrderLine(Guid productId, int amount)
        {
            ProductId = productId;
            Amount = amount;
        }
    }
}
