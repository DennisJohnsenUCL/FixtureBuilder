namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class AddTypeLinkTests
    {
        private class GenericImplementationClass<T> : IGenericInterface<T>;

        #region Linking

        [Test]
        public void TypeParameters_LinksTypes()
        {
            var factory = new FixtureFactory().AddTypeLink<IMarkerInterface, MarkerImplementationClass>();

            var result = factory.New<InterfaceParameterClass>().Build();

            Assert.That(result.Dependency, Is.TypeOf<MarkerImplementationClass>());
        }

        [Test]
        public void TypeArguments_LinksTypes()
        {
#pragma warning disable CA2263 // Prefer generic overload when type is known
            var factory = new FixtureFactory().AddTypeLink(typeof(IMarkerInterface), typeof(MarkerImplementationClass));
#pragma warning restore CA2263 // Prefer generic overload when type is known

            var result = factory.New<InterfaceParameterClass>().Build();

            Assert.That(result.Dependency, Is.TypeOf<MarkerImplementationClass>());
        }

        private class GenericInterfaceParameterClass(IGenericInterface<int> dependency)
        {
            public IGenericInterface<int> Dependency { get; } = dependency;
        }

        [Test]
        public void OpenGenericTypes_LinksClosedTypes()
        {
            var factory = new FixtureFactory().AddTypeLink(typeof(IGenericInterface<>), typeof(GenericImplementationClass<>));

            var result = factory.New<GenericInterfaceParameterClass>().Build();

            Assert.That(result.Dependency, Is.TypeOf<GenericImplementationClass<int>>());
        }

        [Test]
        public void CustomTypeLink_LinksTypes()
        {
            var factory = new FixtureFactory().AddTypeLink(new MarkerInterfaceTypeLink());

            var result = factory.New<InterfaceParameterClass>().Build();

            Assert.That(result.Dependency, Is.TypeOf<MarkerImplementationClass>());
        }

        [Test]
        public void LaterTypeLink_WinsOverEarlierTypeLink()
        {
            var factory = new FixtureFactory()
                .AddTypeLink<IMarkerInterface, MarkerImplementationClass>()
                .AddTypeLink<IMarkerInterface, OtherMarkerImplementationClass>();

            var result = factory.New<InterfaceParameterClass>().Build();

            Assert.That(result.Dependency, Is.TypeOf<OtherMarkerImplementationClass>());
        }

        #endregion

        #region Validation

        [Test]
        public void UnassignableTypes_ThrowsArgumentException()
        {
            var factory = new FixtureFactory();

            Assert.Throws<ArgumentException>(() => factory.AddTypeLink<IMarkerInterface, StringConstructorClass>());
        }

        [Test]
        public void UnassignableOpenGenericTypes_ThrowsArgumentException()
        {
            var factory = new FixtureFactory();

            Assert.Throws<ArgumentException>(() => factory.AddTypeLink(typeof(IGenericInterface<>), typeof(List<>)));
        }

        [Test]
        public void OpenGenericToClosedType_ThrowsArgumentException()
        {
            var factory = new FixtureFactory();

            Assert.Throws<ArgumentException>(() => factory.AddTypeLink(typeof(IGenericInterface<>), typeof(GenericImplementationClass<int>)));
        }

        [Test]
        public void ClosedToOpenGenericType_ThrowsArgumentException()
        {
            var factory = new FixtureFactory();

            Assert.Throws<ArgumentException>(() => factory.AddTypeLink(typeof(IGenericInterface<int>), typeof(GenericImplementationClass<>)));
        }

        #endregion
    }
}
