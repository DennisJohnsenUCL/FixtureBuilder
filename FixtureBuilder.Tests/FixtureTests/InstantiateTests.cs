using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class InstantiateTests
    {
        #region Default instantiation

        [Test]
        public void Property_SetsAutoConstructedInstance()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value).Build();

            Assert.That(result.Value.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void PropertyWithoutSetter_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value).Build();

            Assert.That(result.Value.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void Field_SetsField()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value).Build();

            Assert.That(result.Value.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void FieldChain_SetsLastField()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<FieldHolder<ConstructorSetsValueClass>>>();

            var result = fixture.Instantiate(x => x.Value.Value).Build();

            Assert.That(result.Value.Value.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void PropertyOfStructInProperty_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PropertyHolderStruct<ConstructorSetsValueClass>>>();

            var result = fixture.Instantiate(x => x.Value.Value).Build();

            Assert.That(result.Value.Value.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void RootTypeLink_InstantiatesLinkedType()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<PropertyHolder<IMarkerInterface>>(builder => builder.AddTypeLink<IMarkerInterface, MarkerImplementationClass>());
            var fixture = TestHelper.MakeFixture<PropertyHolder<IMarkerInterface>>(factory);

            var result = fixture.Instantiate(x => x.Value).Build();

            Assert.That(result.Value, Is.TypeOf<MarkerImplementationClass>());
        }

        [Test]
        public void ProvidedValueForMemberType_SetsProvidedValue()
        {
            var provided = new ConstructorSetsValueClass();
            var factory = new FixtureFactory().With(provided);
            var fixture = TestHelper.MakeFixture<PropertyHolder<ConstructorSetsValueClass>>(factory);

            var result = fixture.Instantiate(x => x.Value).Build();

            Assert.That(result.Value, Is.SameAs(provided));
        }

        [Test]
        public void RootScopedParameterValue_ConstructsInstanceWithIt()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<PropertyHolder<StringConstructorClass>>(builder => builder.WithParameter("scoped"));
            var fixture = TestHelper.MakeFixture<PropertyHolder<StringConstructorClass>>(factory);

            var result = fixture.Instantiate(x => x.Value).Build();

            Assert.That(result.Value.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void CreateUninitializedOption_SetsUninitializedInstance()
        {
            var options = new FixtureOptions { DefaultInstantiateInstantiationMethod = InstantiationMethod.CreateUninitialized };
            var factory = new FixtureFactory(options);
            var fixture = TestHelper.MakeFixture<PropertyHolder<ConstructorSetsValueClass>>(factory);

            var result = fixture.Instantiate(x => x.Value).Build();

            Assert.That(result.Value.Value, Is.Null);
        }

        [Test]
        public void MethodCall_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<ConstructorSetsValueClass>>();

            Assert.Throws<InvalidOperationException>(() => fixture.Instantiate(x => x.Value.GetHashCode()));
        }

        [Test]
        public void ReturnsSameFixture()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value);

            Assert.That(result, Is.SameAs(fixture));
        }

        [Test]
        public void NotConstructed_ConstructsImplicitlyAndSetsProperty()
        {
            var fixture = Fixture.New<PropertyHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value).Build();

            Assert.That(result.Value.Value, Is.EqualTo("constructor"));
        }

        #endregion

        #region With constructor function

        [Test]
        public void UseAutoConstructor_SetsAutoConstructedInstance()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.UseAutoConstructor()).Build();

            Assert.That(result.Value.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void UseConstructor_SetsInstanceConstructedWithArguments()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<StringConstructorClass>>();

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.UseConstructor("value")).Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void CreateUninitialized_SetsUninitializedInstance()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.CreateUninitialized()).Build();

            Assert.That(result.Value.Value, Is.Null);
        }

        [Test]
        public void CreateUninitializedWithAllMembers_SetsInstanceWithInitializedMembers()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.CreateUninitialized(InitializeMembers.All)).Build();

            Assert.That(result.Value.Value, Is.EqualTo("Value"));
        }

        [Test]
        public void CreateUninitialized_TypeThatCannotBeCreatedUninitialized_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<string>>();

            Assert.Throws<InvalidOperationException>(() => fixture.Instantiate(x => x.Value, constructor => constructor.CreateUninitialized()));
        }

        [Test]
        public void UseCustomInstantiator_SetsInstantiatorInstance()
        {
            var instance = new ConstructorSetsValueClass();
            var fixture = TestHelper.MakeFixture<PropertyHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.UseCustomInstantiator(() => instance)).Build();

            Assert.That(result.Value, Is.SameAs(instance));
        }

        [Test]
        public void UseCustomInstantiator_ReturnsNull_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<ConstructorSetsValueClass>>();

            Assert.Throws<InvalidOperationException>(() => fixture.Instantiate(x => x.Value, constructor => constructor.UseCustomInstantiator(() => null!)));
        }

        [Test]
        public void UseAutoConstructor_PropertyWithoutSetter_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.UseAutoConstructor()).Build();

            Assert.That(result.Value.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void UseAutoConstructor_Field_SetsField()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.UseAutoConstructor()).Build();

            Assert.That(result.Value.Value, Is.EqualTo("constructor"));
        }

        private struct StringConstructorStruct(string value)
        {
            public string Value { get; } = value;
        }

        [Test]
        public void UseConstructor_NullableStructProperty_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<StringConstructorStruct?>>();

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.UseConstructor("value")).Build();

            Assert.That(result.Value?.Value, Is.EqualTo("value"));
        }

        [Test]
        public void UseAutoConstructor_TypeLink_InstantiatesLinkedType()
        {
            var factory = new FixtureFactory().AddTypeLink<IMarkerInterface, MarkerImplementationClass>();
            var fixture = TestHelper.MakeFixture<PropertyHolder<IMarkerInterface>>(factory);

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.UseAutoConstructor()).Build();

            Assert.That(result.Value, Is.TypeOf<MarkerImplementationClass>());
        }

        [Test]
        public void UseAutoConstructor_RootTypeLink_InstantiatesLinkedType()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<PropertyHolder<IMarkerInterface>>(builder => builder.AddTypeLink<IMarkerInterface, MarkerImplementationClass>());
            var fixture = TestHelper.MakeFixture<PropertyHolder<IMarkerInterface>>(factory);

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.UseAutoConstructor()).Build();

            Assert.That(result.Value, Is.TypeOf<MarkerImplementationClass>());
        }

        [Test]
        public void CreateUninitialized_TypeLink_InstantiatesLinkedType()
        {
            var factory = new FixtureFactory().AddTypeLink<IMarkerInterface, MarkerImplementationClass>();
            var fixture = TestHelper.MakeFixture<PropertyHolder<IMarkerInterface>>(factory);

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.CreateUninitialized()).Build();

            Assert.That(result.Value, Is.TypeOf<MarkerImplementationClass>());
        }

        [Test]
        public void UseConstructor_TypeLink_IgnoresLinkAndThrowsInvalidOperationException()
        {
            var factory = new FixtureFactory().AddTypeLink<IMarkerInterface, MarkerImplementationClass>();
            var fixture = TestHelper.MakeFixture<PropertyHolder<IMarkerInterface>>(factory);

            Assert.Throws<InvalidOperationException>(() => fixture.Instantiate(x => x.Value, constructor => constructor.UseConstructor()));
        }

        [Test]
        public void UseAutoConstructor_MethodCall_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<ConstructorSetsValueClass>>();

            Assert.Throws<InvalidOperationException>(() => fixture.Instantiate(x => x.Value.GetHashCode(), constructor => constructor.UseAutoConstructor()));
        }

        [Test]
        public void UseAutoConstructor_ReturnsSameFixture()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<ConstructorSetsValueClass>>();

            var result = fixture.Instantiate(x => x.Value, constructor => constructor.UseAutoConstructor());

            Assert.That(result, Is.SameAs(fixture));
        }

        #endregion
    }
}
