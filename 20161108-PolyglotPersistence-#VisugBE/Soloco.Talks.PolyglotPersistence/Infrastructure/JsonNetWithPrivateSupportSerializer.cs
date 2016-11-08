using System;
using System.IO;
using Baseline;
using JsonNet.PrivateSettersContractResolvers;
using Marten;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Soloco.Talks.PolyglotPersistence.Infrastructure
{
    public class JsonNetWithPrivateSupportSerializer : ISerializer
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

        public JsonNetWithPrivateSupportSerializer()
        {
            EnumStorage = EnumStorage.AsString;
        }

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

        public T FromJson<T>(TextReader reader)
        {
            return (T) _serializer.Deserialize(reader, typeof(T));
        }

        public object FromJson(Type type, TextReader reader)
        {
            return _serializer.Deserialize(new JsonTextReader(reader), type);
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
            private set
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