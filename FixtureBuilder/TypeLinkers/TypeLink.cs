namespace FixtureBuilder.TypeLinkers
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

            _inType = inType;
            _outType = outType;
        }

        public Type? Link(Type target)
        {
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
    }
}
