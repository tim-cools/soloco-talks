using System;
using System.Linq;
using Marten;
using Npgsql;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence.Infrastructure
{
    public class TestOutputLogger : IMartenLogger, IMartenSessionLogger
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestOutputLogger(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public IMartenSessionLogger StartSession(IQuerySession session)
        {
            return this;
        }

        public void SchemaChange(string sql)
        {
            _testOutputHelper.WriteLine("Executing DDL change:");
            _testOutputHelper.WriteLine(sql);
            _testOutputHelper.WriteLine(string.Empty);
        }

        public void LogSuccess(NpgsqlCommand command)
        {
            _testOutputHelper.WriteLine(command.CommandText);
        }

        public void LogFailure(NpgsqlCommand command, Exception ex)
        {
            _testOutputHelper.WriteLine("Postgresql command failed!");
            _testOutputHelper.WriteLine(command.CommandText);
            _testOutputHelper.WriteLine(ex.ToString());
        }

        public void RecordSavedChanges(IDocumentSession session)
        {
            var lastCommit = session.LastCommit;
            _testOutputHelper.WriteLine($"Persisted {lastCommit.Updated.Count()} updates, {lastCommit.Inserted.Count()} inserts, and {lastCommit.Deleted.Count()} deletions");
        }
    }
}