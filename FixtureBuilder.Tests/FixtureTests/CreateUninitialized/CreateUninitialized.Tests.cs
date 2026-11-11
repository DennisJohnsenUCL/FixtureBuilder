using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureTests.CreateUninitialized
{
    internal sealed class CreateUninitializedTests
    {
        private class NullableAndNonNullableClass
        {
            public string NonNullable { get; set; } = null!;
            public string? Nullable { get; set; }
        }

        [Test]
        public void BypassesConstructor()
        {
            var fixture = Fixture.New<ConstructorSetsValueClass>();

            var result = fixture.CreateUninitialized(InitializeMembers.None).Build();

            Assert.That(result.Value, Is.Null);
        }

        [Test]
        public void None_InitializesNoMembers()
        {
            var fixture = Fixture.New<NullableAndNonNullableClass>();

            var result = fixture.CreateUninitialized(InitializeMembers.None).Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.NonNullable, Is.Null);
                Assert.That(result.Nullable, Is.Null);
            }
        }

        [Test]
        public void All_InitializesAllMembers()
        {
            var fixture = Fixture.New<NullableAndNonNullableClass>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.NonNullable, Is.EqualTo("NonNullable"));
                Assert.That(result.Nullable, Is.EqualTo("Nullable"));
            }
        }

        [Test]
        public void NonNullables_InitializesOnlyNonNullableMembers()
        {
            var fixture = Fixture.New<NullableAndNonNullableClass>();

            var result = fixture.CreateUninitialized(InitializeMembers.NonNullables).Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.NonNullable, Is.EqualTo("NonNullable"));
                Assert.That(result.Nullable, Is.Null);
            }
        }

        [Test]
        public void NoArgument_DefaultOptions_InitializesNoMembers()
        {
            var fixture = Fixture.New<NullableAndNonNullableClass>();

            var result = fixture.CreateUninitialized().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.NonNullable, Is.Null);
                Assert.That(result.Nullable, Is.Null);
            }
        }

        [Test]
        public void NoArgument_DefaultInitializeMembersAll_InitializesAllMembers()
        {
            var options = new FixtureOptions { DefaultInitializeMembers = InitializeMembers.All };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<NullableAndNonNullableClass>();

            var result = fixture.CreateUninitialized().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.NonNullable, Is.EqualTo("NonNullable"));
                Assert.That(result.Nullable, Is.EqualTo("Nullable"));
            }
        }

        [Test]
        public void NoArgument_DefaultInitializeMembersNonNullables_InitializesOnlyNonNullableMembers()
        {
            var options = new FixtureOptions { DefaultInitializeMembers = InitializeMembers.NonNullables };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<NullableAndNonNullableClass>();

            var result = fixture.CreateUninitialized().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.NonNullable, Is.EqualTo("NonNullable"));
                Assert.That(result.Nullable, Is.Null);
            }
        }

        [Test]
        public void UninitializableMember_DefaultOptions_LeavesMemberNull()
        {
            var fixture = Fixture.New<PropertyHolder<IMarkerInterface>>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.Null);
        }

        [Test]
        public void InterfaceMember_SkippingNotAllowed_ThrowsInvalidOperationException()
        {
            var options = new FixtureOptions { AllowSkipUninitializableMembers = false };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<PropertyHolder<IMarkerInterface>>();

            Assert.Throws<InvalidOperationException>(() => fixture.CreateUninitialized(InitializeMembers.All));
        }

        [Test]
        public void AbstractMember_SkippingNotAllowed_ThrowsInvalidOperationException()
        {
            var options = new FixtureOptions { AllowSkipUninitializableMembers = false };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<PropertyHolder<AbstractClass>>();

            Assert.Throws<InvalidOperationException>(() => fixture.CreateUninitialized(InitializeMembers.All));
        }

        [Test]
        public void TypeThatCannotBeCreatedUninitialized_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<string>();

            Assert.Throws<InvalidOperationException>(() => fixture.CreateUninitialized(InitializeMembers.None));
        }

        [Test]
        public void AlreadyConstructed_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<NullableAndNonNullableClass>();
            fixture.CreateUninitialized(InitializeMembers.None);

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.CreateUninitialized(InitializeMembers.None));

            Assert.That(ex.Message, Does.Contain("already constructed"));
        }
    }
}
