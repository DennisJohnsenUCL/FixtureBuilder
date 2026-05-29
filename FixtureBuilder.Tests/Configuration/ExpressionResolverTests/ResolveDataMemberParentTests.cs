using System.Linq.Expressions;
using FixtureBuilder.Configuration;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ExpressionResolverTests
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

        private static ExpressionResolver CreateSut(IFixtureContext context, Type? rootType = null) =>
            new(rootType ?? typeof(object), context);

        [Test]
        public void NullRoot_ThrowsArgumentException()
        {
            var sut = CreateSut(Mock.Of<IFixtureContext>());
            Expression<Func<Root, int>> expr = x => x.Child.Value;

            Assert.Throws<ArgumentException>(
                () => sut.ResolveDataMemberParent<Root, int>(null!, expr));
        }

        [Test]
        public void DoesNotInitializeFinalMember()
        {
            var sut = CreateSut(new Mock<IFixtureContext>().Object);
            var root = new Root { Child = new Child() };
            Expression<Func<Root, int>> expr = x => x.Child.Value;

            var (instance, dataMember) = sut.ResolveDataMemberParent(root, expr);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(root.Child));
                Assert.That(dataMember.Name, Is.EqualTo(nameof(Child.Value)));
            }
        }
    }
}
