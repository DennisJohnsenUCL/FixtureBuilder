namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class CastToTests
    {
        class ClassOne;
        class ClassTwo;
        [Test]
        public void NotImplementedInterface_ThrowsException()
        {
            var fixture = TestHelper.MakeFixture<ClassOne>();

            Assert.Throws<InvalidCastException>(() => fixture.CastTo<ClassTwo>());
        }

        interface IInterface;
        class ClassWithInterface : IInterface;
        [Test]
        public void ImplementedInterface_CastsToInterface()
        {
            var fixture = TestHelper.MakeFixture<ClassWithInterface>();

            IFixtureConfigurator<IInterface> castFixture = null!;
            Assert.DoesNotThrow(() => castFixture = fixture.CastTo<IInterface>());
            var field = TestHelper.GetFixture(castFixture);

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
            var fixture = TestHelper.MakeFixture<ExplicitInterfaceClass>();

            var castFixture = fixture.CastTo<IInterfaceWithMember>();
            castFixture.WithSetter(t => t.Text, text);

            var field = TestHelper.GetFixture(castFixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.Text, Is.EqualTo(text));
            }
        }

        [Test]
        public void CastTo_InstantiatesFixture()
        {
            var fixture = TestHelper.MakeFixture<ClassWithInterface>();

            using (Assert.EnterMultipleScope())
            {
                Assert.DoesNotThrow(() => fixture.CastTo<IInterface>());
                Assert.That(TestHelper.GetFixture(fixture), Is.Not.Null);
            }
        }
    }
}
