namespace FixtureBuilder.Tests
{
    internal class NewTests
    {
        [Test]
        public void PreBuiltFixture_SetsAndBuildsFixture()
        {
            var text = "Test";
            var number = 123;

            var preBuiltFixture = new TestClass() { Text = text };

            var fixture = FixtureBuilder.New(preBuiltFixture).WithField(p => p.Number, number).Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(fixture.Text, Is.EqualTo(text));
                Assert.That(fixture.Number, Is.EqualTo(number));
            }
        }

        [Test]
        public void ReadOnlyFieldNotSet_HasDefaultValue()
        {
            var number = 123;

            var fixture = FixtureBuilder.New<TestClass>().WithField(p => p.Number, number).Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(fixture.PrivateExplicitField, Is.Not.Null);
                Assert.That(fixture.PrivateExplicitField, Is.EqualTo(""));
                Assert.That(fixture.Number, Is.EqualTo(number));
            }
        }

        [Test]
        public void GenericTypeInterface_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => FixtureBuilder.New<INestedInterface>());
        }
    }
}
