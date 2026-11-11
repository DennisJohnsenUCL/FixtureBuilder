using System.Linq.Expressions;
using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.ExpressionValidatorTests
{
    internal sealed class ValidateExpressionTests
    {
        #region Accepted

        [Test]
        public void Property_DoesNotThrow()
        {
            Expression<Func<PropertyHolder<string>, string>> expression = x => x.Value;

            Assert.DoesNotThrow(() => ExpressionValidator.ValidateExpression(expression));
        }

        [Test]
        public void Field_DoesNotThrow()
        {
            Expression<Func<FieldHolder<string>, string>> expression = x => x.Value;

            Assert.DoesNotThrow(() => ExpressionValidator.ValidateExpression(expression));
        }

        [Test]
        public void PropertyChain_DoesNotThrow()
        {
            Expression<Func<PropertyHolder<PropertyHolder<string>>, string>> expression = x => x.Value.Value;

            Assert.DoesNotThrow(() => ExpressionValidator.ValidateExpression(expression));
        }

        [Test]
        public void FieldThenProperty_DoesNotThrow()
        {
            Expression<Func<FieldHolder<PropertyHolder<string>>, string>> expression = x => x.Value.Value;

            Assert.DoesNotThrow(() => ExpressionValidator.ValidateExpression(expression));
        }

        [Test]
        public void NullableValueInMiddleOfChain_DoesNotThrow()
        {
            Expression<Func<PropertyHolder<PointStruct?>, int>> expression = x => x.Value!.Value.X;

            Assert.DoesNotThrow(() => ExpressionValidator.ValidateExpression(expression));
        }

        #endregion

        #region Rejected

        [Test]
        public void MethodCall_ThrowsInvalidOperationException()
        {
            Expression<Func<PropertyHolder<string>, int>> expression = x => x.GetHashCode();

            Assert.Throws<InvalidOperationException>(() => ExpressionValidator.ValidateExpression(expression));
        }

        [Test]
        public void Constant_ThrowsInvalidOperationException()
        {
            Expression<Func<PropertyHolder<string>, string>> expression = x => "constant";

            Assert.Throws<InvalidOperationException>(() => ExpressionValidator.ValidateExpression(expression));
        }

        [Test]
        public void Parameter_ThrowsInvalidOperationException()
        {
            Expression<Func<PropertyHolder<string>, PropertyHolder<string>>> expression = x => x;

            Assert.Throws<InvalidOperationException>(() => ExpressionValidator.ValidateExpression(expression));
        }

        [Test]
        public void CapturedVariable_ThrowsInvalidOperationException()
        {
            var holder = new PropertyHolder<string>();
            Expression<Func<PropertyHolder<string>, string>> expression = x => holder.Value;

            Assert.Throws<InvalidOperationException>(() => ExpressionValidator.ValidateExpression(expression));
        }

        [Test]
        public void TrailingNullableValue_ThrowsInvalidOperationExceptionSuggestingNullableMember()
        {
            Expression<Func<PropertyHolder<PointStruct?>, PointStruct>> expression = x => x.Value!.Value;

            var ex = Assert.Throws<InvalidOperationException>(() => ExpressionValidator.ValidateExpression(expression));

            Assert.That(ex.Message, Does.Contain("'x => x.Value'"));
        }

        #endregion
    }
}
