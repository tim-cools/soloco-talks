using System.Data;
using Soloco.Talks.PolyglotPersistence.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence.A_IsolationLevels
{   
    public class Examples_A
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Examples_A(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }        

        [Fact]
        public void TwoConcurrentTransactionsDefault()
        {
            var store = TestDocumentStore.Create();

            using (var session = store.OpenSession())
            {
                var accountA = new Account("A", 750);

                session.Store(accountA);
                session.SaveChanges();
            }

            using (var session1 = store.DirtyTrackedSession())
            using (var session2 = store.DirtyTrackedSession())
            {
                var session1AcountA = session1.Load<Account>("A");
                session1AcountA.Substract(500);

                var session2AcountA = session2.Load<Account>("A");
                session2AcountA.Substract(350);

                session1.SaveChanges();
                session2.SaveChanges();
            }
        }








        [Fact]
        public void LoadAccountA()
        {
            var store = TestDocumentStore.Create();

            using (var session = store.OpenSession())
            {
                var accountA = session.Load<Account>("A");

                _testOutputHelper.WriteAsJson(accountA);
            }
        }











        [Fact]
        public void TwoConcurrentTransactionsSerialized()
        {
            var store = TestDocumentStore.Create();

            using (var session = store.OpenSession())
            {
                var accountA = new Account("A", 750);

                session.Store(accountA);
                session.SaveChanges();
            }

            using (var session1 = store.DirtyTrackedSession(IsolationLevel.Serializable))
            using (var session2 = store.DirtyTrackedSession(IsolationLevel.Serializable))
            {
                var session1AcountA = session1.Load<Account>("A");
                session1AcountA.Substract(500);

                var session2AcountA = session2.Load<Account>("A");
                session2AcountA.Substract(350);

                session1.SaveChanges();
                session2.SaveChanges();                
            }
        }
    }
}
