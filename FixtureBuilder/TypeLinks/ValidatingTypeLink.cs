namespace FixtureBuilder.TypeLinks
{
    internal class ValidatingTypeLink : ITypeLink
    {
        private readonly ITypeLink _inner;

        public ValidatingTypeLink(ITypeLink inner)
        {
            ArgumentNullException.ThrowIfNull(inner);

            _inner = inner;
        }

        public Type? Link(Type target)
        {
            ArgumentNullException.ThrowIfNull(target);

            return _inner.Link(target);
        }
    }
}
