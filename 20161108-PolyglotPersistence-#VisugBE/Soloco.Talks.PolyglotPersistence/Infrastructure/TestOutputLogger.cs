using System;
using System.Linq;
using Marten;
using Marten.Services;
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

        public void RecordSavedChanges(IDocumentSession session, IChangeSet commit)
        {
            _testOutputHelper.WriteLine($"Persisted {commit.Updated.Count()} updates, {commit.Inserted.Count()} inserts, and {commit.Deleted.Count()} deletions");
        }
    }
}