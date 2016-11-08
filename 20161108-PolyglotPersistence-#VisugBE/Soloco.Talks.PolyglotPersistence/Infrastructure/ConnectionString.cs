using System;
using System.Configuration;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence.Infrastructure
{
    internal static class ConnectionString
    {
        public static string Local => GetConnectionString("test");
        public static string Master => GetConnectionString("master");
        public static string Slave => GetConnectionString("slave");

        private static string GetConnectionString(string name)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name];
            if (connectionString == null)
            {
                throw new InvalidOperationException($"Connection string nog found: {name}");
            }
            return connectionString.ConnectionString;
        }
    }
}