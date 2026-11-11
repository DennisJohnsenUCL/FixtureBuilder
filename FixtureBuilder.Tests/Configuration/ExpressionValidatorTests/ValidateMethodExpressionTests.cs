using System.Linq.Expressions;
using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.ExpressionValidatorTests
{
    internal sealed class ValidateMethodExpressionTests
    {
        #region Accepted

        [Test]
        public void MethodOnProperty_DoesNotThrow()
        {
            Expression<Action<PropertyHolder<string>>> expression = x => x.Value.GetHashCode();

            Assert.DoesNotThrow(() => ExpressionValidator.ValidateMethodExpression(expression));
        }

        [Test]
        public void MethodOnField_DoesNotThrow()
        {
            Expression<Action<FieldHolder<string>>> expression = x => x.Value.GetHashCode();

            Assert.DoesNotThrow(() => ExpressionValidator.ValidateMethodExpression(expression));
        }

        [Test]
        public void MethodOnParameter_DoesNotThrow()
        {
            Expression<Action<PropertyHolder<string>>> expression = x => x.GetHashCode();

            Assert.DoesNotThrow(() => ExpressionValidator.ValidateMethodExpression(expression));
        }

        [Test]
        public void MethodOnNullableValue_DoesNotThrow()
        {
            Expression<Action<PropertyHolder<PointStruct?>>> expression = x => x.Value!.Value.GetHashCode();

            Assert.DoesNotThrow(() => ExpressionValidator.ValidateMethodExpression(expression));
        }

        #endregion

        #region Rejected

        [Test]
        public void StaticMethod_ThrowsInvalidOperationException()
        {
            Expression<Action<PropertyHolder<string>>> expression = _ => Console.WriteLine();

            Assert.Throws<InvalidOperationException>(() => ExpressionValidator.ValidateMethodExpression(expression));
        }

        [Test]
        public void MethodOnCapturedVariable_ThrowsInvalidOperationException()
        {
            var holder = new PropertyHolder<string>();
            Expression<Action<PropertyHolder<string>>> expression = _ => holder.GetHashCode();

            Assert.Throws<InvalidOperationException>(() => ExpressionValidator.ValidateMethodExpression(expression));
        }

        #endregion
    }
}
