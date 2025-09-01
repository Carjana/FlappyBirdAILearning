using System;
using Newtonsoft.Json;
using UnityEngine;

namespace AI
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WriteEndObject();
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            float x = 0;
            float y = 0;

            // Sicherstellen, dass das Token ein Objekt ist
            if (reader.TokenType == JsonToken.StartObject)
            {
                while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string propertyName = (string)reader.Value;
                        reader.Read(); // Zum Wert springen

                        switch (propertyName)
                        {
                            case "x":
                                x = Convert.ToSingle(reader.Value);
                                break;
                            case "y":
                                y = Convert.ToSingle(reader.Value);
                                break;
                        }
                    }
                }
            }
            return new Vector2(x, y);
        }
    }
}