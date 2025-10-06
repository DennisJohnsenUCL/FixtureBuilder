namespace TestUtilities.Tests.FixtureBuilderTests
{
    internal sealed class WithFieldUnsafeTests
    {
        [Test]
        public void ClassPrivateField_SetsField()
        {
            var text = "New string name";
            var fieldName = "_privateField";

            var fixture = new FixtureBuilder<TestClassObject>().WithFieldUnsafe(fieldName, text).Build();

            Assert.That(fixture.PrivateField, Is.EqualTo(text));
        }

        [Test]
        public void IncorrectFieldName_ThrowsException()
        {
            var text = "New string name";
            var fieldName = "_notAField";

            Assert.Throws<InvalidOperationException>(() => new FixtureBuilder<TestClassObject>().WithFieldUnsafe(fieldName, text).Build());
        }

        [Test]
        public void IncorrectFieldType_ThrowsException()
        {
            var number = 123;
            var fieldName = "_privateField";

            Assert.Throws<ArgumentException>(() => new FixtureBuilder<TestClassObject>().WithFieldUnsafe(fieldName, number).Build());
        }
    }
}
