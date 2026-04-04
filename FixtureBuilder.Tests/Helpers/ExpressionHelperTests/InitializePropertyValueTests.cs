using FixtureBuilder.FixtureContexts;
using FixtureBuilder.Helpers;
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
            context
                .Setup(c => c.AutoResolve(
                    It.Is<FixtureRequest>(r => r.Type == typeof(Child)),
                    context.Object))
                .Returns(resolved);

            var result = ExpressionHelper.InitializePropertyValue(parent, prop, context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.SameAs(resolved));
                Assert.That(parent.Child, Is.SameAs(resolved));
            }
        }

        [Test]
        public void NullReadOnlyProperty_ThrowsInvalidOperationException()
        {
            var parent = new Parent();
            var prop = typeof(Parent).GetProperty(nameof(Parent.ReadOnly))!;
            var context = Mock.Of<IFixtureContext>();

            Assert.Throws<InvalidOperationException>(
                () => ExpressionHelper.InitializePropertyValue(parent, prop, context));
        }
    }
}
