using System.Collections.ObjectModel;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.CollectionConverters
{
    internal class CollectionConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(ReadOnlyCollection<>), typeof(Collection<>)];

        public object? Convert(TypeRequest request, object value, IFixtureContext context)
        {
            var target = request.Type;
            if (_types.Contains(target.GetGenericTypeDefinitionOrDefault())
                && value.GetType().GetEnumerableElementType() == target.GenericTypeArguments[0])
            {
                var intermediateType = typeof(List<>).MakeGenericType(target.GetGenericArguments()[0]);
                value = Activator.CreateInstance(intermediateType, value)!;

                return Activator.CreateInstance(target, value);
            }

            return new NoResult();
        }
    }
}
