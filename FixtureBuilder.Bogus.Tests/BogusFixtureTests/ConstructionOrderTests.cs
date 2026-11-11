namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal sealed class ConstructionOrderTests
    {
        #region Construction method after construction method

        [Test]
        public void UseConstructorWithFaker_AlreadyConstructed_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.WithBogus<ConstructorSetsValueClass>();
            fixture.UseAutoConstructor();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseConstructor(_ => []));

            Assert.That(ex.Message, Does.Contain("already constructed"));
        }

        [Test]
        public void UseCustomInstantiatorWithFaker_AlreadyConstructed_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.WithBogus<ConstructorSetsValueClass>();
            fixture.UseAutoConstructor();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseCustomInstantiator(_ => new ConstructorSetsValueClass()));

            Assert.That(ex.Message, Does.Contain("already constructed"));
        }

        [Test]
        public void CreateUninitialized_AlreadyConstructed_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.WithBogus<ConstructorSetsValueClass>();
            fixture.UseAutoConstructor();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.CreateUninitialized());

            Assert.That(ex.Message, Does.Contain("already constructed"));
        }

        [Test]
        public void CreateUninitializedWithMembers_AlreadyConstructed_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.WithBogus<ConstructorSetsValueClass>();
            fixture.UseAutoConstructor();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.CreateUninitialized(InitializeMembers.None));

            Assert.That(ex.Message, Does.Contain("already constructed"));
        }

        [Test]
        public void UseConstructor_AlreadyConstructed_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.WithBogus<ConstructorSetsValueClass>();
            fixture.UseAutoConstructor();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseConstructor());

            Assert.That(ex.Message, Does.Contain("already constructed"));
        }

        [Test]
        public void UseAutoConstructor_AlreadyConstructed_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.WithBogus<ConstructorSetsValueClass>();
            fixture.UseAutoConstructor();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());

            Assert.That(ex.Message, Does.Contain("already constructed"));
        }

        [Test]
        public void UseCustomInstantiator_AlreadyConstructed_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.WithBogus<ConstructorSetsValueClass>();
            fixture.UseAutoConstructor();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseCustomInstantiator(() => new ConstructorSetsValueClass()));

            Assert.That(ex.Message, Does.Contain("already constructed"));
        }

        #endregion

        #region Construction method after other calls

        [Test]
        public void ConstructionMethod_AfterConfigurationMethod_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.WithBogus<ConstructorSetsValueClass>();
            fixture.With(x => x.Value, "value");

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());

            Assert.That(ex.Message, Does.Contain("already constructed"));
        }

        [Test]
        public void ConstructionMethod_AfterBuild_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.WithBogus<ConstructorSetsValueClass>();
            fixture.Build();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());

            Assert.That(ex.Message, Does.Contain("already constructed"));
        }

        [Test]
        public void ConstructionMethod_AfterCastTo_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.WithBogus<MarkerImplementationClass>();
            fixture.CastTo<IMarkerInterface>();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());

            Assert.That(ex.Message, Does.Contain("already constructed"));
        }

        private class ThrowingConstructorClass
        {
            public string Value { get; } = "initializer";

            public ThrowingConstructorClass() => throw new FormatException();
        }

        [Test]
        public void ConstructionMethod_AfterFailedBuild_ConstructsFixture()
        {
            var fixture = Fixture.WithBogus<ThrowingConstructorClass>();
            Assert.Throws<InvalidOperationException>(() => fixture.Build());

            var result = fixture.CreateUninitialized().Build();

            Assert.That(result.Value, Is.Null);
        }

        #endregion
    }
}
