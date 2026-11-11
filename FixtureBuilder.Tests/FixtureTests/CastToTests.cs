namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class CastToTests
    {
        [Test]
        public void ImplementedInterface_ReturnsFixtureOfSameInstance()
        {
            var instance = new MarkerImplementationClass();
            var fixture = Fixture.New(instance);

            var result = fixture.CastTo<IMarkerInterface>().Build();

            Assert.That(result, Is.SameAs(instance));
        }

        private class UnrelatedClass;

        [Test]
        public void UnrelatedType_ThrowsInvalidCastException()
        {
            var fixture = TestHelper.MakeFixture<MarkerImplementationClass>();

            Assert.Throws<InvalidCastException>(() => fixture.CastTo<UnrelatedClass>());
        }

        private class ExplicitImplementationClass : IValueInterface
        {
            string IValueInterface.Value { get; set; } = null!;
        }

        [Test]
        public void ExplicitInterfaceImplementation_MembersCanBeConfigured()
        {
            var fixture = TestHelper.MakeFixture<ExplicitImplementationClass>();

            var result = fixture.CastTo<IValueInterface>().WithSetter(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void NotConstructed_ConstructsImplicitly()
        {
            var fixture = Fixture.New<MarkerImplementationClass>();

            var result = fixture.CastTo<IMarkerInterface>().Build();

            Assert.That(result, Is.TypeOf<MarkerImplementationClass>());
        }
    }
}
