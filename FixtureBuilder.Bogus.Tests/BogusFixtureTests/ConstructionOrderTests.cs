namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal sealed class ConstructionOrderTests
    {
        private interface INamed
        {
            string Name { get; }
        }

        public class SimpleClass : INamed
        {
            public string Name { get; set; } = string.Empty;
        }

        private class ThrowingConstructorClass
        {
            public ThrowingConstructorClass() => throw new InvalidOperationException("Constructor failed.");
        }

        private static TestCaseData ConstructionCase(string name, Func<IBogusFixtureConstructor<SimpleClass>, IBogusFixtureConfigurator<SimpleClass>> construct)
            => new TestCaseData(construct).SetArgDisplayNames(name);

        private static IEnumerable<TestCaseData> ConstructionMethods()
        {
            yield return ConstructionCase("UseConstructor(Faker)", c => c.UseConstructor(_ => []));
            yield return ConstructionCase("UseCustomInstantiator(Faker)", c => c.UseCustomInstantiator(_ => new SimpleClass()));
            yield return ConstructionCase("CreateUninitialized()", c => c.CreateUninitialized());
            yield return ConstructionCase("CreateUninitialized(InitializeMembers)", c => c.CreateUninitialized(InitializeMembers.None));
            yield return ConstructionCase("UseConstructor(args)", c => c.UseConstructor());
            yield return ConstructionCase("UseAutoConstructor()", c => c.UseAutoConstructor());
            yield return ConstructionCase("UseCustomInstantiator()", c => c.UseCustomInstantiator(() => new SimpleClass()));
        }

        [TestCaseSource(nameof(ConstructionMethods))]
        public void ConstructionMethod_AfterConstructionMethod_ThrowsInvalidOperationException(
            Func<IBogusFixtureConstructor<SimpleClass>, IBogusFixtureConfigurator<SimpleClass>> construct)
        {
            var fixture = Fixture.WithBogus<SimpleClass>();
            fixture.UseAutoConstructor();

            var ex = Assert.Throws<InvalidOperationException>(() => construct(fixture));

            Assert.That(ex!.Message, Does.Contain("already constructed"));
        }

        [Test]
        public void ConstructionMethod_AfterConfiguration_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.WithBogus<SimpleClass>();
            fixture.With(x => x.Name, "Configured");

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());

            Assert.That(ex!.Message, Does.Contain("already constructed"));
        }

        [Test]
        public void ConstructionMethod_AfterBuild_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.WithBogus<SimpleClass>();
            fixture.Build();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());

            Assert.That(ex!.Message, Does.Contain("already constructed"));
        }

        [Test]
        public void ConstructionMethod_AfterFailedBuild_IsAllowed()
        {
            var fixture = Fixture.WithBogus<ThrowingConstructorClass>();
            Assert.Catch(() => fixture.Build());

            var result = fixture.CreateUninitialized().Build();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void ConstructionMethod_AfterCastTo_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.WithBogus<SimpleClass>();
            fixture.CastTo<INamed>();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());

            Assert.That(ex!.Message, Does.Contain("already constructed"));
        }
    }
}
