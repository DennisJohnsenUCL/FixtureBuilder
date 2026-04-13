using System.Linq.Expressions;
using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.ExpressionHelperTests
{
    internal sealed class IsPropertyWriteableTests
    {
        private class Parent
        {
            public string Writable { get; set; } = "";
            public int ReadOnly { get; } = 42;
            public Child Child { get; set; } = new();
            public string field = "";

            public string GetValue() => Writable;
        }

        private class Child
        {
            public int Age { get; set; }
            public int ReadOnly { get; } = 7;
        }

        [Test]
        public void WritableProperty_ReturnsTrue()
        {
            Expression<Func<Parent, string>> expr = x => x.Writable;

            Assert.That(ExpressionHelper.IsPropertyWritable(expr), Is.True);
        }

        [Test]
        public void ReadOnlyProperty_ReturnsFalse()
        {
            Expression<Func<Parent, int>> expr = x => x.ReadOnly;

            Assert.That(ExpressionHelper.IsPropertyWritable(expr), Is.False);
        }

        [Test]
        public void NestedWritableProperty_ReturnsTrue()
        {
            Expression<Func<Parent, int>> expr = x => x.Child.Age;

            Assert.That(ExpressionHelper.IsPropertyWritable(expr), Is.True);
        }

        [Test]
        public void NestedReadOnlyProperty_ReturnsFalse()
        {
            Expression<Func<Parent, int>> expr = x => x.Child.ReadOnly;

            Assert.That(ExpressionHelper.IsPropertyWritable(expr), Is.False);
        }

        [Test]
        public void MethodCallExpression_ReturnsFalse()
        {
            Expression<Func<Parent, string>> expr = x => x.GetValue();

            Assert.That(ExpressionHelper.IsPropertyWritable(expr), Is.False);
        }

        [Test]
        public void ConstantExpression_ReturnsFalse()
        {
            Expression<Func<Parent, string>> expr = x => "hello";

            Assert.That(ExpressionHelper.IsPropertyWritable(expr), Is.False);
        }

        [Test]
        public void FieldExpression_ReturnsFalse()
        {
            Expression<Func<Parent, string>> expr = x => x.field;

            Assert.That(ExpressionHelper.IsPropertyWritable(expr), Is.False);
        }
    }
}
