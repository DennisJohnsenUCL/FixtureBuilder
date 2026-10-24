using System.Linq.Expressions;
using FixtureBuilder.Configuration;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ExpressionResolverTests
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

        private static ExpressionResolver CreateSut(IFixtureContext context, Type? rootType = null) =>
            new(rootType ?? typeof(object), context);

        [Test]
        public void NullRoot_ThrowsArgumentException()
        {
            var sut = CreateSut(Mock.Of<IFixtureContext>());
            Expression<Func<Root, int>> expr = x => x.Child.Value;

            Assert.Throws<ArgumentException>(
                () => sut.ResolveDataMemberInstance<Root, int>(null!, expr, (_, _) => { }));
        }

        [Test]
        public void PassedInstanceIsFinalMemberValue()
        {
            var sut = CreateSut(Mock.Of<IFixtureContext>());
            var root = new Root { Child = new Child { Value = 42 } };
            Expression<Func<Root, int>> expr = x => x.Child.Value;

            object instance = null!;
            sut.ResolveDataMemberInstance(root, expr, (resolved, _) => instance = resolved);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.EqualTo(42));
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
            context.Setup(c => c.OptionsFor(It.IsAny<Type>())).Returns(options);
            context.Setup(c => c.ProvideWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Child)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolvedChild);
            var sut = CreateSut(context.Object);

            object instance = null!;
            sut.ResolveDataMemberInstance(root, expr, (resolved, _) => instance = resolved);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(resolvedChild));
                Assert.That(root.Child, Is.SameAs(resolvedChild));
            }
        }
    }
}
