using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters.Casters
{
    internal static class ElementConverter
    {
        public static object? Convert(object? item, Type targetType, Type rootType, IFixtureContext context)
        {
            if (targetType.IsValueType && Nullable.GetUnderlyingType(targetType) is null && item is null)
                throw new InvalidCastException($"Cannot assign a null element to a collection with non-nullable element type {targetType.Name}.");

            if (item == null) return null;
            if (targetType.IsAssignableFrom(item.GetType())) return item;

            var linked = context.UnwrapAndLink(targetType, rootType);
            var request = new TypeRequest(linked, rootType);
            var converted = context.Converter.Convert(request, item, targetType, context);
            if (converted is NoResult)
            {
                throw new InvalidCastException(
                    $"Cannot convert element of type {item.GetType().Name} to {targetType.Name}.");
            }

            return converted;
        }
    }
}
