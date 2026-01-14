using System;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Marten.Schema;
using Marten.Schema.Identity;
using Should;
using Soloco.Talks.PolyglotPersistence.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence.Z_Replication
{
    // not sure what I was trying to do here :-) definitely look into the default db replication options before
    // attempting to do something like this yourself...
    public class Examples
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Examples(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void LoadAggregateById()
        {
            var slaveStore = TestDocumentStore.Create(ConnectionString.Slave,
                optionsHandler: options =>
                {
                    options.UpsertType = PostgresUpsertType.Legacy;
                    options.AutoCreateSchemaObjects = AutoCreate.None;
                }, clear: false);
            var masterStore = TestDocumentStore.Create(ConnectionString.Master, optionsHandler: options => options.UpsertType = PostgresUpsertType.Legacy);

            var tempRouteId = CombGuidIdGeneration.New();
            var routeId = CombGuidIdGeneration.New();

            using (var massWriter = new Writer(masterStore))
            using (var listenerMaster = new Listener("master", _testOutputHelper, masterStore, routeId, tempRouteId))
            using (var listenerSlave = new Listener("slave", _testOutputHelper, slaveStore, routeId, tempRouteId))
            using (var session = masterStore.OpenSession())
            {

                session.Store(new Route { ID = tempRouteId });
                session.SaveChanges();

                massWriter.Start();

                Thread.Sleep(5000);

                listenerSlave.Stored();
                listenerMaster.Stored();

                session.Store(new Route { ID = routeId });
                session.SaveChanges();

                listenerMaster.Start();
                listenerSlave.Start();


                _testOutputHelper.WriteLine($"Route stored");

                Thread.Sleep(500);
            }
        }
    }

    public class Writer : IDisposable
    {
        private readonly IDocumentStore _store;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;

        public Writer(IDocumentStore masterStore)
        {
            _store = masterStore;

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = new CancellationToken();
        }

        public void Start()
        {
            for (int i = 0; i < 15; i++)
            {
                Task.Run(Write, _cancellationToken);
            }
        }

        private async Task Write()
        {
            using (var session = _store.LightweightSession())
            {
                while (true)
                {
                    if (_cancellationToken.IsCancellationRequested) return;

                    session.Store( new Route());
                    await session.SaveChangesAsync(_cancellationToken);
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }

    public class Listener : IDisposable
    {
        private readonly IDocumentStore _store;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;
        private readonly ITestOutputHelper _output;
        private bool _found;
        private readonly string _name;
        private Guid _dummyId;
        private readonly Guid _routeId;

        private long _numberOfReads;
        private long _storedAt;

        public Listener(string name, ITestOutputHelper testOutputHelper, IDocumentStore store, Guid id, Guid dummyId)
        {
            _output = testOutputHelper;
            _store = store;
            _routeId = id;
            _dummyId = dummyId;
            _name = name;

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = new CancellationToken();
        }

        public void Stored()
        {
            var reads = Interlocked.Read(ref _numberOfReads);
            Interlocked.Exchange(ref _storedAt, reads);

            //_output.WriteLine($"Stored after {reads} reads of {_name}");
        }

        public void Start()
        {
            _numberOfReads = 0;
            Task.Run(Listen, _cancellationToken);
        }

        private async Task Listen()
        {
            using (var session = _store.QuerySession())
            {
                var dummy = await session.LoadAsync<Route>(_dummyId, _cancellationToken);
                dummy.ShouldNotBeNull();

                while (true)
                {
                    if (_cancellationToken.IsCancellationRequested) return;

                    var dateTime = DateTime.Now;
                    var route = await session.LoadAsync<Route>(_routeId, _cancellationToken);
                    if (route != null)
                    {
                        var numberOfReads = Interlocked.Read(ref _numberOfReads);
                        var storedAt = Interlocked.Read(ref _storedAt);

                        _output.WriteLine(
                            $"Document read from {_name} database {numberOfReads - storedAt} reads after the write. (Read time: {(DateTime.Now - dateTime).TotalMilliseconds}, total reads: {numberOfReads}, stored after: {storedAt})");
                        _found = true;

                        return;
                    }
                    Interlocked.Increment(ref _numberOfReads);
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            if (!_found)
            {
                _output.WriteLine($"Route never read from {_name} database");
            }
        }
    }
}
