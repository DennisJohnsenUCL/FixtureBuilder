#pragma warning disable CA1822

using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.ExpressionHelperTests
{
    internal sealed class ValidateExpressionTests
    {
        private class Outer
        {
            public Inner Prop { get; set; } = new();
            public Inner Field = new();
            public string Name { get; set; } = "";
            public string NameField = "";
            public int GetValue() => 42;
        }

        private class Inner
        {
            public string Value { get; set; } = "";
        }

        [Test]
        public void SingleProperty_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                ExpressionHelper.ValidateExpression<Outer, string>(x => x.Name));
        }

        [Test]
        public void SingleField_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                ExpressionHelper.ValidateExpression<Outer, string>(x => x.NameField));
        }

        [Test]
        public void NestedPropertyChain_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                ExpressionHelper.ValidateExpression<Outer, string>(x => x.Prop.Value));
        }

        [Test]
        public void MixedFieldThenProperty_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                ExpressionHelper.ValidateExpression<Outer, string>(x => x.Field.Value));
        }

        [Test]
        public void MethodCall_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                ExpressionHelper.ValidateExpression<Outer, int>(x => x.GetValue()));
        }

        [Test]
        public void Constant_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                ExpressionHelper.ValidateExpression<Outer, string>(x => "hello"));
        }

        [Test]
        public void BareParameter_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                ExpressionHelper.ValidateExpression<Outer, Outer>(x => x));
        }

        [Test]
        public void CapturedVariable_Throws()
        {
            var outer = new Outer();
            Assert.Throws<InvalidOperationException>(() =>
                ExpressionHelper.ValidateExpression<Outer, string>(x => outer.Name));
        }
    }
}
