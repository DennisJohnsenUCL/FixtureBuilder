namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests.WhenBuilding
{
    internal sealed class WhenBuildingCustomizationsTests
    {
        #region AddProvider

        [Test]
        public void AddProvider_RootType_SuppliesValues()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<StringConstructorClass>(builder => builder.AddProvider(new FixedStringProvider("provided")));

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        [Test]
        public void AddProvider_OtherRootType_DoesNotApply()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<StringConstructorClass>(builder => builder.AddProvider(new FixedStringProvider("provided")));

            var result = factory.New<OtherStringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void AddProvider_ScopedProvider_WinsOverLaterFactoryWideProvider()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<StringConstructorClass>(builder => builder.AddProvider(new FixedStringProvider("scoped")))
                .AddProvider(new FixedStringProvider("factory-wide"));

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void AddProvider_LaterScopedProvider_WinsOverEarlierScopedProvider()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<StringConstructorClass>(builder => builder.AddProvider(new FixedStringProvider("earlier")))
                .WhenBuilding<StringConstructorClass>(builder => builder.AddProvider(new FixedStringProvider("later")));

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("later"));
        }

        #endregion

        #region AddConverter

        [Test]
        public void AddConverter_RootType_ConvertsValueForBackingField()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<WiderBackingFieldClass>(builder => builder.AddConverter(new FixedLongConverter(456)));
            var fixture = TestHelper.MakeFixture<WiderBackingFieldClass>(factory);

            var result = fixture.WithBackingField(x => x.Value, 123).Build();

            Assert.That(result.Value, Is.EqualTo(456));
        }

        [Test]
        public void AddConverter_OtherRootType_DoesNotApply()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<StringConstructorClass>(builder => builder.AddConverter(new FixedLongConverter(456)));
            var fixture = TestHelper.MakeFixture<WiderBackingFieldClass>(factory);

            var result = fixture.WithBackingField(x => x.Value, 123).Build();

            Assert.That(result.Value, Is.EqualTo(123));
        }

        [Test]
        public void AddConverter_ScopedConverter_WinsOverLaterFactoryWideConverter()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<WiderBackingFieldClass>(builder => builder.AddConverter(new FixedLongConverter(1)))
                .AddConverter(new FixedLongConverter(2));
            var fixture = TestHelper.MakeFixture<WiderBackingFieldClass>(factory);

            var result = fixture.WithBackingField(x => x.Value, 123).Build();

            Assert.That(result.Value, Is.EqualTo(1));
        }

        [Test]
        public void AddConverter_LaterScopedConverter_WinsOverEarlierScopedConverter()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<WiderBackingFieldClass>(builder => builder.AddConverter(new FixedLongConverter(1)))
                .WhenBuilding<WiderBackingFieldClass>(builder => builder.AddConverter(new FixedLongConverter(2)));
            var fixture = TestHelper.MakeFixture<WiderBackingFieldClass>(factory);

            var result = fixture.WithBackingField(x => x.Value, 123).Build();

            Assert.That(result.Value, Is.EqualTo(2));
        }

        #endregion

        #region AddTypeLink

        [Test]
        public void AddTypeLink_TypeParameters_RootType_LinksTypes()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<InterfaceParameterClass>(builder => builder.AddTypeLink<IMarkerInterface, MarkerImplementationClass>());

            var result = factory.New<InterfaceParameterClass>().Build();

            Assert.That(result.Dependency, Is.TypeOf<MarkerImplementationClass>());
        }

        [Test]
        public void AddTypeLink_TypeArguments_RootType_LinksTypes()
        {
#pragma warning disable CA2263 // Prefer generic overload when type is known
            var factory = new FixtureFactory()
                .WhenBuilding<InterfaceParameterClass>(builder => builder.AddTypeLink(typeof(IMarkerInterface), typeof(MarkerImplementationClass)));
#pragma warning restore CA2263 // Prefer generic overload when type is known

            var result = factory.New<InterfaceParameterClass>().Build();

            Assert.That(result.Dependency, Is.TypeOf<MarkerImplementationClass>());
        }

        [Test]
        public void AddTypeLink_CustomTypeLink_RootType_LinksTypes()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<InterfaceParameterClass>(builder => builder.AddTypeLink(new MarkerInterfaceTypeLink()));

            var result = factory.New<InterfaceParameterClass>().Build();

            Assert.That(result.Dependency, Is.TypeOf<MarkerImplementationClass>());
        }

        [Test]
        public void AddTypeLink_OtherRootType_DoesNotApply()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<StringConstructorClass>(builder => builder.AddTypeLink<IMarkerInterface, MarkerImplementationClass>());
            var fixture = factory.New<InterfaceParameterClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.Build());
        }

        [Test]
        public void AddTypeLink_ScopedTypeLink_WinsOverLaterFactoryWideTypeLink()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<InterfaceParameterClass>(builder => builder.AddTypeLink<IMarkerInterface, MarkerImplementationClass>())
                .AddTypeLink<IMarkerInterface, OtherMarkerImplementationClass>();

            var result = factory.New<InterfaceParameterClass>().Build();

            Assert.That(result.Dependency, Is.TypeOf<MarkerImplementationClass>());
        }

        [Test]
        public void AddTypeLink_LaterScopedTypeLink_WinsOverEarlierScopedTypeLink()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<InterfaceParameterClass>(builder => builder.AddTypeLink<IMarkerInterface, MarkerImplementationClass>())
                .WhenBuilding<InterfaceParameterClass>(builder => builder.AddTypeLink<IMarkerInterface, OtherMarkerImplementationClass>());

            var result = factory.New<InterfaceParameterClass>().Build();

            Assert.That(result.Dependency, Is.TypeOf<OtherMarkerImplementationClass>());
        }

        #endregion
    }
}
