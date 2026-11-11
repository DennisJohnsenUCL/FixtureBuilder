namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class WithSetterTests
    {
        [Test]
        public void PropertyWithSetter_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<string>>();

            var result = fixture.WithSetter(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        private class PrivateSetterClass
        {
            public string Value { get; private set; } = null!;
        }

        [Test]
        public void PrivateSetter_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<PrivateSetterClass>();

            var result = fixture.WithSetter(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void InitOnlyPropertyOfRecord_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<PositionalRecord>();

            var result = fixture.WithSetter(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void InheritedProperty_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<DerivedClass>();

            var result = fixture.WithSetter(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void OverriddenProperty_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<OverridingClass>();

            var result = fixture.WithSetter(x => x.Value, 123).Build();

            Assert.That(result.Value, Is.EqualTo(123));
        }

        [Test]
        public void ImplicitInterfaceImplementation_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<ImplicitImplementationClass>();

            var result = fixture.WithSetter(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void PropertyWithoutSetter_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<string>>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithSetter(x => x.Value, "value"));
        }

        [Test]
        public void Field_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<string>>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithSetter(x => x.Value, "value"));
        }

        [Test]
        public void ListProperty_SetsList()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<List<string>>>();

            var result = fixture.WithSetter(x => x.Value, ["first", "second"]).Build();

            Assert.That(result.Value, Is.EqualTo(["first", "second"]));
        }

        [Test]
        public void PropertyChain_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PropertyHolder<string>>>();

            var result = fixture.WithSetter(x => x.Value.Value, "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void LongerPropertyChain_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PropertyHolder<PropertyHolder<int>>>>();

            var result = fixture.WithSetter(x => x.Value.Value.Value, 123).Build();

            Assert.That(result.Value.Value.Value, Is.EqualTo(123));
        }

        [Test]
        public void InheritedPropertyChain_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<DerivedChainClass>();

            var result = fixture.WithSetter(x => x.Value.Value, "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void FieldInChain_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<PropertyHolder<string>>>();

            var result = fixture.WithSetter(x => x.Value.Value, "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void StructFieldInChain_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<PropertyHolderStruct<int>>>();

            var result = fixture.WithSetter(x => x.Value.Value, 123).Build();

            Assert.That(result.Value.Value, Is.EqualTo(123));
        }

        [Test]
        public void NotConstructed_ConstructsImplicitlyAndSetsProperty()
        {
            var fixture = Fixture.New<PropertyHolder<string>>();

            var result = fixture.WithSetter(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }
    }
}
