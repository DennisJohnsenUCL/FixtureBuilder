using FixtureBuilder.Extensions;
using FixtureBuilder.Helpers;

namespace FixtureBuilder.ValueConverters
{
    internal class MutableGenericCollectionConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(List<>)];

        public object? Convert(Type target, object value)
        {
            var sourceType = value.GetType();

            if (sourceType.Implements(typeof(IEnumerable<>))
                && _types.Contains(target.GetGenericTypeDefinitionOrDefault())
                && sourceType.GenericTypeArguments[0] == target.GenericTypeArguments[0]
                )
            {
                return InstantiationHelpers.UseConstructor(target, value);
            }
            return null;
        }
    }
}
