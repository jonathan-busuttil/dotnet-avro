namespace Chr.Avro.Serialization
{
    using System;
    using System.Linq.Expressions;
    using Chr.Avro.Abstract;
    using Chr.Avro.Resolution;

    /// <summary>
    /// Implements a <see cref="BinarySerializerBuilder" /> case that matches <see cref="LongSchema" />
    /// and attempts to map it to any provided type.
    /// </summary>
    public class BinaryLongSerializerBuilderCase : LongSerializerBuilderCase, IBinarySerializerBuilderCase
    {
        /// <summary>
        /// Builds a <see cref="BinarySerializer{T}" /> for a <see cref="LongSchema" />.
        /// </summary>
        /// <returns>
        /// A successful <see cref="BinarySerializerBuilderCaseResult" /> if <paramref name="schema" />
        /// is a <see cref="LongSchema" />; an unsuccessful <see cref="BinarySerializerBuilderCaseResult" />
        /// otherwise.
        /// </returns>
        /// <exception cref="UnsupportedTypeException">
        /// Thrown when the resolved <see cref="Type" /> cannot be converted to <see cref="long" />.
        /// </exception>
        /// <inheritdoc />
        public virtual BinarySerializerBuilderCaseResult BuildExpression(Expression value, TypeResolution resolution, Schema schema, BinarySerializerBuilderContext context)
        {
            if (schema is LongSchema longSchema)
            {
                var writeInteger = typeof(BinaryWriter)
                    .GetMethod(nameof(BinaryWriter.WriteInteger), new[] { typeof(long) });

                try
                {
                    return BinarySerializerBuilderCaseResult.FromExpression(
                        Expression.Call(context.Writer, writeInteger, BuildConversion(value, typeof(long))));
                }
                catch (InvalidOperationException exception)
                {
                    throw new UnsupportedTypeException(resolution.Type, $"Failed to map {longSchema} to {resolution.Type}.", exception);
                }
            }
            else
            {
                return BinarySerializerBuilderCaseResult.FromException(new UnsupportedSchemaException(schema, $"{nameof(BinaryLongSerializerBuilderCase)} can only be applied to {nameof(LongSchema)}s."));
            }
        }
    }
}
