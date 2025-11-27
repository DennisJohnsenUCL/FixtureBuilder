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

        [Test]
        public void ClassWithOnlyNullable_DoesNotInstantiateNullableMember()
        {
            var fixture = Fixture.New<ClassWithOnlyNullable>().Build();

            Assert.That(fixture.NestedNullableClass, Is.Null);
        }

        [Test]
        public void ClassWithNullable_DoesNotInstantiateNullableMember()
        {
            var fixture = Fixture.New<TestClass>().Build();

            Assert.That(fixture.NullableClass, Is.Null);
        }
    }
}