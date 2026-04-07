#pragma warning disable IDE0044
#pragma warning disable CS0169

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
            var fixture = TestHelper.MakeFixture<SameTypeClass>();

            fixture.WithBackingFieldUntyped(t => t.Text, _text);

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
            var fixture = TestHelper.MakeFixture<DifferentTypeAssignableClass>();

            fixture.WithBackingFieldUntyped(t => t.Number, _number);

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
            var fixture = TestHelper.MakeFixture<DifferentTypeNotAssignableClass>();

            Assert.Throws<InvalidOperationException>(
                () => fixture.WithBackingFieldUntyped(t => t.SomeProperty, _text));
        }

        [Test]
        public void FieldNameGiven_Assignable_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<DifferentTypeNotAssignableClass>();

            fixture.WithBackingFieldUntyped(t => t.SomeProperty, _number, "_someProperty");

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.SomeProperty, Is.EqualTo(_number.ToString()));
        }
    }
}
