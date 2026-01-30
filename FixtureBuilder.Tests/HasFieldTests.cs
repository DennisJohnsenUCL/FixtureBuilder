namespace FixtureBuilder.Tests
{
    internal sealed class HasFieldTests
    {
        class FieldClass
        {
            public string _existingField = null!;
        }
        class DerivedFieldClass : FieldClass;

        [Test]
        public void FieldDoesNotExist_ReturnsFalse()
        {
            var fixture = Fixture.New<FieldClass>().BypassConstructor();

            var exists = fixture.HasField("_notExistingField");

            Assert.That(exists, Is.False);
        }

        [Test]
        public void FieldExists_ReturnsTrue()
        {
            var fixture = Fixture.New<FieldClass>().BypassConstructor();

            var exists = fixture.HasField("_existingField");

            Assert.That(exists, Is.True);
        }

        [Test]
        public void DerivedTypeFieldExists_ReturnsTrue()
        {
            var fixture = Fixture.New<DerivedFieldClass>().BypassConstructor();

            var exists = fixture.HasField("_existingField");

            Assert.That(exists, Is.True);
        }
    }
}
