using System.Linq.Expressions;
using FixtureBuilder.Configuration;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ExpressionResolverTests
{
    internal sealed class ResolveMethodParentTests
    {
        [Test]
        public void NullRoot_ThrowsArgumentException()
        {
            var resolver = new ExpressionResolver(typeof(PropertyHolder<string>), Mock.Of<IFixtureContext>());
            Expression<Action<PropertyHolder<string>>> expression = x => x.GetHashCode();

            Assert.Throws<ArgumentException>(() => resolver.ResolveMethodParent(null!, expression));
        }
    }
}
