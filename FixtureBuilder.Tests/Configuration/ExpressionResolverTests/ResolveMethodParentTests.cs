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
        private class Root
        {
            public Child Child { get; set; } = null!;

            public void RootMethod() { }
        }

        private class Child
        {
            public void DoWork() { }
        }

        private static ExpressionResolver CreateSut(IFixtureContext context, Type? rootType = null) =>
            new(rootType ?? typeof(object), context);

        [Test]
        public void NullRoot_ThrowsArgumentException()
        {
            var sut = CreateSut(Mock.Of<IFixtureContext>());
            Expression<Action<Root>> expr = x => x.RootMethod();

            Assert.Throws<ArgumentException>(
                () => sut.ResolveMethodParent<Root>(null!, expr));
        }

        [Test]
        public void MethodDirectlyOnRoot_DoesNotCallContext()
        {
            Expression<Action<Root>> expr = x => x.RootMethod();
            var context = new Mock<IFixtureContext>();
            var sut = CreateSut(context.Object);

            sut.ResolveMethodParent(new Root(), expr);

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
            context.Setup(c => c.OptionsFor(It.IsAny<Type>())).Returns(options);
            context.Setup(c => c.ProvideWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Child)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolvedChild);
            var sut = CreateSut(context.Object);

            sut.ResolveMethodParent(root, expr);

            Assert.That(root.Child, Is.SameAs(resolvedChild));
        }

        [Test]
        public void MethodOnPrePopulatedIntermediate_DoesNotCallContext()
        {
            var existingChild = new Child();
            var root = new Root { Child = existingChild };
            Expression<Action<Root>> expr = x => x.Child.DoWork();
            var context = new Mock<IFixtureContext>();
            var sut = CreateSut(context.Object);

            sut.ResolveMethodParent(root, expr);

            Assert.That(root.Child, Is.SameAs(existingChild));
            context.VerifyNoOtherCalls();
        }

        private struct ChildHolder
        {
            public Child Child { get; set; }
        }

        private class StructRoot
        {
            public ChildHolder Holder { get; set; }
        }

        [Test]
        public void MethodOnNullMemberInsideStruct_WritesInitializedValueBack()
        {
            var root = new StructRoot();
            var resolvedChild = new Child();
            Expression<Action<StructRoot>> expr = x => x.Holder.Child.DoWork();
            var context = new Mock<IFixtureContext>();
            var options = new FixtureOptions();
            context.Setup(c => c.OptionsFor(It.IsAny<Type>())).Returns(options);
            context.Setup(c => c.ProvideWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Child)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolvedChild);
            var sut = CreateSut(context.Object);

            sut.ResolveMethodParent(root, expr);

            Assert.That(root.Holder.Child, Is.SameAs(resolvedChild));
        }
    }
}
