namespace FixtureBuilder.Tests.FixtureTests.WithField
{
    internal class WithFieldExpressionTests
    {
        private const string _text = "Test string";

        class BaseClass
        {
            public NestedClass NestedClass { get; set; } = null!;
        }

        class NestedClass
        {
            public TwiceNestedClass TwiceNestedClass { get; set; } = null!;
        }

        class TwiceNestedClass : InheritedClass
        {
            private readonly string _field = null!;
            public string GetField => _field;
        }

        class InheritedClass
        {
            protected readonly string _inheritedField = null!;
            public string GetInheritedField => _inheritedField;
        }

        [Test]
        public void NestedProperty_SetsField()
        {
            var fieldName = "_field";

            var fixture = Fixture.New<BaseClass>().CreateUninitialized().WithField(c => c.NestedClass.TwiceNestedClass, fieldName, _text);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.NestedClass.TwiceNestedClass.GetField, Is.EqualTo(_text));
        }

        [Test]
        public void InheritedField_SetsField()
        {
            var fieldName = "_inheritedField";

            var fixture = Fixture.New<BaseClass>().CreateUninitialized().WithField(c => c.NestedClass.TwiceNestedClass, fieldName, _text);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.NestedClass.TwiceNestedClass.GetInheritedField, Is.EqualTo(_text));
        }

        [Test]
        public void ComputedProperty_ThrowsException()
        {
            var fieldName = "_field";

            Assert.Throws<InvalidOperationException>(
                () => Fixture.New<BaseClass>().CreateUninitialized().WithField(c => c.NestedClass.TwiceNestedClass.GetField.Length, fieldName, _text));
        }

        [Test]
        public void MethodAccessor_ThrowsException()
        {
            var fieldName = "_field";

            Assert.Throws<InvalidOperationException>(
                () => Fixture.New<BaseClass>().CreateUninitialized().WithField(c => c.NestedClass.TwiceNestedClass.GetHashCode(), fieldName, _text));
        }
    }
}
