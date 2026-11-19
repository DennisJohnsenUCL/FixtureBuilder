using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureTests.WithBackingField
{
    internal sealed class WithBackingFieldTuplesTests
    {
        private class ObjectElementsFieldClass
        {
            private readonly (object, object?) _value;
            public (int, string?) Value => ((int, string?))_value;
        }

        #region Element conversions

        [Test]
        public void ObjectElementsField_SetsConvertedTuple()
        {
            var fixture = TestHelper.MakeFixture<ObjectElementsFieldClass>();

            var result = fixture.WithBackingField(x => x.Value, (42, "value")).Build();

            Assert.That(result.Value, Is.EqualTo((42, "value")));
        }

        private class WiderElementsFieldClass
        {
            private readonly (long, long) _value;
            public (int, int) Value => ((int, int))_value;
        }

        [Test]
        public void ImplicitConversionAllowed_WiderElementsField_SetsWidenedTuple()
        {
            var options = new FixtureOptions { AllowImplicitConversion = true };
            var factory = new FixtureFactory(options);
            var fixture = TestHelper.MakeFixture<WiderElementsFieldClass>(factory);

            var result = fixture.WithBackingField(x => x.Value, (1, 2)).Build();

            Assert.That(result.Value, Is.EqualTo((1, 2)));
        }

        [Test]
        public void ImplicitConversionNotAllowed_WiderElementsField_ThrowsInvalidCastException()
        {
            var fixture = TestHelper.MakeFixture<WiderElementsFieldClass>();

            Assert.Throws<InvalidCastException>(() => fixture.WithBackingField(x => x.Value, (1, 2)));
        }

        [Test]
        public void WiderElementsField_ConverterForElementType_SetsConvertedTuple()
        {
            var factory = new FixtureFactory().AddConverter(new FixedLongConverter(456));
            var fixture = TestHelper.MakeFixture<WiderElementsFieldClass>(factory);

            var result = fixture.WithBackingField(x => x.Value, (1, 2)).Build();

            Assert.That(result.Value, Is.EqualTo((456, 456)));
        }

        #endregion

        #region Tuple kinds

        private class NullableObjectElementsFieldClass
        {
            private readonly (object, object)? _value;
            public (int, string) Value => ((int, string))_value!.Value;
        }

        [Test]
        public void NullableTupleField_SetsConvertedTuple()
        {
            var fixture = TestHelper.MakeFixture<NullableObjectElementsFieldClass>();

            var result = fixture.WithBackingField(x => x.Value, (42, "value")).Build();

            Assert.That(result.Value, Is.EqualTo((42, "value")));
        }

        private class ReferenceTupleObjectElementsFieldClass
        {
            private readonly Tuple<object, object> _value = null!;
            public Tuple<int, string> Value => Tuple.Create((int)_value.Item1, (string)_value.Item2);
        }

        [Test]
        public void ReferenceTupleField_SetsConvertedTuple()
        {
            var fixture = TestHelper.MakeFixture<ReferenceTupleObjectElementsFieldClass>();

            var result = fixture.WithBackingField(x => x.Value, Tuple.Create(42, "value")).Build();

            Assert.That(result.Value, Is.EqualTo(Tuple.Create(42, "value")));
        }

        #endregion

        #region Element nullability

        [Test]
        public void ObjectElementsField_NullElement_SetsConvertedTuple()
        {
            var fixture = TestHelper.MakeFixture<ObjectElementsFieldClass>();

            var result = fixture.WithBackingField(x => x.Value, (42, null)).Build();

            Assert.That(result.Value, Is.EqualTo((42, (string?)null)));
        }

        private class NonNullableValueElementsFieldClass
        {
            private readonly (int, int) _value;
            public (int?, int?) Value => _value;
        }

        [Test]
        public void NonNullableValueElementsField_NullElement_ThrowsInvalidCastException()
        {
            var fixture = TestHelper.MakeFixture<NonNullableValueElementsFieldClass>();

            Assert.Throws<InvalidCastException>(() => fixture.WithBackingField(x => x.Value, (1, null)));
        }

        #endregion
    }
}
