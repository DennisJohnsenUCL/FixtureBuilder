namespace FixtureBuilder.Tests.FixtureTests.WithField
{
    internal sealed class WithFieldTests
    {
        private readonly static string _text = "Test string";
        private readonly static int _number = 123;

        [Test]
        public void ClassPrivateField_SetsField()
        {
            var fieldName = "_text";

            var fixture = Fixture.New<ExplicitBackingFieldClass>().BypassConstructor().WithField(fieldName, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
        }

        [Test]
        public void NullValue_SetsField()
        {
            var fieldName = "_text";

            var fixture = Fixture.New<ExplicitBackingFieldClass>().BypassConstructor().WithField<string>(fieldName, null!);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.Null);
        }

        [Test]
        public void IncorrectFieldName_ThrowsException()
        {
            var fieldName = "_notAField";

            Assert.Throws<InvalidOperationException>(() => Fixture.New<ExplicitBackingFieldClass>().BypassConstructor().WithField(fieldName, _text));
        }

        [Test]
        public void IncorrectFieldType_ThrowsException()
        {
            var fieldName = "_text";

            Assert.Throws<InvalidOperationException>(() => Fixture.New<ExplicitBackingFieldClass>().BypassConstructor().WithField(fieldName, _number));
        }

        [Test]
        public void InheritedProtectedField_SetsField()
        {
            var fieldName = "_inheritedField";

            var fixture = Fixture.New<DerivedTestClass>().BypassConstructor().WithField(fieldName, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.InheritedFieldGetter, Is.EqualTo(_text));
        }

        record FieldRecord
        {
            private readonly string _text = null!;
            public string Text => _text;
        }
        [Test]
        public void RecordField_SetsField()
        {
            var text = "Test string";

            var fixture = Fixture.New<FieldRecord>().WithField("_text", text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(text));
        }

        [Test]
        public void Field_ConstructionNotChosen_InstantiatesNonNullables()
        {
            var fixture = Fixture.New<ClassWithNullable>().WithField("_text", "test");
            var field = Helpers.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.NullableClass, Is.Null);
                Assert.That(field.NonNullableClass, Is.Not.Null);
            }
        }

        class StringListClass
        {
            private readonly List<string> _stringList = null!;
            public List<string> StringList => _stringList;
        }
        [Test]
        public void CollectionTypeField_StringParameter_ThrowsException()
        {
            var fieldName = "_stringList";

            Assert.Throws<InvalidOperationException>(() => Fixture.New<StringListClass>().BypassConstructor().WithField(fieldName, _text));
        }

        [Test]
        public void CollectionTypeField_CollectionParameter_SetsField()
        {
            var fieldName = "_stringList";

            var fixture = Fixture.New<StringListClass>().BypassConstructor().WithField<List<string>>(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.StringList[0], Is.EqualTo(_text));
        }

        [Test]
        public void CollectionTypeField_CollectionParameters_SetsField()
        {
            var fieldName = "_stringList";
            var secondEntry = "More test";

            var fixture = Fixture.New<StringListClass>().BypassConstructor().WithField<List<string>>(fieldName, [_text, secondEntry]);
            var field = Helpers.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.StringList[0], Is.EqualTo(_text));
                Assert.That(field.StringList[1], Is.EqualTo(secondEntry));
            }
        }
    }
}
