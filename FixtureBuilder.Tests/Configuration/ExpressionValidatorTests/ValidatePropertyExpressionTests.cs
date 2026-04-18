using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.ExpressionValidatorTests
{
    internal sealed class ValidatePropertyExpressionTests
    {
        private class Outer
        {
            public Inner Prop { get; set; } = new();
            public Inner Field = new();
            public string Name { get; set; } = "";
            public string NameField = "";
        }

        private class Inner
        {
            public string Value { get; set; } = "";
            public string ValueField = "";
        }

        [Test]
        public void SingleProperty_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                ExpressionValidator.ValidatePropertyExpression<Outer, string>(x => x.Name));
        }

        [Test]
        public void NestedProperty_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                ExpressionValidator.ValidatePropertyExpression<Outer, string>(x => x.Prop.Value));
        }

        [Test]
        public void FieldThenProperty_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                ExpressionValidator.ValidatePropertyExpression<Outer, string>(x => x.Field.Value));
        }

        [Test]
        public void SingleField_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                ExpressionValidator.ValidatePropertyExpression<Outer, string>(x => x.NameField));
        }

        [Test]
        public void PropertyThenField_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                ExpressionValidator.ValidatePropertyExpression<Outer, string>(x => x.Prop.ValueField));
        }

        [Test]
        public void BareParameter_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                ExpressionValidator.ValidatePropertyExpression<Outer, Outer>(x => x));
        }

        [Test]
        public void Constant_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                ExpressionValidator.ValidatePropertyExpression<Outer, string>(x => "hello"));
        }
    }
}
