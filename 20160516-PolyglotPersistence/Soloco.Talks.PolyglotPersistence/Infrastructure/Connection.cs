using System.Configuration;

namespace Soloco.Talks.PolyglotPersistence.Infrastructure
{
    internal static class Connection
    {
        public static string String => ConfigurationManager.ConnectionStrings["test"].ConnectionString;
    }
}