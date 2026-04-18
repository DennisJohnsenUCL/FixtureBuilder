#pragma warning disable CA1822

using System.Linq.Expressions;
using FixtureBuilder.Configuration;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ExpressionResolverTests
{
    internal sealed class ResolveMethodParentTests
    {
        private readonly ExpressionResolver _sut = new(typeof(object));

        private class Root
        {
            public Child Child { get; set; } = null!;

            public void RootMethod() { }
        }

        private class Child
        {
            public void DoWork() { }
        }

        [Test]
        public void NullRoot_ThrowsArgumentException()
        {
            Expression<Action<Root>> expr = x => x.RootMethod();

            Assert.Throws<ArgumentException>(
                () => _sut.ResolveMethodParent<Root>(null!, expr, Mock.Of<IFixtureContext>()));
        }

        [Test]
        public void MethodDirectlyOnRoot_DoesNotCallContext()
        {
            Expression<Action<Root>> expr = x => x.RootMethod();
            var context = new Mock<IFixtureContext>();

            _sut.ResolveMethodParent(new Root(), expr, context.Object);

            context.VerifyNoOtherCalls();
        }

        [Test]
        public void MethodOnNullIntermediate_InitializesIntermediate()
        {
            var root = new Root();
            var resolvedChild = new Child();
            Expression<Action<Root>> expr = x => x.Child.DoWork();
            var context = new Mock<IFixtureContext>();
            var options = new FixtureOptions();
            context.Setup(c => c.Options).Returns(options);
            context.Setup(c => c.ProvideWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Child)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolvedChild);

            _sut.ResolveMethodParent(root, expr, context.Object);

            Assert.That(root.Child, Is.SameAs(resolvedChild));
        }

        [Test]
        public void MethodOnPrePopulatedIntermediate_DoesNotCallContext()
        {
            var existingChild = new Child();
            var root = new Root { Child = existingChild };
            Expression<Action<Root>> expr = x => x.Child.DoWork();
            var context = new Mock<IFixtureContext>();

            _sut.ResolveMethodParent(root, expr, context.Object);

            Assert.That(root.Child, Is.SameAs(existingChild));
            context.VerifyNoOtherCalls();
        }
    }
}
