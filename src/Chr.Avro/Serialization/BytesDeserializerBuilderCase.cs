namespace Chr.Avro.Serialization
{
    using System;
    using System.Linq.Expressions;
    using Chr.Avro.Abstract;

    /// <summary>
    /// Provides a base implementation for deserializer builder cases that match <see cref="BytesSchema" />.
    /// </summary>
    public abstract class BytesDeserializerBuilderCase : DeserializerBuilderCase
    {
        /// <remarks>
        /// This override includes additional conditions to handle conversions to types that can be
        /// idiomatically represented as byte arrays. If none match, the base implementation is
        /// used.
        /// </remarks>
        /// <inheritdoc />
        protected override Expression BuildConversion(Expression value, Type target)
        {
            if (target == typeof(Guid) || target == typeof(Guid?))
            {
                var guidConstructor = typeof(Guid)
                    .GetConstructor(new[] { value.Type });

                value = Expression.New(guidConstructor, value);
            }

            return base.BuildConversion(value, target);
        }
    }
}
