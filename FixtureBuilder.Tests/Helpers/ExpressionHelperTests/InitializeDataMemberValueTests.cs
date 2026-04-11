#pragma warning disable CA1822

using FixtureBuilder.FixtureContexts;
using FixtureBuilder.Helpers;
using FixtureBuilder.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Helpers.ExpressionHelperTests
{
    internal sealed class InitializeDataMemberValueTests
    {
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

        // --- Property-backed ---

        [Test]
        public void PropertyAlreadySet_ReturnsExistingValue()
        {
            var parent = new Parent();
            var prop = typeof(Parent).GetProperty(nameof(Parent.PrePopulated))!;
            var dataMember = new DataMemberInfo(prop);
            var context = Mock.Of<IFixtureContext>();

            var result = ExpressionHelper.InitializeDataMemberValue(parent, dataMember, context);

            Assert.That(result, Is.SameAs(parent.PrePopulated));
        }

        [Test]
        public void PropertyAlreadySet_DoesNotCallContext()
        {
            var parent = new Parent();
            var prop = typeof(Parent).GetProperty(nameof(Parent.PrePopulated))!;
            var dataMember = new DataMemberInfo(prop);
            var context = new Mock<IFixtureContext>();

            ExpressionHelper.InitializeDataMemberValue(parent, dataMember, context.Object);

            context.VerifyNoOtherCalls();
        }

        [Test]
        public void NullWritableProperty_ResolvesAndAssignsValue()
        {
            var parent = new Parent();
            var prop = typeof(Parent).GetProperty(nameof(Parent.Child))!;
            var dataMember = new DataMemberInfo(prop);
            var resolved = new Child { Age = 10 };
            var context = new Mock<IFixtureContext>();
            var options = new FixtureOptions();
            context.Setup(c => c.Options).Returns(options);
            context.Setup(c => c.InstantiateWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Child)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolved);

            var result = ExpressionHelper.InitializeDataMemberValue(parent, dataMember, context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.SameAs(resolved));
                Assert.That(parent.Child, Is.SameAs(resolved));
            }
        }

        [Test]
        public void NullWritableProperty_MemberInstantiationNotAllowed_ThrowsException()
        {
            var parent = new Parent();
            var prop = typeof(Parent).GetProperty(nameof(Parent.Child))!;
            var dataMember = new DataMemberInfo(prop);
            var context = new Mock<IFixtureContext>();
            var options = new FixtureOptions { AllowInstantiateNestedMembers = false };
            context.Setup(c => c.Options).Returns(options);

            Assert.Throws<InvalidOperationException>(
                () => ExpressionHelper.InitializeDataMemberValue(parent, dataMember, context.Object));
        }

        [Test]
        public void NullReadOnlyProperty_ThrowsInvalidOperationException()
        {
            var parent = new Parent();
            var prop = typeof(Parent).GetProperty(nameof(Parent.ReadOnly))!;
            var dataMember = new DataMemberInfo(prop);
            var context = new Mock<IFixtureContext>();
            var options = new FixtureOptions();
            context.Setup(c => c.Options).Returns(options);

            Assert.Throws<InvalidOperationException>(
                () => ExpressionHelper.InitializeDataMemberValue(parent, dataMember, context.Object));
        }

        [Test]
        public void WriteOnlyProperty_ThrowsInvalidOperationException()
        {
            var parent = new Parent();
            var prop = typeof(Parent).GetProperty(nameof(Parent.WriteOnly))!;
            var dataMember = new DataMemberInfo(prop);
            var context = Mock.Of<IFixtureContext>();

            Assert.Throws<InvalidOperationException>(
                () => ExpressionHelper.InitializeDataMemberValue(parent, dataMember, context));
        }

        // --- Field-backed ---

        [Test]
        public void FieldAlreadySet_ReturnsExistingValue()
        {
            var parent = new Parent();
            var field = typeof(Parent).GetField(nameof(Parent.PrePopulatedField))!;
            var dataMember = new DataMemberInfo(field);
            var context = Mock.Of<IFixtureContext>();

            var result = ExpressionHelper.InitializeDataMemberValue(parent, dataMember, context);

            Assert.That(result, Is.SameAs(parent.PrePopulatedField));
        }

        [Test]
        public void FieldAlreadySet_DoesNotCallContext()
        {
            var parent = new Parent();
            var field = typeof(Parent).GetField(nameof(Parent.PrePopulatedField))!;
            var dataMember = new DataMemberInfo(field);
            var context = new Mock<IFixtureContext>();

            ExpressionHelper.InitializeDataMemberValue(parent, dataMember, context.Object);

            context.VerifyNoOtherCalls();
        }

        [Test]
        public void NullWritableField_ResolvesAndAssignsValue()
        {
            var parent = new Parent();
            var field = typeof(Parent).GetField(nameof(Parent.ChildField))!;
            var dataMember = new DataMemberInfo(field);
            var resolved = new Child { Age = 20 };
            var context = new Mock<IFixtureContext>();
            var options = new FixtureOptions();
            context.Setup(c => c.Options).Returns(options);
            context.Setup(c => c.InstantiateWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Child)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolved);

            var result = ExpressionHelper.InitializeDataMemberValue(parent, dataMember, context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.SameAs(resolved));
                Assert.That(parent.ChildField, Is.SameAs(resolved));
            }
        }

        [Test]
        public void NullWritableField_MemberInstantiationNotAllowed_ThrowsException()
        {
            var parent = new Parent();
            var field = typeof(Parent).GetField(nameof(Parent.ChildField))!;
            var dataMember = new DataMemberInfo(field);
            var context = new Mock<IFixtureContext>();
            var options = new FixtureOptions { AllowInstantiateNestedMembers = false };
            context.Setup(c => c.Options).Returns(options);

            Assert.Throws<InvalidOperationException>(
                () => ExpressionHelper.InitializeDataMemberValue(parent, dataMember, context.Object));
        }
    }
}
