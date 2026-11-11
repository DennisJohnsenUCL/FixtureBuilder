using System.Linq.Expressions;
using FixtureBuilder.Configuration;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ExpressionResolverTests
{
    internal sealed class ResolveDataMemberPathTests
    {
        // Configuration methods reject expressions ending in Nullable<T>.Value, so no public method can reach this path.
        [Test]
        public void ResolveInstance_ChainEndingInNullableValue_WritesChangedStructBack()
        {
            var root = new PropertyHolder<PointStruct?> { Value = new PointStruct() };
            Expression<Func<PropertyHolder<PointStruct?>, PointStruct>> expression = x => x.Value!.Value;
            var memberExpression = (MemberExpression)expression.Body;
            var resolver = new ExpressionResolver(typeof(PropertyHolder<PointStruct?>), Mock.Of<IFixtureContext>());

            resolver.ResolveDataMemberPath(memberExpression, root, resolveInstance: true,
                (instance, _) => typeof(PointStruct).GetProperty(nameof(PointStruct.X))!.SetValue(instance, 5));

            Assert.That(root.Value?.X, Is.EqualTo(5));
        }
    }
}
