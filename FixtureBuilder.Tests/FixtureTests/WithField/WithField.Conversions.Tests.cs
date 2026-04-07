#pragma warning disable CS0649

namespace FixtureBuilder.Tests.FixtureTests.WithField
{
    internal class WithFieldConversionsTests
    {
        private readonly static string _text = "Test string";
        private readonly static int _number = 123;

        [Test]
        public void NotNullableField_NullableReferenceTypeValue_SetsField()
        {
            var fieldName = "_text";
            string? text = "Test string";
            var fixture = TestHelper.MakeFixture<ExplicitBackingFieldClass>();

            fixture.WithField(fieldName, text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.Text, Is.EqualTo(text));
        }

        [Test]
        public void NotNullableField_NullableReferenceTypeValue_SetsNull()
        {
            var fieldName = "_text";
            var fixture = TestHelper.MakeFixture<ExplicitBackingFieldClass>();

            fixture.WithField<string?>(fieldName, null);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.Text, Is.Null);
        }

        class NullableReferenceTypeClass
        {
            public string? NullableString;
        }
        [Test]
        public void NullableReferenceTypeField_NullableReferenceTypeValue_SetsField()
        {
            var fieldName = "NullableString";
            string? text = "Test string";
            var fixture = TestHelper.MakeFixture<NullableReferenceTypeClass>();

            fixture.WithField(fieldName, text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NullableString, Is.EqualTo(text));
        }

        [Test]
        public void NullableReferenceTypeField_NullableReferenceTypeValue_SetsNull()
        {
            var fieldName = "NullableString";
            var fixture = TestHelper.MakeFixture<NullableReferenceTypeClass>();

            fixture.WithField<string?>(fieldName, null);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NullableString, Is.Null);
        }

        [Test]
        public void NullableReferenceTypeField_NonNullableReferenceTypeValue_SetsField()
        {
            var fieldName = "NullableString";
            var fixture = TestHelper.MakeFixture<NullableReferenceTypeClass>();

            fixture.WithField(fieldName, _text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NullableString, Is.EqualTo(_text));
        }

        class ValueTypesClass
        {
            public int NonNullableInt;
            public int? NullableInt;
        }
        [Test]
        public void NonNullableValueTypeField_NonNullableValueTypeValue_SetsField()
        {
            var fieldName = "NonNullableInt";
            var fixture = TestHelper.MakeFixture<ValueTypesClass>();

            fixture.WithField(fieldName, _number);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NonNullableInt, Is.EqualTo(_number));
        }

        [Test]
        public void NonNullableValueTypeField_NullableValueTypeValue_SetsField()
        {
            var fieldName = "NonNullableInt";
            int? number = 123;
            var fixture = TestHelper.MakeFixture<ValueTypesClass>();

            fixture.WithField(fieldName, number);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NonNullableInt, Is.EqualTo(number));
        }

        [Test]
        public void NullableValueTypeField_NullableValueTypeValue_SetsField()
        {
            var fieldName = "NullableInt";
            int? number = 123;
            var fixture = TestHelper.MakeFixture<ValueTypesClass>();

            fixture.WithField(fieldName, number);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NullableInt, Is.EqualTo(number));
        }

        [Test]
        public void NullableValueTypeField_NullableValueTypeValue_SetsNull()
        {
            var fieldName = "NullableInt";
            var fixture = TestHelper.MakeFixture<ValueTypesClass>();

            fixture.WithField<int?>(fieldName, null);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NullableInt, Is.Null);
        }

        [Test]
        public void NonNullableValueTypeField_NullableValueTypeValue_()
        {
            var fieldName = "NonNullableInt";
            var fixture = TestHelper.MakeFixture<ValueTypesClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithField<int?>(fieldName, null));
        }

        [Test]
        public void NullableValueTypeField_NonNullableValueTypeValue_SetsField()
        {
            var fieldName = "NullableInt";
            var fixture = TestHelper.MakeFixture<ValueTypesClass>();

            fixture.WithField(fieldName, _number);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NullableInt, Is.EqualTo(_number));
        }
    }
}
