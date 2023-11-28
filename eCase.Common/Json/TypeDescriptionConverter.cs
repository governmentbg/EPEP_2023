using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using Newtonsoft.Json;

namespace eCase.Common.Json
{
    public class TypeDescriptionConverter : JsonConverter
    {
        private static readonly ConcurrentDictionary<Type, string> cache = new ConcurrentDictionary<Type, string>();

        public TypeDescriptionConverter()
        {
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            if (!typeof(Type).IsInstanceOfType(value))
            {
                writer.WriteNull();
                return;
            }

            Type t = (Type)value;

            string cachedDescription;
            if (!cache.TryGetValue(t, out cachedDescription))
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])t.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0)
                {
                    cachedDescription = attributes[0].Description;
                }
                else
                {
                    cachedDescription = t.Name;
                }

                cache.TryAdd(t, cachedDescription);
            }

            writer.WriteValue(cachedDescription);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Type);
        }
    }
}
