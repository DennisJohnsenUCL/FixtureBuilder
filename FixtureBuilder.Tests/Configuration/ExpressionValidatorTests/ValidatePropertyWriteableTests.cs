using System.Linq.Expressions;
using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.ExpressionValidatorTests
{
    internal sealed class ValidatePropertyWriteableTests
    {
        private class Parent
        {
            public string Writable { get; set; } = "";
            public int ReadOnly { get; } = 42;
            public Child Child { get; set; } = new();
        }

        private class Child
        {
            public int Age { get; set; }
            public int ReadOnly { get; } = 7;
        }

        [Test]
        public void WritableProperty_DoesNotThrow()
        {
            Expression<Func<Parent, string>> expr = x => x.Writable;

            Assert.DoesNotThrow(() => ExpressionValidator.ValidatePropertyWriteable(expr));
        }

        [Test]
        public void NestedWritableProperty_DoesNotThrow()
        {
            Expression<Func<Parent, int>> expr = x => x.Child.Age;

            Assert.DoesNotThrow(() => ExpressionValidator.ValidatePropertyWriteable(expr));
        }

        [Test]
        public void ReadOnlyProperty_ThrowsInvalidOperationException()
        {
            Expression<Func<Parent, int>> expr = x => x.ReadOnly;

            Assert.Throws<InvalidOperationException>(
                () => ExpressionValidator.ValidatePropertyWriteable(expr));
        }

        [Test]
        public void NestedReadOnlyProperty_ThrowsInvalidOperationException()
        {
            Expression<Func<Parent, int>> expr = x => x.Child.ReadOnly;

            Assert.Throws<InvalidOperationException>(
                () => ExpressionValidator.ValidatePropertyWriteable(expr));
        }
    }
}
