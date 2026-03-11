using System.Linq.Expressions;
using FixtureBuilder.Helpers;

namespace FixtureBuilder.Tests.Helpers.ExpressionHelperTests
{
    internal sealed class ValidateExpressionTests
    {
        public class Parent
        {
            public Child Child { get; set; } = new();
            public string Name { get; set; } = "";
            public int Value { get; set; }
            public string field = "";

            public string GetName() => Name;
        }

        public class Child
        {
            public Grandchild Grandchild { get; set; } = new();
            public int Age { get; set; }
        }

        public class Grandchild
        {
            public string Tag { get; set; } = "";
        }

        [Test]
        public void SinglePropertyAccess_DoesNotThrow()
        {
            Expression<Func<Parent, string>> expr = x => x.Name;

            Assert.DoesNotThrow(() => ExpressionHelper.ValidateExpression(expr));
        }

        [Test]
        public void TwoLevelPropertyChain_DoesNotThrow()
        {
            Expression<Func<Parent, int>> expr = x => x.Child.Age;

            Assert.DoesNotThrow(() => ExpressionHelper.ValidateExpression(expr));
        }

        [Test]
        public void ThreeLevelPropertyChain_DoesNotThrow()
        {
            Expression<Func<Parent, string>> expr = x => x.Child.Grandchild.Tag;

            Assert.DoesNotThrow(() => ExpressionHelper.ValidateExpression(expr));
        }

        [Test]
        public void BareParameter_ThrowsInvalidOperationException()
        {
            Expression<Func<Parent, Parent>> expr = x => x;

            Assert.Throws<InvalidOperationException>(() => ExpressionHelper.ValidateExpression(expr));
        }

        [Test]
        public void MethodCall_ThrowsInvalidOperationException()
        {
            Expression<Func<Parent, string>> expr = x => x.GetName();

            Assert.Throws<InvalidOperationException>(() => ExpressionHelper.ValidateExpression(expr));
        }

        [Test]
        public void FieldAccess_ThrowsInvalidOperationException()
        {
            Expression<Func<Parent, string>> expr = x => x.field;

            Assert.Throws<InvalidOperationException>(() => ExpressionHelper.ValidateExpression(expr));
        }

        [Test]
        public void Constant_ThrowsInvalidOperationException()
        {
            Expression<Func<Parent, string>> expr = x => "hello";

            Assert.Throws<InvalidOperationException>(() => ExpressionHelper.ValidateExpression(expr));
        }

        [Test]
        public void ComputedValue_ThrowsInvalidOperationException()
        {
            Expression<Func<Parent, int>> expr = x => x.Value + 1;

            Assert.Throws<InvalidOperationException>(() => ExpressionHelper.ValidateExpression(expr));
        }

        [Test]
        public void MethodCallInChain_ThrowsInvalidOperationException()
        {
            Expression<Func<Parent, int>> expr = x => x.Name.GetHashCode();

            Assert.Throws<InvalidOperationException>(() => ExpressionHelper.ValidateExpression(expr));
        }

        [Test]
        public void StringLength_OnNestedProperty_DoesNotThrow()
        {
            // x.Child.Grandchild.Tag.Length — Length is a property, so this is a valid chain
            Expression<Func<Parent, int>> expr = x => x.Child.Grandchild.Tag.Length;

            Assert.DoesNotThrow(() => ExpressionHelper.ValidateExpression(expr));
        }

        [Test]
        public void CapturedVariable_ThrowsInvalidOperationException()
        {
            var captured = "value";
            Expression<Func<Parent, string>> expr = x => captured;

            Assert.Throws<InvalidOperationException>(() => ExpressionHelper.ValidateExpression(expr));
        }
    }
}
