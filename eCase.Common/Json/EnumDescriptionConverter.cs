using System;
using Newtonsoft.Json;

namespace eCase.Common.Json
{
    public class EnumDescriptionConverter : JsonConverter
    {
        public EnumDescriptionConverter()
        {
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Enum e = (Enum)value;

            writer.WriteValue(e.GetEnumDescription());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!objectType.IsValueType || Nullable.GetUnderlyingType(objectType) != null)
                return null;

            return Activator.CreateInstance(objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            Type t = (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>))
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            return t.IsEnum;
        }
    }
}
