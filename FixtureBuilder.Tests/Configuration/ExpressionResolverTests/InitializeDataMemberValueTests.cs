using FixtureBuilder.Configuration;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ExpressionResolverTests
{
    internal sealed class InitializeDataMemberValueTests
    {
        // A member expression cannot read a write-only property, so no public method can reach this check.
        [Test]
        public void WriteOnlyProperty_ThrowsInvalidOperationException()
        {
            var resolver = new ExpressionResolver(typeof(WriteOnlyPropertyClass), Mock.Of<IFixtureContext>());
            var member = new DataMemberInfo(typeof(WriteOnlyPropertyClass).GetProperty(nameof(WriteOnlyPropertyClass.Value))!);

            Assert.Throws<InvalidOperationException>(() => resolver.InitializeDataMemberValue(new WriteOnlyPropertyClass(), member));
        }
    }
}
