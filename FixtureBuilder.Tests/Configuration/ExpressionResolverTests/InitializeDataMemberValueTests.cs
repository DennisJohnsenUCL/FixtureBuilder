#pragma warning disable CA1822

using FixtureBuilder.Configuration;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ExpressionResolverTests
{
    internal sealed class InitializeDataMemberValueTests
    {
        private readonly ExpressionResolver _sut = new(typeof(object));

        private class Parent
        {
            public Child Child { get; set; } = null!;
            public Child PrePopulated { get; set; } = new();
            public Child ReadOnly { get; } = null!;
            public Child WriteOnly { set { } }

            public Child ChildField = null!;
            public Child PrePopulatedField = new();
        }

        private class Child
        {
            public int Age { get; set; }
        }

        private static DataMemberInfo Property(string name) =>
            new(typeof(Parent).GetProperty(name)!);

        private static DataMemberInfo Field(string name) =>
            new(typeof(Parent).GetField(name)!);

        private static Mock<IFixtureContext> MockContext(bool allowInstantiate = true)
        {
            var context = new Mock<IFixtureContext>();
            var options = new FixtureOptions { AllowInstantiateNestedMembers = allowInstantiate };
            context.Setup(c => c.OptionsFor(It.IsAny<Type>())).Returns(options);
            return context;
        }

        private static void SetupProvide(Mock<IFixtureContext> context, object? result)
        {
            context.Setup(c => c.ProvideWithStrategy(
                    It.IsAny<FixtureRequest>(),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(result!);
        }

        // --- Already set ---

        [Test]
        public void PropertyAlreadySet_ReturnsExistingValue()
        {
            var parent = new Parent();

            var result = _sut.InitializeDataMemberValue(parent, Property(nameof(Parent.PrePopulated)), Mock.Of<IFixtureContext>());

            Assert.That(result, Is.SameAs(parent.PrePopulated));
        }

        [Test]
        public void FieldAlreadySet_ReturnsExistingValue()
        {
            var parent = new Parent();

            var result = _sut.InitializeDataMemberValue(parent, Field(nameof(Parent.PrePopulatedField)), Mock.Of<IFixtureContext>());

            Assert.That(result, Is.SameAs(parent.PrePopulatedField));
        }

        [Test]
        public void PropertyAlreadySet_DoesNotCallContext()
        {
            var context = new Mock<IFixtureContext>();

            _sut.InitializeDataMemberValue(new Parent(), Property(nameof(Parent.PrePopulated)), context.Object);

            context.VerifyNoOtherCalls();
        }

        [Test]
        public void FieldAlreadySet_DoesNotCallContext()
        {
            var context = new Mock<IFixtureContext>();

            _sut.InitializeDataMemberValue(new Parent(), Field(nameof(Parent.PrePopulatedField)), context.Object);

            context.VerifyNoOtherCalls();
        }

        // --- Null writable members resolve and assign ---

        [Test]
        public void NullWritableProperty_ResolvesAndAssignsValue()
        {
            var parent = new Parent();
            var resolved = new Child { Age = 10 };
            var context = MockContext();
            SetupProvide(context, resolved);

            var result = _sut.InitializeDataMemberValue(parent, Property(nameof(Parent.Child)), context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.SameAs(resolved));
                Assert.That(parent.Child, Is.SameAs(resolved));
            }
        }

        [Test]
        public void NullWritableField_ResolvesAndAssignsValue()
        {
            var parent = new Parent();
            var resolved = new Child { Age = 20 };
            var context = MockContext();
            SetupProvide(context, resolved);

            var result = _sut.InitializeDataMemberValue(parent, Field(nameof(Parent.ChildField)), context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.SameAs(resolved));
                Assert.That(parent.ChildField, Is.SameAs(resolved));
            }
        }

        // --- Instantiation not allowed ---

        [Test]
        public void NullWritableProperty_MemberInstantiationNotAllowed_ThrowsException()
        {
            var context = MockContext(allowInstantiate: false);

            Assert.Throws<InvalidOperationException>(
                () => _sut.InitializeDataMemberValue(new Parent(), Property(nameof(Parent.Child)), context.Object));
        }

        [Test]
        public void NullWritableField_MemberInstantiationNotAllowed_ThrowsException()
        {
            var context = MockContext(allowInstantiate: false);

            Assert.Throws<InvalidOperationException>(
                () => _sut.InitializeDataMemberValue(new Parent(), Field(nameof(Parent.ChildField)), context.Object));
        }

        // --- Invalid property accessors ---

        [Test]
        public void NullReadOnlyProperty_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(
                () => _sut.InitializeDataMemberValue(new Parent(), Property(nameof(Parent.ReadOnly)), MockContext().Object));
        }

        [Test]
        public void WriteOnlyProperty_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(
                () => _sut.InitializeDataMemberValue(new Parent(), Property(nameof(Parent.WriteOnly)), Mock.Of<IFixtureContext>()));
        }

        // --- Provider returns null ---

        [Test]
        public void NullWritableProperty_ProviderReturnsNull_ThrowsInvalidOperationException()
        {
            var context = MockContext();
            SetupProvide(context, null);

            Assert.Throws<InvalidOperationException>(
                () => _sut.InitializeDataMemberValue(new Parent(), Property(nameof(Parent.Child)), context.Object));
        }

        [Test]
        public void NullWritableField_ProviderReturnsNull_ThrowsInvalidOperationException()
        {
            var context = MockContext();
            SetupProvide(context, null);

            Assert.Throws<InvalidOperationException>(
                () => _sut.InitializeDataMemberValue(new Parent(), Field(nameof(Parent.ChildField)), context.Object));
        }

        // --- Root type ---

        [Test]
        public void NullWritableProperty_PassesRootTypeToFixtureRequest()
        {
            var sut = new ExpressionResolver(typeof(Parent));
            var context = MockContext();
            SetupProvide(context, new Child());

            sut.InitializeDataMemberValue(new Parent(), Property(nameof(Parent.Child)), context.Object);

            context.Verify(c => c.ProvideWithStrategy(
                It.Is<FixtureRequest>(r => r.RootType == typeof(Parent)),
                It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()));
        }
    }
}
