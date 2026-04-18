#pragma warning disable CA1822

using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.ExpressionValidatorTests
{
    internal sealed class ValidateMethodExpressionTests
    {
        private class Outer
        {
            public Inner Prop { get; set; } = new();
            public Inner Field = new();
            public int GetValue() => 42;
        }

        private class Inner
        {
            public int DoSomething() => 1;
        }

        [Test]
        public void MethodOnProperty_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                ExpressionValidator.ValidateMethodExpression<Outer>(x => x.Prop.DoSomething()));
        }

        [Test]
        public void MethodOnField_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                ExpressionValidator.ValidateMethodExpression<Outer>(x => x.Field.DoSomething()));
        }

        [Test]
        public void MethodDirectlyOnParameter_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                ExpressionValidator.ValidateMethodExpression<Outer>(x => x.GetValue()));
        }

        [Test]
        public void StaticMethod_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                ExpressionValidator.ValidateMethodExpression<Outer>(x => Console.WriteLine()));
        }

        [Test]
        public void MethodOnCapturedVariable_Throws()
        {
            var outer = new Outer();
            Assert.Throws<InvalidOperationException>(() =>
                ExpressionValidator.ValidateMethodExpression<Outer>(x => outer.GetValue()));
        }
    }
}
