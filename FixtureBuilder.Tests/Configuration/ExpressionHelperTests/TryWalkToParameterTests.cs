using System.Linq.Expressions;
using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.ExpressionHelperTests
{
    internal sealed class TryWalkToParameterTests
    {
        private class Outer
        {
            public Inner Prop { get; set; } = new();
            public Inner Field = new();
            public string Name { get; set; } = "";
            public string NameField = "";
        }

        private class Inner
        {
            public string Value { get; set; } = "";
        }

        [Test]
        public void BareParameter_ReturnsTrue()
        {
            Expression<Func<Outer, Outer>> expr = x => x;
            var result = ExpressionHelper.TryWalkToParameter(expr.Body, expr.Parameters[0]);
            Assert.That(result, Is.True);
        }

        [Test]
        public void SingleProperty_ReturnsTrue()
        {
            Expression<Func<Outer, string>> expr = x => x.Name;
            var result = ExpressionHelper.TryWalkToParameter(expr.Body, expr.Parameters[0]);
            Assert.That(result, Is.True);
        }

        [Test]
        public void SingleField_ReturnsTrue()
        {
            Expression<Func<Outer, string>> expr = x => x.NameField;
            var result = ExpressionHelper.TryWalkToParameter(expr.Body, expr.Parameters[0]);
            Assert.That(result, Is.True);
        }

        [Test]
        public void NestedPropertyChain_ReturnsTrue()
        {
            Expression<Func<Outer, string>> expr = x => x.Prop.Value;
            var result = ExpressionHelper.TryWalkToParameter(expr.Body, expr.Parameters[0]);
            Assert.That(result, Is.True);
        }

        [Test]
        public void MixedFieldAndPropertyChain_ReturnsTrue()
        {
            Expression<Func<Outer, string>> expr = x => x.Field.Value;
            var result = ExpressionHelper.TryWalkToParameter(expr.Body, expr.Parameters[0]);
            Assert.That(result, Is.True);
        }

        [Test]
        public void CapturedVariable_ReturnsFalse()
        {
            var outer = new Outer();
            Expression<Func<Outer, string>> expr = x => outer.Name;
            var result = ExpressionHelper.TryWalkToParameter(expr.Body, expr.Parameters[0]);
            Assert.That(result, Is.False);
        }

        [Test]
        public void WrongParameter_ReturnsFalse()
        {
            Expression<Func<Outer, string>> expr1 = x => x.Name;
            Expression<Func<Outer, string>> expr2 = y => y.Name;
            var result = ExpressionHelper.TryWalkToParameter(expr1.Body, expr2.Parameters[0]);
            Assert.That(result, Is.False);
        }
    }
}
