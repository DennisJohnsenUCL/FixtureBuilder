#pragma warning disable CS0649

using System.Linq.Expressions;
using FixtureBuilder.Configuration;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
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
            context.Setup(c => c.UnwrapAndLink(It.IsAny<Type>(), It.IsAny<Type>())).Returns((Type type, Type _) => type);
            return context;
        }

        // --- Field-backed: single level ---

        [Test]
        public void SingleField_PassesRootAndFieldDataMember()
        {
            var root = new Root();
            var memberExpr = GetMemberExpr<Root, string>(x => x.NameField);
            var sut = CreateSut(Mock.Of<IFixtureContext>());

            object instance = null!;
            DataMemberInfo dataMember = null!;
            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (resolved, member) => (instance, dataMember) = (resolved, member));

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

            object instance = null!;
            DataMemberInfo dataMember = null!;
            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (resolved, member) => (instance, dataMember) = (resolved, member));

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

            object instance = null!;
            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (resolved, _) => instance = resolved);

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

            object instance = null!;
            DataMemberInfo dataMember = null!;
            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (resolved, member) => (instance, dataMember) = (resolved, member));

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

            object instance = null!;
            DataMemberInfo dataMember = null!;
            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (resolved, member) => (instance, dataMember) = (resolved, member));

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

            object instance = null!;
            DataMemberInfo dataMember = null!;
            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (resolved, member) => (instance, dataMember) = (resolved, member));

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

            object instance = null!;
            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (resolved, _) => instance = resolved);

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

            object instance = null!;
            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: true, (resolved, _) => instance = resolved);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance, Is.SameAs(resolvedLeaf));
                Assert.That(root.MiddleField.LeafField, Is.SameAs(resolvedLeaf));
            }
        }

        // --- Struct members: changes are written back ---

        private struct Point
        {
            public int X;
            public int Y { get; set; }
            public Leaf Leaf { get; set; }
        }

        private struct Segment
        {
            public Point Start { get; set; }
        }

        private class StructRoot
        {
            public Point PointField;
            public Point PointProperty { get; set; }
            public Segment Segment { get; set; }
            private readonly Point _readOnlyPoint;
            public Point ReadOnlyPoint => _readOnlyPoint;
        }

        [Test]
        public void StructFieldIntermediate_WritesChangeBack()
        {
            var root = new StructRoot();
            var memberExpr = GetMemberExpr<StructRoot, int>(x => x.PointField.X);
            var sut = CreateSut(Mock.Of<IFixtureContext>());

            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (instance, dataMember) => dataMember.SetValue(instance, 5));

            Assert.That(root.PointField.X, Is.EqualTo(5));
        }

        [Test]
        public void StructPropertyIntermediate_WritesChangeBack()
        {
            var root = new StructRoot();
            var memberExpr = GetMemberExpr<StructRoot, int>(x => x.PointProperty.Y);
            var sut = CreateSut(Mock.Of<IFixtureContext>());

            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (instance, dataMember) => dataMember.SetValue(instance, 5));

            Assert.That(root.PointProperty.Y, Is.EqualTo(5));
        }

        [Test]
        public void NestedStructIntermediates_WritesChangeBackThroughEveryLevel()
        {
            var root = new StructRoot();
            var memberExpr = GetMemberExpr<StructRoot, int>(x => x.Segment.Start.X);
            var sut = CreateSut(Mock.Of<IFixtureContext>());

            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (instance, dataMember) => dataMember.SetValue(instance, 5));

            Assert.That(root.Segment.Start.X, Is.EqualTo(5));
        }

        [Test]
        public void ResolveInstanceTrue_StructFinalMember_WritesChangeBack()
        {
            var root = new StructRoot();
            var memberExpr = GetMemberExpr<StructRoot, Point>(x => x.PointProperty);
            var sut = CreateSut(Mock.Of<IFixtureContext>());

            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: true, (instance, _) => typeof(Point).GetField(nameof(Point.X))!.SetValue(instance, 5));

            Assert.That(root.PointProperty.X, Is.EqualTo(5));
        }

        [Test]
        public void NullMemberInsideStruct_WritesInitializedValueBack()
        {
            var root = new StructRoot();
            var resolvedLeaf = new Leaf();
            var memberExpr = GetMemberExpr<StructRoot, string>(x => x.PointProperty.Leaf.Tag);
            var context = CreateContext();
            context.Setup(c => c.ProvideWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Leaf)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolvedLeaf);
            var sut = CreateSut(context.Object);

            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (_, _) => { });

            Assert.That(root.PointProperty.Leaf, Is.SameAs(resolvedLeaf));
        }

        [Test]
        public void ChangedStructInPropertyWithoutSetter_ThrowsInvalidOperationException()
        {
            var root = new StructRoot();
            var memberExpr = GetMemberExpr<StructRoot, int>(x => x.ReadOnlyPoint.X);
            var sut = CreateSut(Mock.Of<IFixtureContext>());

            Assert.Throws<InvalidOperationException>(
                () => sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (instance, dataMember) => dataMember.SetValue(instance, 5)));
        }

        [Test]
        public void UnchangedStructInPropertyWithoutSetter_DoesNotThrow()
        {
            var root = new StructRoot();
            var memberExpr = GetMemberExpr<StructRoot, int>(x => x.ReadOnlyPoint.X);
            var sut = CreateSut(Mock.Of<IFixtureContext>());

            Assert.DoesNotThrow(
                () => sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (_, _) => { }));
        }

        private struct Money
        {
            public decimal Amount { get; set; }
            public string Currency { get; set; }

            // Ignores Currency, so Equals cannot tell a copy with a changed Currency from the original
            public override readonly bool Equals(object? obj) => obj is Money other && other.Amount == Amount;
            public override readonly int GetHashCode() => Amount.GetHashCode();
        }

        private class MoneyRoot
        {
            private readonly Money _price;
            public Money Price => _price;
        }

        [Test]
        public void ChangedStructWithCustomEqualsInPropertyWithoutSetter_ThrowsInvalidOperationException()
        {
            var root = new MoneyRoot();
            var memberExpr = GetMemberExpr<MoneyRoot, string>(x => x.Price.Currency);
            var sut = CreateSut(Mock.Of<IFixtureContext>());

            Assert.Throws<InvalidOperationException>(
                () => sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (instance, dataMember) => dataMember.SetValue(instance, "EUR")));
        }

        // --- Nullable struct members: Value is skipped, so changes are written back to the nullable member ---

        private struct NullableSegment
        {
            public Point? End { get; set; }
        }

        private class NullableStructRoot
        {
            public Point? NullablePoint { get; set; }
            public Point? NullablePointField;
            public NullableSegment Segment { get; set; }
        }

        [Test]
        public void NullableStructPropertyIntermediate_WritesChangeBack()
        {
            var root = new NullableStructRoot { NullablePoint = new Point() };
            var memberExpr = GetMemberExpr<NullableStructRoot, int>(x => x.NullablePoint!.Value.X);
            var sut = CreateSut(Mock.Of<IFixtureContext>());

            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (instance, dataMember) => dataMember.SetValue(instance, 5));

            Assert.That(root.NullablePoint?.X, Is.EqualTo(5));
        }

        [Test]
        public void NullableStructFieldIntermediate_WritesChangeBack()
        {
            var root = new NullableStructRoot { NullablePointField = new Point() };
            var memberExpr = GetMemberExpr<NullableStructRoot, int>(x => x.NullablePointField!.Value.X);
            var sut = CreateSut(Mock.Of<IFixtureContext>());

            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (instance, dataMember) => dataMember.SetValue(instance, 5));

            Assert.That(root.NullablePointField?.X, Is.EqualTo(5));
        }

        [Test]
        public void NullNullableStructIntermediate_WritesInitializedValueBack()
        {
            var root = new NullableStructRoot();
            var memberExpr = GetMemberExpr<NullableStructRoot, int>(x => x.NullablePoint!.Value.X);
            var context = CreateContext();
            context.Setup(c => c.UnwrapAndLink(typeof(Point?), typeof(object))).Returns(typeof(Point));
            context.Setup(c => c.ProvideWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Point)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(new Point());
            var sut = CreateSut(context.Object);

            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (instance, dataMember) => dataMember.SetValue(instance, 5));

            Assert.That(root.NullablePoint?.X, Is.EqualTo(5));
        }

        [Test]
        public void NullableStructInsideStruct_WritesChangeBackThroughEveryLevel()
        {
            var root = new NullableStructRoot { Segment = new NullableSegment { End = new Point() } };
            var memberExpr = GetMemberExpr<NullableStructRoot, int>(x => x.Segment.End!.Value.X);
            var sut = CreateSut(Mock.Of<IFixtureContext>());

            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: false, (instance, dataMember) => dataMember.SetValue(instance, 5));

            Assert.That(root.Segment.End?.X, Is.EqualTo(5));
        }

        [Test]
        public void ResolveInstanceTrue_NullableStructFinalMember_WritesChangeBack()
        {
            var root = new NullableStructRoot { NullablePoint = new Point() };
            var memberExpr = GetMemberExpr<NullableStructRoot, Point>(x => x.NullablePoint!.Value);
            var sut = CreateSut(Mock.Of<IFixtureContext>());

            sut.ResolveDataMemberPath(memberExpr, root, resolveInstance: true, (instance, _) => typeof(Point).GetField(nameof(Point.X))!.SetValue(instance, 5));

            Assert.That(root.NullablePoint?.X, Is.EqualTo(5));
        }
    }
}
