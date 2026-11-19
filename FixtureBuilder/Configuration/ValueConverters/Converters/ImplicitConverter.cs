using System.Collections.Concurrent;
using System.Linq.Expressions;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Microsoft.CSharp.RuntimeBinder;

namespace FixtureBuilder.Configuration.ValueConverters.Converters
{
    /// <summary>
    /// Converts a value to the target type with C#'s implicit conversion rules, applied at runtime by the C# runtime binder.
    /// This covers what a plain assignment allows: user-defined implicit operators, numeric widening, nullable wrapping and combinations of these.
    /// A value type is converted to the original type, so a nullable target keeps its nullability; any other type is converted to the linked type, so type links are honored.
    /// An interface target returns <see cref="NoResult"/>, as C# never applies a user-defined conversion to an interface type.
    /// An array value is bound with its innermost element type as the context, as the runtime binder cannot use an array type as its context.
    /// Native integer (<see langword="nint"/>, <see langword="nuint"/>) and tuple conversions are not applied, as the runtime binder does not support them.
    /// A missing or ambiguous conversion returns <see cref="NoResult"/>; an exception thrown by an implicit operator is propagated.
    /// The compiled conversion is cached per target and source type, including conversions that do not exist.
    /// Requires <see cref="FixtureOptions.AllowImplicitConversion"/> to be enabled.
    /// </summary>
    internal class ImplicitConverter : IValueConverter
    {
        private static readonly ConcurrentDictionary<(Type Target, Type Source), Func<object, object?>?> _converters = [];

        public object? Convert(ConverterRequest request, object value, IFixtureContext context)
        {
            if (context.OptionsFor(request.RootType).AllowImplicitConversion)
            {
                var target = request.OriginalType.IsValueType ? request.OriginalType : request.Type;

                if (target.IsInterface) return NoResult.Instance;

                return AttemptConvert(target, value.GetType(), value);
            }
            return NoResult.Instance;
        }

        private static object? AttemptConvert(Type target, Type source, object value)
        {
            var converter = _converters.GetOrAdd((target, source), static key => MakeConverter(key.Target, key.Source));

            if (converter is null) return NoResult.Instance;

            try
            {
                return converter.Invoke(value);
            }
            catch (RuntimeBinderException)
            {
                _converters[(target, source)] = null;
                return NoResult.Instance;
            }
        }

        private static Func<object, object?> MakeConverter(Type target, Type source)
        {
            var context = source;
            while (context.IsArray) context = context.GetElementType()!;

            var binder = Binder.Convert(CSharpBinderFlags.None, target, context);
            var value = Expression.Parameter(typeof(object), "value");
            var body = Expression.Convert(Expression.Dynamic(binder, target, value), typeof(object));
            return Expression.Lambda<Func<object, object?>>(body, value).Compile();
        }
    }
}
