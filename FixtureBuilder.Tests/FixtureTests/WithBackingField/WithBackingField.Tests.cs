using System.Reflection;

namespace FixtureBuilder.Tests.FixtureTests.WithBackingField
{
    internal sealed class WithBackingFieldTests
    {
        #region Backing field lookup

        [Test]
        public void AutoProperty_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<string>>();

            var result = fixture.WithBackingField(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void ExplicitBackingField_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<string>>();

            var result = fixture.WithBackingField(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        private class MultiWordBackingFieldClass
        {
            private readonly string _multiWordValue = null!;
            public string MultiWordValue => _multiWordValue;
        }

        [Test]
        public void ExplicitBackingFieldWithMultiWordName_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<MultiWordBackingFieldClass>();

            var result = fixture.WithBackingField(x => x.MultiWordValue, "value").Build();

            Assert.That(result.MultiWordValue, Is.EqualTo("value"));
        }

        [Test]
        public void MethodCall_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<string>>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingField(x => x.GetHashCode(), 123));
        }

        [Test]
        public void PropertyWithoutFindableBackingField_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<string>>();
            fixture.With(x => x.Value, "value");

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingField(x => x.Value.Length, 123));
        }

        [Test]
        public void Field_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<string>>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingField(x => x.Value, "value"));
        }

        [Test]
        public void ExpressionEndingInNullableValue_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PointStruct?>>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingField(x => x.Value!.Value, new PointStruct()));
        }

        [Test]
        public void NotConstructed_ConstructsImplicitlyAndSetsBackingField()
        {
            var fixture = Fixture.New<PropertyHolder<string>>();

            var result = fixture.WithBackingField(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        #endregion

        #region Inheritance and interfaces

        [Test]
        public void InheritedAutoProperty_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<DerivedClass>();

            var result = fixture.WithBackingField(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        private class NotOverridingClass : VirtualPropertyClass;

        [Test]
        public void InheritedVirtualAutoProperty_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<NotOverridingClass>();

            var result = fixture.WithBackingField(x => x.Value, 123).Build();

            Assert.That(result.Value, Is.EqualTo(123));
        }

        [Test]
        public void OverriddenAutoProperty_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<OverridingClass>();

            var result = fixture.WithBackingField(x => x.Value, 123).Build();

            Assert.That(result.Value, Is.EqualTo(123));
        }

        private class GetOnlyVirtualPropertyClass
        {
            public virtual int Value { get; }
        }

        private class UnconventionalFieldOverrideClass : GetOnlyVirtualPropertyClass
        {
            private int _unconventionalName;
            public override int Value => _unconventionalName;
        }

        // _unconventionalName does not follow the naming conventions and no field name is passed, so the base auto-property's
        // field is written instead. The getter cannot be inspected, so this cannot be detected.
        [Test]
        public void OverridingPropertyWithUnconventionalField_WritesBaseAutoPropertyFieldSilently()
        {
            var fixture = TestHelper.MakeFixture<UnconventionalFieldOverrideClass>();

            var result = fixture.WithBackingField(x => x.Value, 123).Build();

            var baseField = typeof(GetOnlyVirtualPropertyClass).GetField("<Value>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!;
            var baseFieldValue = baseField.GetValue(result);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(baseFieldValue, Is.EqualTo(123));
                Assert.That(result.Value, Is.Zero);
            }
        }

        [Test]
        public void ImplicitInterfaceImplementation_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<ImplicitImplementationClass>();

            var result = fixture.WithBackingField(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void CastToInterface_ImplicitImplementation_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<ImplicitImplementationClass>().CastTo<IValueInterface>();

            var result = fixture.WithBackingField(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        #endregion

        #region Member chains

        [Test]
        public void PropertyChain_SetsBackingFieldOfLastMember()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PropertyHolder<string>>>();

            var result = fixture.WithBackingField(x => x.Value.Value, "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void LongerPropertyChain_SetsBackingFieldOfLastMember()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PropertyHolder<PropertyHolder<int>>>>();

            var result = fixture.WithBackingField(x => x.Value.Value.Value, 123).Build();

            Assert.That(result.Value.Value.Value, Is.EqualTo(123));
        }

        [Test]
        public void PropertyChainStartingInInheritedProperty_SetsBackingFieldOfLastMember()
        {
            var fixture = TestHelper.MakeFixture<DerivedChainClass>();

            var result = fixture.WithBackingField(x => x.Value.Value, "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void PropertyChainEndingInInheritedAutoProperty_SetsBackingFieldOfLastMember()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<DerivedClass>>();

            var result = fixture.WithBackingField(x => x.Value.Value, "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void PropertyChainEndingInOverriddenAutoProperty_SetsBackingFieldOfLastMember()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<OverridingClass>>();

            var result = fixture.WithBackingField(x => x.Value.Value, 123).Build();

            Assert.That(result.Value.Value, Is.EqualTo(123));
        }

        [Test]
        public void PropertyChainWithFieldName_SetsNamedFieldOfLastMember()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<UnconventionalBackingFieldClass>>();

            var result = fixture.WithBackingField(x => x.Value.Value, "value", "_unconventionalName").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void FieldInChain_SetsBackingFieldOfLastMember()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<PropertyHolder<string>>>();

            var result = fixture.WithBackingField(x => x.Value.Value, "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void StructPropertyInChain_SetsBackingFieldOfLastMember()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<ReadOnlyPropertyHolderStruct<int>>>();

            var result = fixture.WithBackingField(x => x.Value.Value, 123).Build();

            Assert.That(result.Value.Value, Is.EqualTo(123));
        }

        #endregion
    }
}
