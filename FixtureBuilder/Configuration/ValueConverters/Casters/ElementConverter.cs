using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters.Casters
{
    internal static class ElementConverter
    {
        public static object? Convert(object? item, Type targetType, Type rootType, IFixtureContext context)
        {
            if (item == null) return null;
            if (targetType.IsAssignableFrom(item.GetType())) return item;

            var request = new TypeRequest(targetType, rootType);
            var converted = context.Converter.Root.Convert(request, item, context);
            if (converted is NoResult)
                throw new InvalidCastException(
                    $"Cannot convert element of type {item.GetType().Name} to {targetType.Name}.");
            return converted;
        }
    }
}
