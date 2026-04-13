using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Assignment.ValueProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for array types.
    /// </summary>
    /// <remarks>
    /// Creates arrays with a fixed length of 10 elements, initialized to their default values.
    /// Only handles requests where <see cref="FixtureRequest.Type"/> is an array type;
    /// returns <see langword="null"/> for all other types.
    /// </remarks>
    internal class ArrayProvider : IValueProvider
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type.IsArray && request.Type.GetArrayRank() == 1)
            {
                var elementType = request.Type.GetElementType()!;
                var instance = Array.CreateInstance(elementType, 10);
                return instance;
            }

            return new NoResult();
        }
    }
}
