namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class CastToTests
    {
        class ClassOne;
        class ClassTwo;
        [Test]
        public void NotImplementedInterface_ThrowsException()
        {
            Assert.Throws<InvalidCastException>(() => Fixture.New<ClassOne>().CastTo<ClassTwo>());
        }

        interface IInterface;
        class ClassWithInterface : IInterface;
        [Test]
        public void ImplementedInterface_CastsToInterface()
        {
            IFixtureConfigurator<IInterface> fixture = null!;
            Assert.DoesNotThrow(() => fixture = Fixture.New<ClassWithInterface>().CastTo<IInterface>());
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field, Is.Not.Null);
        }

        interface IInterfaceWithMember
        {
            string Text { get; set; }
        }
        class ExplicitInterfaceClass : IInterfaceWithMember
        {
            string IInterfaceWithMember.Text { get; set; } = null!;
        }
        [Test]
        public void ImplementedInterface_ExplicitMembersCanBeSet()
        {
            var text = "Explicit interface member string";
            var fixture = Fixture.New<ExplicitInterfaceClass>()
                .CastTo<IInterfaceWithMember>()
                .WithSetter(t => t.Text, text);

            var field = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.Text, Is.EqualTo(text));
            }
        }

        [Test]
        [Ignore("Outdated until autoconstructor is default construction method")]
        public void ConstructionNotChosen_InstantiatesNonNullables()
        {
            var fixture = Fixture.New<ClassWithNullable>().CastTo<ClassWithNullable>();
            var field = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.NullableClass, Is.Null);
                Assert.That(field.NonNullableClass, Is.Not.Null);
            }
        }
    }
}
