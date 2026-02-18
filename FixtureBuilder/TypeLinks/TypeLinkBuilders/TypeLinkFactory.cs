namespace FixtureBuilder.TypeLinks.TypeLinkBuilders
{
    internal class TypeLinkFactory : ITypeLinkFactory
    {
        public TypeLinkFactory() { }

        public ITypeLink CreateDefaultTypeLink()
        {
            var typeLink = new TypeLinkBuilder()
                .WithStrategies()
                    .AddEnumerableTypeLinks()
                    .AddDictionaryTypeLinks()
                    .And()
                .WithValidation()
                .Build();

            return typeLink;
        }
    }
}
