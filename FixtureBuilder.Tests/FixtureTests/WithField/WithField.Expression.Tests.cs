namespace FixtureBuilder.Tests.FixtureTests.WithField
{
    internal sealed class WithFieldExpressionTests
    {
        [Test]
        public void Property_SetsFieldOnPropertyValue()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<ReadOnlyPropertyHolder<string>>>();

            var result = fixture.WithField(x => x.Value, "_value", "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void PropertyChain_InitializesChainAndSetsFieldOnLastMember()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PropertyHolder<ReadOnlyPropertyHolder<string>>>>();

            var result = fixture.WithField(x => x.Value.Value, "_value", "value").Build();

            Assert.That(result.Value.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void Field_SetsFieldOnFieldValue()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<ReadOnlyPropertyHolder<string>>>();

            var result = fixture.WithField(x => x.Value, "_value", "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void StructField_SetsFieldOnStructAndWritesItBack()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<ReadOnlyPropertyHolderStruct<int>>>();

            var result = fixture.WithField(x => x.Value, "_value", 5).Build();

            Assert.That(result.Value.Value, Is.EqualTo(5));
        }

        [Test]
        public void InheritedProtectedField_SetsField()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<DerivedFromProtectedFieldClass>>();

            var result = fixture.WithField(x => x.Value, "_value", "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void PrivateFieldInheritedTwoLevelsUp_SetsField()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<GrandchildOfPrivateFieldClass>>();

            var result = fixture.WithField(x => x.Value, "_value", "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void MemberHoldingDerivedInstance_SetsFieldDeclaredOnRuntimeType()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<object>>();
            fixture.With(x => x.Value, new ReadOnlyPropertyHolder<string>());

            var result = fixture.WithField(x => x.Value, "_value", "value").Build();

            var value = (ReadOnlyPropertyHolder<string>)result.Value;
            Assert.That(value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void ComputedProperty_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<string>>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithField(x => x.Value.Length, "_value", "value"));
        }

        [Test]
        public void MethodCall_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<ReadOnlyPropertyHolder<string>>>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithField(x => x.Value.GetHashCode(), "_value", "value"));
        }

        [Test]
        public void NotConstructed_ConstructsImplicitlyAndSetsField()
        {
            var fixture = Fixture.New<PropertyHolder<ReadOnlyPropertyHolder<string>>>();

            var result = fixture.WithField(x => x.Value, "_value", "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }
    }
}
