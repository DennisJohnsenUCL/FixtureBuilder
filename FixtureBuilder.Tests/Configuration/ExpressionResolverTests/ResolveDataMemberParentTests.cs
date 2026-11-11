using System.Linq.Expressions;
using FixtureBuilder.Configuration;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ExpressionResolverTests
{
    internal sealed class ResolveDataMemberParentTests
    {
        [Test]
        public void NullRoot_ThrowsArgumentException()
        {
            var resolver = new ExpressionResolver(typeof(PropertyHolder<string>), Mock.Of<IFixtureContext>());
            Expression<Func<PropertyHolder<string>, string>> expression = x => x.Value;

            Assert.Throws<ArgumentException>(() => resolver.ResolveDataMemberParent(null!, expression, (_, _) => { }));
        }
    }
}
