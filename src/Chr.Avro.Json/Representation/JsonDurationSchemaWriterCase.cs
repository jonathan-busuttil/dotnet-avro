namespace Chr.Avro.Representation
{
    using System.Text.Json;
    using Chr.Avro.Abstract;

    /// <summary>
    /// Implements a <see cref="JsonSchemaWriter" /> case that matches <see cref="FixedSchema" />s
    /// with <see cref="DurationLogicalType" />.
    /// </summary>
    public class JsonDurationSchemaWriterCase : DurationSchemaWriterCase, IJsonSchemaWriterCase
    {
        /// <summary>
        /// Writes a  <see cref="FixedSchema" /> with <see cref="DurationLogicalType" />.
        /// </summary>
        /// <inheritdoc />
        public virtual JsonSchemaWriterCaseResult Write(Schema schema, Utf8JsonWriter json, bool canonical, JsonSchemaWriterContext context)
        {
            if (schema is FixedSchema fixedSchema && schema.LogicalType is DurationLogicalType)
            {
                if (context.Names.TryGetValue(fixedSchema.FullName, out var existing))
                {
                    if (!schema.Equals(existing))
                    {
                        throw new InvalidSchemaException($"A conflicting schema with the name {fixedSchema.FullName} has already been written.");
                    }

                    json.WriteStringValue(fixedSchema.FullName);
                }
                else
                {
                    context.Names.Add(fixedSchema.FullName, fixedSchema);

                    json.WriteStartObject();
                    json.WriteString(JsonAttributeToken.Name, fixedSchema.FullName);

                    if (!canonical)
                    {
                        if (fixedSchema.Aliases.Count > 0)
                        {
                            json.WritePropertyName(JsonAttributeToken.Aliases);
                            json.WriteStartArray();

                            foreach (var alias in fixedSchema.Aliases)
                            {
                                json.WriteStringValue(alias);
                            }

                            json.WriteEndArray();
                        }
                    }

                    json.WriteString(JsonAttributeToken.Type, JsonSchemaToken.Fixed);

                    if (!canonical)
                    {
                        json.WriteString(JsonAttributeToken.LogicalType, JsonSchemaToken.Duration);
                    }

                    json.WriteNumber(JsonAttributeToken.Size, fixedSchema.Size);
                    json.WriteEndObject();
                }

                return new JsonSchemaWriterCaseResult();
            }
            else
            {
                return JsonSchemaWriterCaseResult.FromException(new UnsupportedSchemaException(schema, $"{nameof(JsonDurationSchemaWriterCase)} can only be applied to {nameof(FixedSchema)}s with {nameof(DurationLogicalType)}."));
            }
        }
    }
}
