#pragma warning disable CA1822

using System.Linq.Expressions;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.Helpers;
using FixtureBuilder.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Helpers.ExpressionHelperTests
{
    internal sealed class ResolveMethodParentTests
    {
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
            var context = Mock.Of<IFixtureContext>();

            Assert.Throws<ArgumentException>(
                () => ExpressionHelper.ResolveMethodParent<Root>(null!, expr, context));
        }

        [Test]
        public void MethodDirectlyOnRoot_DoesNotCallContext()
        {
            var root = new Root();
            Expression<Action<Root>> expr = x => x.RootMethod();
            var context = new Mock<IFixtureContext>();

            ExpressionHelper.ResolveMethodParent(root, expr, context.Object);

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
            context.Setup(c => c.InstantiateWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Child)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolvedChild);

            ExpressionHelper.ResolveMethodParent(root, expr, context.Object);

            Assert.That(root.Child, Is.SameAs(resolvedChild));
        }

        [Test]
        public void MethodOnPrePopulatedIntermediate_DoesNotCallContext()
        {
            var existingChild = new Child();
            var root = new Root { Child = existingChild };
            Expression<Action<Root>> expr = x => x.Child.DoWork();
            var context = new Mock<IFixtureContext>();

            ExpressionHelper.ResolveMethodParent(root, expr, context.Object);

            Assert.That(root.Child, Is.SameAs(existingChild));
            context.VerifyNoOtherCalls();
        }
    }
}
