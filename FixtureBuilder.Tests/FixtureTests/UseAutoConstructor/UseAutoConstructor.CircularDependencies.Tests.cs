using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureTests.UseAutoConstructor
{
    internal sealed class UseAutoConstructorCircularDependenciesTests
    {
        private class Outer(Inner inner, string name)
        {
            public Inner Inner { get; } = inner;
            public string Name { get; } = name;
        }

        private class Inner(Outer outer, string name)
        {
            public Outer Outer { get; } = outer;
            public string Name { get; } = name;
        }

        [Test]
        public void NotAllowed_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<Outer>();

            Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());
        }

        [Test]
        public void Allowed_ResolvesCycleWithShell()
        {
            var options = new FixtureOptions { AllowResolveCircularDependencies = true };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<Outer>();

            var result = fixture.UseAutoConstructor().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("name"));
                Assert.That(result.Inner.Name, Is.EqualTo("name"));
                Assert.That(result.Inner.Outer.Name, Is.EqualTo("name"));
                Assert.That(result.Inner.Outer.Inner, Is.SameAs(result.Inner));
            }
        }

        private class First(Second second)
        {
            public Second Second { get; } = second;
        }

        private class Second(Third third)
        {
            public Third Third { get; } = third;
        }

        private class Third(First first)
        {
            public First First { get; } = first;
        }

        [Test]
        public void Allowed_CycleThroughThreeTypes_ResolvesCycleWithShell()
        {
            var options = new FixtureOptions { AllowResolveCircularDependencies = true };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<First>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Second.Third.First.Second, Is.SameAs(result.Second));
        }

        private class BranchRoot(LeftBranch left, RightBranch right)
        {
            public LeftBranch Left { get; } = left;
            public RightBranch Right { get; } = right;
        }

        private class LeftBranch(BranchRoot root)
        {
            public BranchRoot Root { get; } = root;
        }

        private class RightBranch(BranchRoot root)
        {
            public BranchRoot Root { get; } = root;
        }

        [Test]
        public void Allowed_TwoBranchesCyclingToSameType_ResolvesBothWithShells()
        {
            var options = new FixtureOptions { AllowResolveCircularDependencies = true };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<BranchRoot>();

            var result = fixture.UseAutoConstructor().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Left.Root.Right, Is.SameAs(result.Right));
                Assert.That(result.Right.Root.Left, Is.SameAs(result.Left));
            }
        }

        private class NamedBaseClass(string name)
        {
            private readonly string _name = name;
            public string Name => _name;
        }

        private class DerivedOuter(DerivedInner inner, string name) : NamedBaseClass(name)
        {
            public DerivedInner Inner { get; } = inner;
        }

        private class DerivedInner(DerivedOuter outer)
        {
            public DerivedOuter Outer { get; } = outer;
        }

        [Test]
        public void Allowed_ShellReceivesBaseClassPrivateFields()
        {
            var options = new FixtureOptions { AllowResolveCircularDependencies = true };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<DerivedOuter>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Inner.Outer.Name, Is.EqualTo("name"));
        }

        private class TwinsClass(Twin first, Twin second)
        {
            public Twin First { get; } = first;
            public Twin Second { get; } = second;
        }

        private class Twin(TwinPartner partner)
        {
            public TwinPartner Partner { get; } = partner;
        }

        private class TwinPartner(Twin twin)
        {
            public Twin Twin { get; } = twin;
        }

        [Test]
        public void Allowed_SiblingsCyclingToSameType_EachShellMirrorsItsOwnInstance()
        {
            var options = new FixtureOptions { AllowResolveCircularDependencies = true };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<TwinsClass>();

            var result = fixture.UseAutoConstructor().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.First.Partner.Twin.Partner, Is.SameAs(result.First.Partner));
                Assert.That(result.Second.Partner.Twin.Partner, Is.SameAs(result.Second.Partner));
            }
        }

        private class HashedOuter(HashedMiddle middle, string id)
        {
            public HashedMiddle Middle { get; } = middle;
            public string Id { get; } = id;

            public override int GetHashCode() => Id.GetHashCode();
        }

        private class HashedMiddle(HashedOuter outer, HashedInner inner)
        {
            public HashedOuter Outer { get; } = outer;
            public HashedInner Inner { get; } = inner;
        }

        private class HashedInner(HashedMiddle middle)
        {
            public HashedMiddle Middle { get; } = middle;
        }

        [Test]
        public void Allowed_GetHashCodeOverrideReadingUninitializedMember_ResolvesCycle()
        {
            var options = new FixtureOptions { AllowResolveCircularDependencies = true };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<HashedOuter>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Id, Is.EqualTo("id"));
        }
    }
}
