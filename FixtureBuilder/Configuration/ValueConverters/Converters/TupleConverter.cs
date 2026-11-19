using System.Runtime.CompilerServices;
using FixtureBuilder.Configuration.ValueConverters.Casters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters.Converters
{
    /// <summary>
    /// Converts a tuple to a tuple type of the same arity by converting each element to the matching element type of the target.
    /// This covers C#'s implicit tuple conversions, which <see cref="ImplicitConverter"/> cannot apply as the runtime binder does not support them.
    /// <see cref="ValueTuple"/> and <see cref="Tuple"/> types are both supported, as value and as target, in any combination.
    /// The result is built as the linked type of the request, so a nullable tuple target gets its underlying tuple type.
    /// Arity counts the elements of nested rest tuples, so tuples with more than seven elements are converted element by element and rebuilt with the nesting of the target.
    /// An element assignable to its target element type is kept; any other element is converted to its linked type by the context's converter, so type links and registered converters apply to elements.
    /// Element conversions by implicit operator or numeric widening are applied by <see cref="ImplicitConverter"/>, so they require <see cref="FixtureOptions.AllowImplicitConversion"/> to be enabled.
    /// A <see langword="null"/> element for a non-nullable value type, or an element that no converter handles, throws an <see cref="InvalidCastException"/>.
    /// A target that is not a generic <see cref="ValueTuple"/> or <see cref="Tuple"/> type, including other types implementing <see cref="ITuple"/>,
    /// a value that does not implement <see cref="ITuple"/>, or a value of a different arity, returns <see cref="NoResult"/>.
    /// </summary>
    internal class TupleConverter : IValueConverter
    {
        public object? Convert(ConverterRequest request, object value, IFixtureContext context)
        {
            if (request.Type.IsGenericType
                && request.Type.Namespace == "System"
                && request.Type.IsAssignableTo(typeof(ITuple))
                && value is ITuple tuple)
            {
                var elementTypes = GetElementTypes(request.Type);

                if (elementTypes.Count != tuple.Length) return NoResult.Instance;

                var resultArray = new object?[tuple.Length];

                for (int i = 0; i < tuple.Length; i++)
                {
                    var element = tuple[i];

                    var tupleArg = elementTypes[i];

                    resultArray[i] = ElementConverter.Convert(element, tupleArg, request.RootType, context);
                }

                return Build(request.Type, resultArray, 0);
            }

            return NoResult.Instance;
        }

        private static List<Type> GetElementTypes(Type tupleType)
        {
            var tupleArgs = tupleType.GetGenericArguments();
            return tupleArgs.Length < 8 ? [.. tupleArgs] : [.. tupleArgs[..7], .. GetElementTypes(tupleArgs[7])];
        }

        private static object? Build(Type tupleType, object?[] values, int offset)
        {
            var args = tupleType.GetGenericArguments();
            var ctorArgs = new object?[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                ctorArgs[i] = i == 7 ? Build(args[7], values, offset + 7) : values[offset + i];
            }

            return Activator.CreateInstance(tupleType, ctorArgs);
        }
    }
}
