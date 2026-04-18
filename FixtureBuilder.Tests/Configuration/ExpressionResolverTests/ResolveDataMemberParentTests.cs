using System.Linq.Expressions;
using FixtureBuilder.Configuration;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ExpressionResolverTests
{
    internal sealed class ResolveDataMemberParentTests
    {
        private readonly ExpressionResolver _sut = new(typeof(object));

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

            Assert.Throws<ArgumentException>(
                () => _sut.ResolveDataMemberParent<Root, int>(null!, expr, Mock.Of<IFixtureContext>()));
        }

        [Test]
        public void DoesNotInitializeFinalMember()
        {
            var root = new Root { Child = new Child() };
            Expression<Func<Root, int>> expr = x => x.Child.Value;

            var (instance, dataMember) = _sut.ResolveDataMemberParent(root, expr, new Mock<IFixtureContext>().Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(root.Child));
                Assert.That(dataMember.Name, Is.EqualTo(nameof(Child.Value)));
            }
        }
    }
}
