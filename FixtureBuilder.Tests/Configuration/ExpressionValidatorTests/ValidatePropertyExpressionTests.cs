using System.Linq.Expressions;
using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.ExpressionValidatorTests
{
    internal sealed class ValidatePropertyExpressionTests
    {
        #region Accepted

        [Test]
        public void Property_DoesNotThrow()
        {
            Expression<Func<PropertyHolder<string>, string>> expression = x => x.Value;

            Assert.DoesNotThrow(() => ExpressionValidator.ValidatePropertyExpression(expression));
        }

        [Test]
        public void PropertyChain_DoesNotThrow()
        {
            Expression<Func<PropertyHolder<PropertyHolder<string>>, string>> expression = x => x.Value.Value;

            Assert.DoesNotThrow(() => ExpressionValidator.ValidatePropertyExpression(expression));
        }

        [Test]
        public void FieldThenProperty_DoesNotThrow()
        {
            Expression<Func<FieldHolder<PropertyHolder<string>>, string>> expression = x => x.Value.Value;

            Assert.DoesNotThrow(() => ExpressionValidator.ValidatePropertyExpression(expression));
        }

        #endregion

        #region Rejected

        [Test]
        public void Field_ThrowsInvalidOperationException()
        {
            Expression<Func<FieldHolder<string>, string>> expression = x => x.Value;

            Assert.Throws<InvalidOperationException>(() => ExpressionValidator.ValidatePropertyExpression(expression));
        }

        [Test]
        public void PropertyThenField_ThrowsInvalidOperationException()
        {
            Expression<Func<PropertyHolder<FieldHolder<string>>, string>> expression = x => x.Value.Value;

            Assert.Throws<InvalidOperationException>(() => ExpressionValidator.ValidatePropertyExpression(expression));
        }

        [Test]
        public void Parameter_ThrowsInvalidOperationException()
        {
            Expression<Func<PropertyHolder<string>, PropertyHolder<string>>> expression = x => x;

            Assert.Throws<InvalidOperationException>(() => ExpressionValidator.ValidatePropertyExpression(expression));
        }

        [Test]
        public void Constant_ThrowsInvalidOperationException()
        {
            Expression<Func<PropertyHolder<string>, string>> expression = _ => "constant";

            Assert.Throws<InvalidOperationException>(() => ExpressionValidator.ValidatePropertyExpression(expression));
        }

        [Test]
        public void TrailingNullableValue_ThrowsInvalidOperationException()
        {
            Expression<Func<FieldHolder<PointStruct?>, PointStruct>> expression = x => x.Value!.Value;

            Assert.Throws<InvalidOperationException>(() => ExpressionValidator.ValidatePropertyExpression(expression));
        }

        #endregion
    }
}
