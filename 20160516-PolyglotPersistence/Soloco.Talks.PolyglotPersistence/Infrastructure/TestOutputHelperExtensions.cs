using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence.Infrastructure
{
    public static class TestOutputHelperExtensions
    {
        public static ITestOutputHelper Write(this ITestOutputHelper output, string message)
        {
            if (output == null) throw new ArgumentNullException(nameof(output));

            output.WriteLine(message);

            return output;
        }

        public static ITestOutputHelper NewLine(this ITestOutputHelper output)
        {
            if (output == null) throw new ArgumentNullException(nameof(output));

            output.WriteLine(string.Empty);

            return output;
        }

        public static ITestOutputHelper WriteFormattedJson(this ITestOutputHelper output, string json)
        {
            if (output == null) throw new ArgumentNullException(nameof(output));

            var jobject = JObject.Parse(json);
            var formatted = JsonConvert.SerializeObject(jobject, Formatting.Indented);
            output.WriteLine(formatted);

            return output;
        }

        public static ITestOutputHelper WriteAsJson(this ITestOutputHelper output, object value)
        {
            if (output == null) throw new ArgumentNullException(nameof(output));

            var formatted = JsonConvert.SerializeObject(value, Formatting.Indented);
            output.WriteLine(formatted);

            return output;
        }

        public static ITestOutputHelper WriteAsJson<T>(this ITestOutputHelper output, IList<T> values)
        {
            if (output == null) throw new ArgumentNullException(nameof(output));

            output.WriteLine("[");

            foreach (var value in values)
            {
                var formatted = JsonConvert.SerializeObject(value, Formatting.Indented);
                output.WriteLine($"  {value.GetType()} : {formatted}");
            }

            output.WriteLine("]");

            return output;
        }
    }
}