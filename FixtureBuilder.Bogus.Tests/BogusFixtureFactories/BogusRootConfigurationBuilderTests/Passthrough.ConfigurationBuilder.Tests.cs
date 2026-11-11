using FixtureBuilder.Core;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories.BogusRootConfigurationBuilderTests
{
    internal sealed class PassthroughConfigurationBuilderTests
    {
        #region Options

        [Test]
        public void Options_Set_RootTypeUsesOptions()
        {
            var factory = FixtureFactory.WithBogus()
                .WhenBuilding<PrivateConstructorClass>(builder => builder.Options = new FixtureOptions { AllowPrivateConstructors = false });

            Assert.Throws<InvalidOperationException>(() => factory.Build<PrivateConstructorClass>());
        }

        [Test]
        public void SetOptions_RootTypeUsesChangedOptions()
        {
            var factory = FixtureFactory.WithBogus()
                .WhenBuilding<PrivateConstructorClass>(builder => builder.SetOptions(options => options.AllowPrivateConstructors = false));

            Assert.Throws<InvalidOperationException>(() => factory.Build<PrivateConstructorClass>());
        }

        #endregion

        #region Customizations

        [Test]
        public void AddProvider_RootTypeUsesProvider()
        {
            var factory = FixtureFactory.WithBogus()
                .WhenBuilding<StringConstructorClass>(builder => builder.AddProvider(new FixedStringProvider("scoped")));

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        private class OtherStringConstructorClass(string value)
        {
            public string Value { get; } = value;
        }

        [Test]
        public void AddProvider_OtherRootType_DoesNotUseProvider()
        {
            var factory = FixtureFactory.WithBogus()
                .WhenBuilding<StringConstructorClass>(builder => builder.AddProvider(new FixedStringProvider("scoped")));

            var result = factory.Build<OtherStringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void AddConverter_RootTypeUsesConverter()
        {
            var factory = FixtureFactory.WithBogus()
                .WhenBuilding<WiderBackingFieldClass>(builder => builder.AddConverter(new FixedLongConverter(456)));

            var result = factory.New<WiderBackingFieldClass>().WithBackingField(x => x.Value, 123).Build();

            Assert.That(result.Value, Is.EqualTo(456));
        }

        [Test]
        public void AddTypeLink_TypeParameters_RootTypeUsesLink()
        {
            var factory = FixtureFactory.WithBogus()
                .WhenBuilding<InterfaceParameterClass>(builder => builder.AddTypeLink<IMarkerInterface, MarkerImplementationClass>());

            var result = factory.Build<InterfaceParameterClass>();

            Assert.That(result.Dependency, Is.TypeOf<MarkerImplementationClass>());
        }

        [Test]
        public void AddTypeLink_TypeArguments_RootTypeUsesLink()
        {
#pragma warning disable CA2263 // Prefer generic overload when type is known
            var factory = FixtureFactory.WithBogus()
                .WhenBuilding<InterfaceParameterClass>(builder => builder.AddTypeLink(typeof(IMarkerInterface), typeof(MarkerImplementationClass)));
#pragma warning restore CA2263 // Prefer generic overload when type is known

            var result = factory.Build<InterfaceParameterClass>();

            Assert.That(result.Dependency, Is.TypeOf<MarkerImplementationClass>());
        }

        [Test]
        public void AddTypeLink_CustomTypeLink_RootTypeUsesLink()
        {
            var factory = FixtureFactory.WithBogus()
                .WhenBuilding<InterfaceParameterClass>(builder => builder.AddTypeLink(new MarkerInterfaceTypeLink()));

            var result = factory.Build<InterfaceParameterClass>();

            Assert.That(result.Dependency, Is.TypeOf<MarkerImplementationClass>());
        }

        #endregion
    }
}
