namespace FixtureBuilder.Tests.FixtureTests.WithField
{
    internal sealed class WithFieldConversionsTests
    {
        #region Reference type fields

        [Test]
        public void NonNullableReferenceField_NullableTypedValue_SetsField()
        {
            string? value = "value";
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<string>>();

            var result = fixture.WithField("_value", value).Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void NonNullableReferenceField_NullValue_SetsNull()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<string>>();
            fixture.WithField("_value", "value");

            var result = fixture.WithField<string?>("_value", null).Build();

            Assert.That(result.Value, Is.Null);
        }

        private class NullableReferenceFieldClass
        {
            public string? Value;
        }

        [Test]
        public void NullableReferenceField_NonNullableTypedValue_SetsField()
        {
            var fixture = TestHelper.MakeFixture<NullableReferenceFieldClass>();

            var result = fixture.WithField("Value", "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void NullableReferenceField_NullableTypedValue_SetsField()
        {
            string? value = "value";
            var fixture = TestHelper.MakeFixture<NullableReferenceFieldClass>();

            var result = fixture.WithField("Value", value).Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void NullableReferenceField_NullValue_SetsNull()
        {
            var fixture = TestHelper.MakeFixture<NullableReferenceFieldClass>();
            fixture.WithField("Value", "value");

            var result = fixture.WithField<string?>("Value", null).Build();

            Assert.That(result.Value, Is.Null);
        }

        #endregion

        #region Value type fields

        [Test]
        public void NonNullableValueField_NonNullableTypedValue_SetsField()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<int>>();

            var result = fixture.WithField("Value", 123).Build();

            Assert.That(result.Value, Is.EqualTo(123));
        }

        [Test]
        public void NonNullableValueField_NullableTypedValue_SetsField()
        {
            int? value = 123;
            var fixture = TestHelper.MakeFixture<FieldHolder<int>>();

            var result = fixture.WithField("Value", value).Build();

            Assert.That(result.Value, Is.EqualTo(123));
        }

        [Test]
        public void NonNullableValueField_NullValue_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<int>>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithField<int?>("Value", null));
        }

        [Test]
        public void NullableValueField_NonNullableTypedValue_SetsField()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<int?>>();

            var result = fixture.WithField("Value", 123).Build();

            Assert.That(result.Value, Is.EqualTo(123));
        }

        [Test]
        public void NullableValueField_NullableTypedValue_SetsField()
        {
            int? value = 123;
            var fixture = TestHelper.MakeFixture<FieldHolder<int?>>();

            var result = fixture.WithField("Value", value).Build();

            Assert.That(result.Value, Is.EqualTo(123));
        }

        [Test]
        public void NullableValueField_NullValue_SetsNull()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<int?>>();
            fixture.WithField("Value", 123);

            var result = fixture.WithField<int?>("Value", null).Build();

            Assert.That(result.Value, Is.Null);
        }

        #endregion
    }
}
