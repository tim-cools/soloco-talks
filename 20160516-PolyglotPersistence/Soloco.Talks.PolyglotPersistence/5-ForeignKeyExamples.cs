using System;
using System.Collections.Generic;
using System.Linq;
using Soloco.Talks.PolyglotPersistence.Infrastructure;
using Soloco.Talks.PolyglotPersistence.TestData;
using Xunit;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence
{
    public class ForeignKeysExamples
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ForeignKeysExamples(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void InvalidForeignKey()
        {
            var store = TestDocumentStore.Create(optionsHandler: options =>
            {
                options.Schema.For<Order>().ForeignKey<Customer>(order => order.CustomerID);
            });

            using (var session = store.OpenSession())
            {
                var order1 = new Order(customerID: new Guid());

                session.Store(order1);
                session.SaveChanges();
            }
        }

        [Fact]
        public void ValidForeignKey()
        {
            var store = TestDocumentStore.Create(optionsHandler: options =>
            {
                options.Schema.For<Order>().ForeignKey<Customer>(order => order.CustomerID);
            });

            using (var session = store.OpenSession())
            {
                var customer = new Customer("customer-1");
                session.Store(customer);

                var order1 = new Order(customer.ID);

                session.Store(order1);
                session.SaveChanges();
            }
        }
    }    
}