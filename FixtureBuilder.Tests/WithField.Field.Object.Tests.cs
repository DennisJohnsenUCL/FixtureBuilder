namespace FixtureBuilder.Tests
{
    internal sealed partial class WithFieldTests
    {
        [Test]
        public void ClassPrivateField_SetsField()
        {
            var fieldName = "_text";

            var fixture = Fixture.New<ExplicitBackingFieldClass>().BypassConstructor().WithField(fieldName, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
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
    }
}
