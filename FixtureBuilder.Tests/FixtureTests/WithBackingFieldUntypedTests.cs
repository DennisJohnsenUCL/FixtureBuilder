#pragma warning disable IDE0044

namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class WithBackingFieldUntypedTests
    {
        private readonly static string _text = "Test string";
        private readonly static int _number = 123;

        class SameTypeClass
        {
            public string Text { get; set; } = null!;
        }
        [Test]
        public void SameTypeField_SetsProperty()
        {
            var fixture = Fixture.New<SameTypeClass>().CreateUninitialized().WithBackingFieldUntyped(t => t.Text, _text);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
        }

        class DifferentTypeAssignableClass
        {
            private long _number;
            public int Number { get; set; }
        }
        [Test]
        public void DifferentTypeField_Assignable_SetsProperty()
        {
            var fixture = Fixture.New<DifferentTypeAssignableClass>().CreateUninitialized().WithBackingFieldUntyped(t => t.Number, _number);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.Number, Is.EqualTo(_number));
        }

        class DifferentTypeNotAssignableClass
        {
            private int _someProperty;
            public string SomeProperty { get => _someProperty.ToString(); set { _someProperty = int.Parse(value); } }
        }
        [Test]
        public void DifferentTypeField_NotAssignable_SetsProperty()
        {
            Assert.Warn(typeof(int).IsAssignableFrom(typeof(string)).ToString());

            Assert.Throws<InvalidOperationException>(
                () => Fixture.New<DifferentTypeNotAssignableClass>().CreateUninitialized().WithBackingFieldUntyped(t => t.SomeProperty, _text));
        }

        [Test]
        public void FieldNameGiven_Assignable_SetsProperty()
        {
            var fixture = Fixture.New<DifferentTypeNotAssignableClass>().CreateUninitialized().WithBackingFieldUntyped(t => t.SomeProperty, _number, "_someProperty");
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.SomeProperty, Is.EqualTo(_number.ToString()));
        }
    }
}
