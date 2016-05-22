using System;
using System.IO;
using Baseline;
using Jil;
using JsonNet.PrivateSettersContractResolvers;
using Marten;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xunit.Abstractions;

namespace Soloco.Talks.PolyglotPersistence.Infrastructure
{
    public static class TestDocumentStore
    {
        public static IDocumentStore CreateSimple()
        {
            var documentStore = DocumentStore.For(options =>
            {
                options.Connection(ConnectionString.Local);
                options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                options.Serializer(new JsonNetSerializer());
            });

            documentStore.Advanced.Clean.CompletelyRemoveAll();

            return documentStore;
        }

        public static IDocumentStore Create(string connectionString = null, ITestOutputHelper testOutputHelper = null, Action<StoreOptions> optionsHandler = null, bool clear = true)
        {
            var documentStore = DocumentStore.For(options =>
            {
                options.Connection(connectionString ?? ConnectionString.Local);
                options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                options.Serializer(new JsonNetSerializer());

                if (testOutputHelper != null)
                {
                    options.Logger(new TestOutputLogger(testOutputHelper));
                }

                optionsHandler?.Invoke(options);
            });

            if (clear)
            {
                documentStore.Advanced.Clean.CompletelyRemoveAll();
            }

            return documentStore;
        }
    }

    public class JilSerializer : ISerializer
    {
        private readonly Options _options
            = new Options(dateFormat: DateTimeFormat.ISO8601, includeInherited: true);

        public string ToJson(object document)
        {
            return JSON.Serialize(document, _options);
        }

        public T FromJson<T>(string json)
        {
            return JSON.Deserialize<T>(json, _options);
        }

        public T FromJson<T>(Stream stream)
        {
            return JSON.Deserialize<T>(new StreamReader(stream), _options);
        }

        public object FromJson(Type type, string json)
        {
            return JSON.Deserialize(json, type, _options);
        }

        public string ToCleanJson(object document)
        {
            return ToJson(document);
        }

        public EnumStorage EnumStorage => EnumStorage.AsString;
    }

    public class JsonNetSerializer : ISerializer
    {
        private readonly JsonSerializer _clean = new JsonSerializer
        {
            TypeNameHandling = TypeNameHandling.None,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            ContractResolver = new PrivateSetterContractResolver()
        };

        private readonly JsonSerializer _serializer = new JsonSerializer
        {
            TypeNameHandling = TypeNameHandling.Auto,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
            ContractResolver = new PrivateSetterContractResolver()
        };

        public string ToJson(object document)
        {
            var writer = new StringWriter();
            _serializer.Serialize(writer, document);

            return writer.ToString();
        }

        public T FromJson<T>(string json)
        {
            return _serializer.Deserialize<T>(new JsonTextReader(new StringReader(json)));
        }

        public object FromJson(Type type, string json)
        {
            return _serializer.Deserialize(new JsonTextReader(new StringReader(json)), type);
        }

        public string ToCleanJson(object document)
        {
            var writer = new StringWriter();
            _clean.Serialize(writer, document);

            return writer.ToString();
        }

        private EnumStorage _enumStorage = EnumStorage.AsInteger;

        public EnumStorage EnumStorage
        {
            get
            {
                return _enumStorage;
            }
            set
            {
                _enumStorage = value;

                if (value == EnumStorage.AsString)
                {
                    var converter = new StringEnumConverter();
                    _serializer.Converters.Add(converter);
                    _clean.Converters.Add(converter);
                }
                else
                {
                    _serializer.Converters.RemoveAll(x => x is StringEnumConverter);
                    _clean.Converters.RemoveAll(x => x is StringEnumConverter);
                }
            }
        }
    }
}