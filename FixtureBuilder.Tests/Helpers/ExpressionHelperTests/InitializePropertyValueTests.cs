using FixtureBuilder.FixtureContexts;
using FixtureBuilder.Helpers;
using FixtureBuilder.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Helpers.ExpressionHelperTests
{
    internal sealed class InitializePropertyValueTests
    {
        private class Parent
        {
            public Child Child { get; set; } = null!;
            public Child PrePopulated { get; set; } = new();
            public Child ReadOnly { get; } = null!;
        }

        private class Child
        {
            public int Age { get; set; }
        }

        [Test]
        public void PropertyAlreadySet_ReturnsExistingValue()
        {
            var parent = new Parent();
            var prop = typeof(Parent).GetProperty(nameof(Parent.PrePopulated))!;
            var context = Mock.Of<IFixtureContext>();

            var result = ExpressionHelper.InitializePropertyValue(parent, prop, context);

            Assert.That(result, Is.SameAs(parent.PrePopulated));
        }

        [Test]
        public void PropertyAlreadySet_DoesNotCallContext()
        {
            var parent = new Parent();
            var prop = typeof(Parent).GetProperty(nameof(Parent.PrePopulated))!;
            var context = new Mock<IFixtureContext>();

            ExpressionHelper.InitializePropertyValue(parent, prop, context.Object);

            context.VerifyNoOtherCalls();
        }

        [Test]
        public void NullWritableProperty_ResolvesAndAssignsValue()
        {
            var parent = new Parent();
            var prop = typeof(Parent).GetProperty(nameof(Parent.Child))!;
            var resolved = new Child { Age = 10 };
            var context = new Mock<IFixtureContext>();
            var options = new FixtureOptions();
            context.Setup(c => c.Options).Returns(options);
            context.Setup(c => c.InstantiateWithStrategy(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Child)),
                    It.IsAny<InstantiationMethod>(), It.IsAny<InitializeMembers>()))
                .Returns(resolved);

            var result = ExpressionHelper.InitializePropertyValue(parent, prop, context.Object);

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
            var context = new Mock<IFixtureContext>();
            var options = new FixtureOptions { AllowInstantiateNestedMembers = false };
            context.Setup(c => c.Options).Returns(options);

            Assert.Throws<InvalidOperationException>(
                () => ExpressionHelper.InitializePropertyValue(parent, prop, context.Object));

        }

        [Test]
        public void NullReadOnlyProperty_ThrowsInvalidOperationException()
        {
            var parent = new Parent();
            var prop = typeof(Parent).GetProperty(nameof(Parent.ReadOnly))!;
            var context = new Mock<IFixtureContext>();
            var options = new FixtureOptions();
            context.Setup(c => c.Options).Returns(options);

            Assert.Throws<InvalidOperationException>(
                () => ExpressionHelper.InitializePropertyValue(parent, prop, context.Object));
        }
    }
}
