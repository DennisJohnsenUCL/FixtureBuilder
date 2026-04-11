using System.Linq.Expressions;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.Helpers;
using FixtureBuilder.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Helpers.ExpressionHelperTests
{
    internal sealed class ResolveDataMemberInstanceTests
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
                () => ExpressionHelper.ResolveDataMemberInstance<Root, int>(null!, expr, context));
        }

        [Test]
        public void ReturnedInstanceIsFinalMemberValue()
        {
            var root = new Root { Child = new Child { Value = 42 } };
            Expression<Func<Root, int>> expr = x => x.Child.Value;
            var context = Mock.Of<IFixtureContext>();

            var (instance, dataMember) = ExpressionHelper.ResolveDataMemberInstance(root, expr, context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.EqualTo(42));
                Assert.That(dataMember.Name, Is.EqualTo(nameof(Child.Value)));
            }
        }

        [Test]
        public void NullFinalMember_InitializesIt()
        {
            var resolvedChild = new Child();
            var root = new Root();
            Expression<Func<Root, Child>> expr = x => x.Child;
            var context = new Mock<IFixtureContext>();
            var options = new FixtureOptions();
            context.Setup(c => c.Options).Returns(options);
            context.Setup(c => c.ProvideWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Child)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolvedChild);

            var (instance, dataMember) = ExpressionHelper.ResolveDataMemberInstance(root, expr, context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(resolvedChild));
                Assert.That(root.Child, Is.SameAs(resolvedChild));
            }
        }
    }
}
