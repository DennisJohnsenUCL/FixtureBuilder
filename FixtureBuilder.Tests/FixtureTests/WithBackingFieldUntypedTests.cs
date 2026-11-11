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
        public void MethodCall_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<string>>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingFieldUntyped(x => x.GetHashCode(), 123));
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
