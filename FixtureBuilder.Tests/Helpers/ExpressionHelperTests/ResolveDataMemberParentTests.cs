using System.Linq.Expressions;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.Helpers;
using Moq;

namespace FixtureBuilder.Tests.Helpers.ExpressionHelperTests
{
    internal sealed class ResolveDataMemberParentTests
    {
        private class Root
        {
            public Child Child { get; set; } = null!;
        }

        private class Child
        {
            public int Value { get; set; }
        }

        [Test]
        public void NullRoot_ThrowsArgumentException()
        {
            Expression<Func<Root, int>> expr = x => x.Child.Value;
            var context = Mock.Of<IFixtureContext>();

            Assert.Throws<ArgumentException>(
                () => ExpressionHelper.ResolveDataMemberParent<Root, int>(null!, expr, context));
        }

        [Test]
        public void DoesNotInitializeFinalMember()
        {
            var root = new Root { Child = new Child() };
            Expression<Func<Root, int>> expr = x => x.Child.Value;
            var context = new Mock<IFixtureContext>();

            var (instance, dataMember) = ExpressionHelper.ResolveDataMemberParent(root, expr, context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(root.Child));
                Assert.That(dataMember.Name, Is.EqualTo(nameof(Child.Value)));
            }
        }
    }
}
