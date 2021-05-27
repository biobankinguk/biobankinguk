using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Biobanks.Submissions.Api.JsonConverters
{
    /// <summary>
    /// A System.Text.Json converter that will parse integers into strings
    /// such that JSON string fields can accept numeric types
    /// </summary>
    public class JsonNumberAsStringConverter : JsonConverter<string>
    {
        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
            => typeToConvert == typeof(string);

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            => writer.WriteStringValue(value);

        /// <inheritdoc />
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long number))
                {
                    return number.ToString(CultureInfo.InvariantCulture);
                }

                if (reader.TryGetDouble(out double doubleNumber))
                {
                    return doubleNumber.ToString(CultureInfo.InvariantCulture);
                }
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }

            using (var document = JsonDocument.ParseValue(ref reader))
            {
                return document.RootElement.Clone().ToString();
            }
        }
    }
}
