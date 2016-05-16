﻿using System;
using System.Collections.Generic;
using System.Linq;
using Soloco.Talks.PolyglotPersistence.Infrastructure;
using Soloco.Talks.PolyglotPersistence.TestData;
using Xunit;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence
{
    public class QueryWithIncludesExamples
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public QueryWithIncludesExamples(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void QuerySingleInclude()
        {
            var store = TestDocumentStore.Create(_testOutputHelper);

            using (var session = store.OpenSession())
            {
                var customer = new Customer("customer-1");
                session.Store(customer);

                var order = new Order(customer.ID);
                order.Complete();

                session.Store(order);
                session.SaveChanges();
            }

            _testOutputHelper.BeginTest("QuerySingleInclude");

            using (var session = store.QuerySession())
            {
                Customer customer = null;

                var firstOrder = session
                    .Query<Order>()
                    .Include<Customer>(order => order.CustomerID, value => customer = value)
                    .FirstOrDefault(route => route.Status == OrderStatus.Completed);

                _testOutputHelper.WriteLine(firstOrder.ToString());
                _testOutputHelper.WriteLine(customer.ToString());
            }
        }

        [Fact]
        public void QueryMultiInclude()
        {
            var store = TestDocumentStore.Create(_testOutputHelper);
            store.AddProductsAndOrders();

            _testOutputHelper.BeginTest("QueryMultiInclude");

            using (var session = store.QuerySession())
            {
                var customers = new Dictionary<Guid, Customer>();

                var orders = session
                    .Query<Order>()
                    .Include(order => order.CustomerID, customers)
                    .Where(route => route.Status == OrderStatus.Completed)
                    .ToList();

                _testOutputHelper.WriteLine(orders.AsString());
                _testOutputHelper.WriteLine(customers.AsString());
            }
        }
    }    
}