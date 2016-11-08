using System;
using Soloco.Talks.PolyglotPersistence.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence.E_ForeignKeyConstraints
{
    public class ForeignKeysExample_E
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ForeignKeysExample_E(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void InvalidForeignKey()
        {
            var store = TestDocumentStore.Create(optionsHandler: options =>
            {
                options.Schema.For<Order>()
                    .ForeignKey<Customer>(order => order.CustomerId);
            });

            using (var session = store.OpenSession())
            {
                var order1 = new Order(customerId: new Guid());

                session.Store(order1);
                session.SaveChanges();
            }
        }

        [Fact]
        public void ValidForeignKey()
        {
            var store = TestDocumentStore.Create(optionsHandler: options =>
            {
                options.Schema.For<Order>().ForeignKey<Customer>(order => order.CustomerId);
            });

            using (var session = store.OpenSession())
            {
                var customer = new Customer("customer-1");
                session.Store(customer);

                var order1 = new Order(customerId: customer.Id);
                session.Store(order1);

                session.SaveChanges();
            }
        }
    }    
}