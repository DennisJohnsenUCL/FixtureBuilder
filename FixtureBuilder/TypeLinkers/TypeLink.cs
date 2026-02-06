namespace FixtureBuilder.TypeLinkers
{
    internal readonly struct TypeLink : ITypeLink
    {
        private readonly Type _inType;
        private readonly Type _outType;

        public TypeLink(Type inType, Type outType)
        {
            _inType = inType;
            _outType = outType;
        }

        public Type? Link(Type target)
        {
            if (target == _inType) return _outType;
            return null;
        }
    }
}
