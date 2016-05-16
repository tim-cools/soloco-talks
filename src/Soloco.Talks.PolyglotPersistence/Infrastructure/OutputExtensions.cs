using System;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence.Infrastructure
{
    public static class OutputExtensions
    {
        public static void BeginTest(this ITestOutputHelper output, string name)
        {
            if (output == null) throw new ArgumentNullException(nameof(output));

            output.WriteLine("");
            output.WriteLine("---------------------------------------------------------------------------------");
            output.WriteLine(name);
            output.WriteLine("---------------------------------------------------------------------------------");
            output.WriteLine("");
            output.WriteLine("");
        }
    }
}