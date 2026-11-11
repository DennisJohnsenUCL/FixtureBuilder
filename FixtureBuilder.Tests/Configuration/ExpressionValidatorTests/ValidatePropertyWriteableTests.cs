using System.Linq.Expressions;
using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.ExpressionValidatorTests
{
    internal sealed class ValidatePropertyWriteableTests
    {
        [Test]
        public void PropertyWithSetter_DoesNotThrow()
        {
            Expression<Func<PropertyHolder<string>, string>> expression = x => x.Value;

            Assert.DoesNotThrow(() => ExpressionValidator.ValidatePropertyWriteable(expression));
        }

        [Test]
        public void PropertyWithSetterAtEndOfChain_DoesNotThrow()
        {
            Expression<Func<PropertyHolder<PropertyHolder<string>>, string>> expression = x => x.Value.Value;

            Assert.DoesNotThrow(() => ExpressionValidator.ValidatePropertyWriteable(expression));
        }

        [Test]
        public void PropertyWithoutSetter_ThrowsInvalidOperationException()
        {
            Expression<Func<ReadOnlyPropertyHolder<string>, string>> expression = x => x.Value;

            Assert.Throws<InvalidOperationException>(() => ExpressionValidator.ValidatePropertyWriteable(expression));
        }

        [Test]
        public void PropertyWithoutSetterAtEndOfChain_ThrowsInvalidOperationException()
        {
            Expression<Func<PropertyHolder<ReadOnlyPropertyHolder<string>>, string>> expression = x => x.Value.Value;

            Assert.Throws<InvalidOperationException>(() => ExpressionValidator.ValidatePropertyWriteable(expression));
        }
    }
}
