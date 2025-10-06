namespace TestUtilities.Tests.FixtureBuilderTests
{
    internal sealed class WithTests
    {
        [Test]
        public void RecordProperty_SetsProperty()
        {
            var text = "test";

            var fixture = new FixtureBuilder<TestValueObject>().With(t => t.Text, text).Build();

            Assert.That(fixture.Text, Is.EqualTo(text));
        }

        [Test]
        public void RecordProperties_SetsProperties()
        {
            var text = "test";
            var number = 123;

            var fixture = new FixtureBuilder<TestValueObject>().With(t => t.Text, text).With(t => t.Number, number).Build();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.Text, Is.EqualTo(text));
                Assert.That(fixture.Number, Is.EqualTo(number));
            });
        }

        [Test]
        public void NotARecordProperty_ThrowsException()
        {
            var number = 123;

            Assert.Throws<ArgumentException>(() => new FixtureBuilder<TestValueObject>().With(t => t.GetHashCode(), number).Build());
        }

        [Test]
        public void NoRecordPropertyBackingField_ThrowsException()
        {
            var number = 123;

            Assert.Throws<InvalidOperationException>(() => new FixtureBuilder<TestValueObject>().With(t => t.Text.Length, number).Build());
        }

        [Test]
        public void ClassProperty_SetsProperty()
        {
            var text = "test";

            var fixture = new FixtureBuilder<TestClassObject>().With(t => t.Text, text).Build();

            Assert.That(fixture.Text, Is.EqualTo(text));
        }

        [Test]
        public void ClassProperties_SetsProperties()
        {
            var text = "test";
            var number = 123;

            var fixture = new FixtureBuilder<TestClassObject>().With(t => t.Text, text).With(t => t.Number, number).Build();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.Text, Is.EqualTo(text));
                Assert.That(fixture.Number, Is.EqualTo(number));
            });
        }

        [Test]
        public void NotAClassProperty_ThrowsException()
        {
            var number = 123;

            Assert.Throws<ArgumentException>(() => new FixtureBuilder<TestClassObject>().With(t => t.GetHashCode(), number).Build());
        }

        [Test]
        public void NoClassPropertyBackingField_ThrowsException()
        {
            var number = 123;

            Assert.Throws<InvalidOperationException>(() => new FixtureBuilder<TestClassObject>().With(t => t.Text.Length, number).Build());
        }
    }
}
