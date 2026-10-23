using FixtureBuilder.Core;

namespace FixtureBuilder.Assignment.TypeLinks
{
    internal readonly struct TypeLink : ITypeLink
    {
        private readonly Type _inType;
        private readonly Type _outType;

        public TypeLink(Type inType, Type outType)
        {
            ArgumentNullException.ThrowIfNull(inType);
            ArgumentNullException.ThrowIfNull(outType);

            if (inType.IsGenericTypeDefinition != outType.IsGenericTypeDefinition)
                throw new ArgumentException("Both TypeLink types must be either generic type definitions or concrete types.");

            if (!IsLinkable(inType, outType))
            {
                throw new ArgumentException($"Output type {outType.Name} must be assignable to input type {inType.Name} for TypeLink to be valid, " +
                    "otherwise objects of output type cannot be assigned to members of input type.");
            }

            _inType = inType;
            _outType = outType;
        }

        public Type? Link(FixtureRequest request)
        {
            var target = request.Type;

            if (target == _inType) return _outType;

            if (_inType.IsGenericTypeDefinition
                && _outType.IsGenericTypeDefinition
                && target.IsGenericType
                && target.GetGenericTypeDefinition() == _inType)
            {
                var genericArgs = target.GenericTypeArguments;

                return _outType.MakeGenericType(genericArgs);
            }

            return null;
        }

        private static bool IsLinkable(Type inType, Type outType)
        {
            if (!inType.IsGenericTypeDefinition) return inType.IsAssignableFrom(outType);

            var outArgs = outType.GetGenericArguments();
            if (inType.GetGenericArguments().Length != outArgs.Length) return false;

            try
            {
                return inType.MakeGenericType(outArgs).IsAssignableFrom(outType);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
