using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureTests.CreateUninitialized
{
    internal sealed class CreateUninitializedCircularDependenciesTests
    {
        private class Outer
        {
            public Inner Inner { get; set; } = null!;
            public string Name { get; set; } = null!;
        }

        private class Inner
        {
            public Outer Outer { get; set; } = null!;
            public string Name { get; set; } = null!;
        }

        [Test]
        public void NotAllowed_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<Outer>();

            Assert.Throws<InvalidOperationException>(() => fixture.CreateUninitialized(InitializeMembers.NonNullables));
        }

        [Test]
        public void Allowed_ResolvesCycleWithShell()
        {
            var options = new FixtureOptions { AllowResolveCircularDependencies = true };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<Outer>();

            var result = fixture.CreateUninitialized(InitializeMembers.NonNullables).Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("Name"));
                Assert.That(result.Inner.Name, Is.EqualTo("Name"));
                Assert.That(result.Inner.Outer.Name, Is.EqualTo("Name"));
            }
        }

        private class TaggedBaseClass
        {
            private string _tag = null!;
            public string Tag { get => _tag; set => _tag = value; }
        }

        private class DerivedOuter : TaggedBaseClass
        {
            public DerivedInner Inner { get; set; } = null!;
        }

        private class DerivedInner
        {
            public DerivedOuter Outer { get; set; } = null!;
        }

        [Test]
        public void Allowed_ShellReceivesBaseClassPrivateFields()
        {
            var options = new FixtureOptions { AllowResolveCircularDependencies = true };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<DerivedOuter>();

            var result = fixture.CreateUninitialized(InitializeMembers.NonNullables).Build();

            Assert.That(result.Inner.Outer.Tag, Is.EqualTo("Tag"));
        }

        private class HashedOuter
        {
            public HashedMiddle Middle { get; set; } = null!;
            public string Id { get; set; } = null!;

            public override int GetHashCode() => Id.GetHashCode();
        }

        private class HashedMiddle
        {
            public HashedOuter Outer { get; set; } = null!;
            public HashedInner Inner { get; set; } = null!;
        }

        private class HashedInner
        {
            public HashedMiddle Middle { get; set; } = null!;
        }

        [Test]
        public void Allowed_GetHashCodeOverrideReadingUninitializedMember_ResolvesCycle()
        {
            var options = new FixtureOptions { AllowResolveCircularDependencies = true };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<HashedOuter>();

            var result = fixture.CreateUninitialized(InitializeMembers.NonNullables).Build();

            Assert.That(result.Id, Is.EqualTo("Id"));
        }
    }
}
