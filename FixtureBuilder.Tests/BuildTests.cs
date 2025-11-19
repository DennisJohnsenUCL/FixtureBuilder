namespace FixtureBuilder.Tests
{
    internal sealed class BuildTests
    {
        [Test]
        public void ClassWithMembers_InstantiatesClassMembers()
        {
            var fixture = FixtureBuilder.New<TestClass>().Build();

            Assert.That(fixture.NestedClass, Is.Not.Null);
        }
    }
}
