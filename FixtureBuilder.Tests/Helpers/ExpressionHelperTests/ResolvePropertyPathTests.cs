using System.Linq.Expressions;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.Helpers;
using FixtureBuilder.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Helpers.ExpressionHelperTests
{
    internal class ResolvePropertyPathTests
    {
        private class Root
        {
            public Middle Middle { get; set; } = null!;
            public string Name { get; set; } = "hello";
        }

        private class Middle
        {
            public Leaf Leaf { get; set; } = null!;
            public int Value { get; set; }
        }

        private class Leaf
        {
            public string Tag { get; set; } = "";
        }

        private static Mock<IFixtureContext> CreateContextThatResolves<T>(T instance) where T : class
        {
            var context = new Mock<IFixtureContext>();
            context
                .Setup(c => c.ResolveUninitialized(
                    It.Is<FixtureRequest>(r => r.Type == typeof(T)),
                    InitializeMembers.None,
                    context.Object))
                .Returns(instance);
            return context;
        }

        [Test]
        public void NullRoot_ThrowsArgumentException()
        {
            Expression<Func<Root, string>> expr = x => x.Name;
            var context = Mock.Of<IFixtureContext>();

            Assert.Throws<ArgumentException>(
                () => ExpressionHelper.ResolvePropertyPath<Root, string>(null!, expr, false, context));
        }

        [Test]
        public void SingleProperty_ReturnsRootAndProperty()
        {
            var root = new Root();
            Expression<Func<Root, string>> expr = x => x.Name;
            var context = Mock.Of<IFixtureContext>();

            var (instance, property) = ExpressionHelper.ResolvePropertyPath(root, expr, false, context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(root));
                Assert.That(property.Name, Is.EqualTo(nameof(Root.Name)));
            }
        }

        [Test]
        public void TwoLevelChain_InitializesIntermediate_ReturnsMiddleAndFinalProperty()
        {
            var root = new Root();
            var resolvedMiddle = new Middle();
            Expression<Func<Root, int>> expr = x => x.Middle.Value;
            var context = CreateContextThatResolves(resolvedMiddle);

            var (instance, property) = ExpressionHelper.ResolvePropertyPath(root, expr, false, context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(resolvedMiddle));
                Assert.That(root.Middle, Is.SameAs(resolvedMiddle));
                Assert.That(property.Name, Is.EqualTo(nameof(Middle.Value)));
            }
        }

        [Test]
        public void ThreeLevelChain_InitializesAllIntermediates()
        {
            var root = new Root();
            var resolvedMiddle = new Middle();
            var resolvedLeaf = new Leaf();
            Expression<Func<Root, string>> expr = x => x.Middle.Leaf.Tag;
            var context = new Mock<IFixtureContext>();
            context
                .Setup(c => c.ResolveUninitialized(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Middle)),
                    InitializeMembers.None,
                    context.Object))
                .Returns(resolvedMiddle);
            context
                .Setup(c => c.ResolveUninitialized(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Leaf)),
                    InitializeMembers.None,
                    context.Object))
                .Returns(resolvedLeaf);

            var (instance, property) = ExpressionHelper.ResolvePropertyPath(root, expr, false, context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(resolvedLeaf));
                Assert.That(root.Middle, Is.SameAs(resolvedMiddle));
                Assert.That(root.Middle.Leaf, Is.SameAs(resolvedLeaf));
                Assert.That(property.Name, Is.EqualTo(nameof(Leaf.Tag)));
            }
        }

        [Test]
        public void PrePopulatedIntermediates_DoesNotCallContext()
        {
            var existingMiddle = new Middle { Value = 99 };
            var root = new Root { Middle = existingMiddle };
            Expression<Func<Root, int>> expr = x => x.Middle.Value;
            var context = new Mock<IFixtureContext>();

            var (instance, property) = ExpressionHelper.ResolvePropertyPath(root, expr, false, context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(existingMiddle));
                Assert.That(property.Name, Is.EqualTo(nameof(Middle.Value)));
            }
            context.VerifyNoOtherCalls();
        }

        [Test]
        public void InstantiateTargetFalse_DoesNotInitializeFinalProperty()
        {
            var root = new Root { Middle = new Middle() };
            Expression<Func<Root, Leaf>> expr = x => x.Middle.Leaf;
            var context = new Mock<IFixtureContext>();

            var (instance, property) = ExpressionHelper.ResolvePropertyPath(root, expr, false, context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(root.Middle));
                Assert.That(root.Middle.Leaf, Is.Null);
                Assert.That(property.Name, Is.EqualTo(nameof(Middle.Leaf)));
            }
            context.VerifyNoOtherCalls();
        }

        [Test]
        public void InstantiateTargetTrue_InitializesFinalProperty()
        {
            var root = new Root { Middle = new Middle() };
            var resolvedLeaf = new Leaf();
            Expression<Func<Root, Leaf>> expr = x => x.Middle.Leaf;
            var context = CreateContextThatResolves(resolvedLeaf);

            var (instance, property) = ExpressionHelper.ResolvePropertyPath(root, expr, true, context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(resolvedLeaf));
                Assert.That(root.Middle.Leaf, Is.SameAs(resolvedLeaf));
                Assert.That(property.Name, Is.EqualTo(nameof(Middle.Leaf)));
            }
        }
    }
}
