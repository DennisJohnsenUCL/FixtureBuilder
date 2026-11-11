using FixtureBuilder.Core;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories.BogusFixtureFactoryTests
{
    internal sealed class PassthroughConfigurationBuilderTests
    {
        #region Options

        [Test]
        public void Options_Set_FixturesUseOptions()
        {
            var factory = FixtureFactory.WithBogus();

            factory.Options = new FixtureOptions { AllowPrivateConstructors = false };

            Assert.Throws<InvalidOperationException>(() => factory.Build<PrivateConstructorClass>());
        }

        [Test]
        public void SetOptions_ChangesOptions()
        {
            var factory = FixtureFactory.WithBogus();

            factory.SetOptions(options => options.AllowPrivateConstructors = false);

            Assert.That(factory.Options.AllowPrivateConstructors, Is.False);
        }

        [Test]
        public void SetOptions_FixturesUseChangedOptions()
        {
            var factory = FixtureFactory.WithBogus();

            factory.SetOptions(options => options.AllowPrivateConstructors = false);

            Assert.Throws<InvalidOperationException>(() => factory.Build<PrivateConstructorClass>());
        }

        #endregion

        #region Customizations

        [Test]
        public void AddProvider_FixturesUseProvider()
        {
            var factory = FixtureFactory.WithBogus().AddProvider(new FixedStringProvider("provided"));

            var result = factory.Build<StringConstructorClass>();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        [Test]
        public void AddConverter_FixturesUseConverter()
        {
            var factory = FixtureFactory.WithBogus().AddConverter(new FixedLongConverter(456));

            var result = factory.New<WiderBackingFieldClass>().WithBackingField(x => x.Value, 123).Build();

            Assert.That(result.Value, Is.EqualTo(456));
        }

        [Test]
        public void AddTypeLink_TypeParameters_FixturesUseLink()
        {
            var factory = FixtureFactory.WithBogus().AddTypeLink<IMarkerInterface, MarkerImplementationClass>();

            var result = factory.Build<InterfaceParameterClass>();

            Assert.That(result.Dependency, Is.TypeOf<MarkerImplementationClass>());
        }

        [Test]
        public void AddTypeLink_TypeArguments_FixturesUseLink()
        {
#pragma warning disable CA2263 // Prefer generic overload when type is known
            var factory = FixtureFactory.WithBogus().AddTypeLink(typeof(IMarkerInterface), typeof(MarkerImplementationClass));
#pragma warning restore CA2263 // Prefer generic overload when type is known

            var result = factory.Build<InterfaceParameterClass>();

            Assert.That(result.Dependency, Is.TypeOf<MarkerImplementationClass>());
        }

        [Test]
        public void AddTypeLink_CustomTypeLink_FixturesUseLink()
        {
            var factory = FixtureFactory.WithBogus().AddTypeLink(new MarkerInterfaceTypeLink());

            var result = factory.Build<InterfaceParameterClass>();

            Assert.That(result.Dependency, Is.TypeOf<MarkerImplementationClass>());
        }

        #endregion
    }
}
