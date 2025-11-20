namespace FixtureBuilder.Tests
{
    internal sealed class CastToTests
    {
        [Test]
        public void NotImplementedInterface_ThrowsException()
        {
            Assert.Throws<InvalidCastException>(() => Fixture.New<TestClass>().CastTo<INotImplementedInterface>());
        }

        [Test]
        public void ImplementedInterface_CastsToInterface()
        {
            INestedInterface fixture = Fixture.New<TestClass>().CastTo<INestedInterface>().Build();

            Assert.That(fixture, Is.Not.Null);
        }

        [Test]
        public void ImplementedInterface_ExplicitMembersCanBeSet()
        {
            var fixture = Fixture.New<TestClass>()
                .WithSetter(t => t.Text, "Class member string")
                .CastTo<INestedInterface>()
                .WithSetter(t => t.NestedInterfaceClass.Value, "Explicit interface member string")
                .Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(((TestClass)fixture).Text, Is.EqualTo("Class member string"));
                Assert.That(fixture.NestedInterfaceClass.Value, Is.EqualTo("Explicit interface member string"));
            }
        }
    }
}
