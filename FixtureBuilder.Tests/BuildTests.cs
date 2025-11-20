namespace FixtureBuilder.Tests
{
    internal sealed class BuildTests
    {
        [Test]
        public void ClassWithMembers_InstantiatesClassMembers()
        {
            var fixture = Fixture.New<TestClass>().Build();

            Assert.That(fixture.NestedClass, Is.Not.Null);
        }
    }
}
