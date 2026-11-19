using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class WithBackingFieldUntypedTests
    {
        [Test]
        public void ValueOfPropertyType_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<string>>();

            var result = fixture.WithBackingFieldUntyped(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void WiderBackingField_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<WiderBackingFieldClass>();

            var result = fixture.WithBackingFieldUntyped(x => x.Value, 123).Build();

            Assert.That(result.Value, Is.EqualTo(123));
        }

        private class IntFieldStringPropertyClass
        {
            private int _value;
            public string Value { get => _value.ToString(); set => _value = int.Parse(value); }
        }

        [Test]
        public void ValueOfFieldType_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<IntFieldStringPropertyClass>();

            var result = fixture.WithBackingFieldUntyped(x => x.Value, 123, "_value").Build();

            Assert.That(result.Value, Is.EqualTo("123"));
        }

        [Test]
        public void ValueNotAssignableToBackingField_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<IntFieldStringPropertyClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingFieldUntyped(x => x.Value, "value"));
        }

        [Test]
        public void ImplicitConversionAllowed_SetsConvertedValue()
        {
            var options = new FixtureOptions { AllowImplicitConversion = true };
            var factory = new FixtureFactory(options);
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<ImplicitTargetStruct>>(factory);

            var result = fixture.WithBackingFieldUntyped(x => x.Value, new ImplicitSourceStruct { Number = 42 }).Build();

            Assert.That(result.Value.Number, Is.EqualTo(42));
        }

        [Test]
        public void MethodCall_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<string>>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingFieldUntyped(x => x.GetHashCode(), 123));
        }

        [Test]
        public void NullExpression_ThrowsArgumentNullException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<string>>();

            var ex = Assert.Throws<ArgumentNullException>(() => fixture.WithBackingFieldUntyped<string>(null!, "value"));

            Assert.That(ex.ParamName, Is.EqualTo("expr"));
        }

        [Test]
        public void NullExpressionWithFieldName_ThrowsArgumentNullException()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<string>>();

            var ex = Assert.Throws<ArgumentNullException>(() => fixture.WithBackingFieldUntyped<string>(null!, "value", "_value"));

            Assert.That(ex.ParamName, Is.EqualTo("expr"));
        }

        [Test]
        public void NullFieldName_ThrowsArgumentNullException()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<string>>();

            var ex = Assert.Throws<ArgumentNullException>(() => fixture.WithBackingFieldUntyped(x => x.Value, "value", null!));

            Assert.That(ex.ParamName, Is.EqualTo("fieldName"));
        }

        [Test]
        public void NotConstructed_ConstructsImplicitlyAndSetsBackingField()
        {
            var fixture = Fixture.New<PropertyHolder<string>>();

            var result = fixture.WithBackingFieldUntyped(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }
    }
}
