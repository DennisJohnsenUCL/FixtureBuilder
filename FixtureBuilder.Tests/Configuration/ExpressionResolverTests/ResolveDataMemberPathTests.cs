using System.Linq.Expressions;
using FixtureBuilder.Configuration;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ExpressionResolverTests
{
    internal sealed class ResolveDataMemberPathTests
    {
        private class Root
        {
            public Middle Middle { get; set; } = null!;
            public Middle MiddleField = null!;
            public string Name { get; set; } = "hello";
            public string NameField = "hello";
        }

        private class Middle
        {
            public Leaf Leaf { get; set; } = null!;
            public Leaf LeafField = null!;
            public int Value { get; set; }
            public int ValueField;
        }

        private class Leaf
        {
            public string Tag { get; set; } = "";
            public string TagField = "";
        }

        private static MemberExpression GetMemberExpr<T, TProp>(Expression<Func<T, TProp>> expr)
            => (MemberExpression)expr.Body;

        private static ExpressionResolver CreateSut(IFixtureContext context, Type? rootType = null) =>
            new(rootType ?? typeof(object), context);

        private static Mock<IFixtureContext> CreateContext()
        {
            var context = new Mock<IFixtureContext>();
            var options = new FixtureOptions();
            context.Setup(c => c.OptionsFor(It.IsAny<Type>())).Returns(options);
            return context;
        }

        // --- Field-backed: single level ---

        [Test]
        public void SingleField_ReturnsRootAndFieldDataMember()
        {
            var root = new Root();
            var memberExpr = GetMemberExpr<Root, string>(x => x.NameField);
            var sut = CreateSut(Mock.Of<IFixtureContext>());

            var (instance, dataMember) = sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(root));
                Assert.That(dataMember.Name, Is.EqualTo(nameof(Root.NameField)));
                Assert.That(dataMember.IsFieldInfo, Is.True);
            }
        }

        // --- Field-backed: two-level chain ---

        [Test]
        public void TwoLevelFieldChain_InitializesIntermediate()
        {
            var root = new Root();
            var resolvedMiddle = new Middle();
            var memberExpr = GetMemberExpr<Root, int>(x => x.MiddleField.ValueField);
            var context = CreateContext();
            context.Setup(c => c.ProvideWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Middle)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolvedMiddle);
            var sut = CreateSut(context.Object);

            var (instance, dataMember) = sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(resolvedMiddle));
                Assert.That(root.MiddleField, Is.SameAs(resolvedMiddle));
                Assert.That(dataMember.Name, Is.EqualTo(nameof(Middle.ValueField)));
            }
        }

        // --- Field-backed: pre-populated ---

        [Test]
        public void PrePopulatedFieldIntermediate_DoesNotCallContext()
        {
            var existingMiddle = new Middle { ValueField = 42 };
            var root = new Root { MiddleField = existingMiddle };
            var memberExpr = GetMemberExpr<Root, int>(x => x.MiddleField.ValueField);
            var context = new Mock<IFixtureContext>();
            var sut = CreateSut(context.Object);

            var (instance, dataMember) = sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false);

            Assert.That(instance, Is.SameAs(existingMiddle));
            context.VerifyNoOtherCalls();
        }

        // --- Field-backed: three-level chain ---

        [Test]
        public void ThreeLevelFieldChain_InitializesAllIntermediates()
        {
            var root = new Root();
            var resolvedMiddle = new Middle();
            var resolvedLeaf = new Leaf();
            var memberExpr = GetMemberExpr<Root, string>(x => x.MiddleField.LeafField.TagField);
            var context = CreateContext();
            context.Setup(c => c.ProvideWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Middle)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolvedMiddle);
            context.Setup(c => c.ProvideWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Leaf)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolvedLeaf);
            var sut = CreateSut(context.Object);

            var (instance, dataMember) = sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(resolvedLeaf));
                Assert.That(root.MiddleField, Is.SameAs(resolvedMiddle));
                Assert.That(root.MiddleField.LeafField, Is.SameAs(resolvedLeaf));
                Assert.That(dataMember.Name, Is.EqualTo(nameof(Leaf.TagField)));
            }
        }

        // --- Mixed chains ---

        [Test]
        public void MixedChain_PropertyIntermediateFieldFinal()
        {
            var root = new Root();
            var resolvedMiddle = new Middle();
            var memberExpr = GetMemberExpr<Root, int>(x => x.Middle.ValueField);
            var context = CreateContext();
            context.Setup(c => c.ProvideWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Middle)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolvedMiddle);
            var sut = CreateSut(context.Object);

            var (instance, dataMember) = sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(resolvedMiddle));
                Assert.That(root.Middle, Is.SameAs(resolvedMiddle));
                Assert.That(dataMember.IsFieldInfo, Is.True);
            }
        }

        [Test]
        public void MixedChain_FieldIntermediatePropertyFinal()
        {
            var root = new Root();
            var resolvedMiddle = new Middle();
            var memberExpr = GetMemberExpr<Root, Leaf>(x => x.MiddleField.Leaf);
            var context = CreateContext();
            context.Setup(c => c.ProvideWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Middle)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolvedMiddle);
            var sut = CreateSut(context.Object);

            var (instance, dataMember) = sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(resolvedMiddle));
                Assert.That(root.MiddleField, Is.SameAs(resolvedMiddle));
                Assert.That(dataMember.IsPropertyInfo, Is.True);
            }
        }

        // --- resolveInstance flag ---

        [Test]
        public void ResolveInstanceFalse_DoesNotInitializeFinalMember()
        {
            var root = new Root { MiddleField = new Middle() };
            var memberExpr = GetMemberExpr<Root, Leaf>(x => x.MiddleField.LeafField);
            var context = new Mock<IFixtureContext>();
            var sut = CreateSut(context.Object);

            var (instance, dataMember) = sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(root.MiddleField));
                Assert.That(root.MiddleField.LeafField, Is.Null);
            }
            context.VerifyNoOtherCalls();
        }

        [Test]
        public void ResolveInstanceTrue_InitializesFinalMember()
        {
            var root = new Root { MiddleField = new Middle() };
            var resolvedLeaf = new Leaf();
            var memberExpr = GetMemberExpr<Root, Leaf>(x => x.MiddleField.LeafField);
            var context = CreateContext();
            context.Setup(c => c.ProvideWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Leaf)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolvedLeaf);
            var sut = CreateSut(context.Object);

            var (instance, dataMember) = sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: true);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(resolvedLeaf));
                Assert.That(root.MiddleField.LeafField, Is.SameAs(resolvedLeaf));
            }
        }
    }
}
