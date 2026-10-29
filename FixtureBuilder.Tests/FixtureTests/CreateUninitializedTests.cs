#pragma warning disable CA1822 // Mark members as static

namespace FixtureBuilder.Tests.FixtureTests
{
    internal class CreateUninitializedTests
    {
        public class SimpleTestClass
        {
            public string? Name { get; set; }
            public int Count { get; set; }
        }

        public class ClassWithNullableAndNonNullable
        {
            public string Required { get; set; } = null!;
            public string? Optional { get; set; }
        }

        [Test]
        public void CreateUninitialized_SimpleClass_DoesNotInitializeMembers()
        {
            var fixture = Fixture.New<SimpleTestClass>();

            fixture.CreateUninitialized();

            var field = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.Name, Is.Null);
                Assert.That(field.Count, Is.Default);
            }
        }

        [Test]
        public void CreateUninitialized_None_LeavesAllMembersDefault()
        {
            var fixture = Fixture.New<SimpleTestClass>();

            fixture.CreateUninitialized(InitializeMembers.None);

            var field = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.Name, Is.Null);
                Assert.That(field.Count, Is.Default);
            }
        }

        [Test]
        public void CreateUninitialized_All_InitializesAllMembers()
        {
            var fixture = Fixture.New<ClassWithNullableAndNonNullable>();

            fixture.CreateUninitialized(InitializeMembers.All);

            var field = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.Required, Is.Not.Null);
                Assert.That(field.Optional, Is.Not.Null);
            }
        }

        [Test]
        public void CreateUninitialized_NonNullables_InitializesNonNullableMembers()
        {
            var fixture = Fixture.New<ClassWithNullableAndNonNullable>();

            fixture.CreateUninitialized(InitializeMembers.NonNullables);

            var field = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.Required, Is.Not.Null);
                Assert.That(field.Optional, Is.Null);
            }
        }

        public class ClassWithConstructorSideEffect
        {
            public bool ConstructorWasCalled { get; }
            public ClassWithConstructorSideEffect()
                => ConstructorWasCalled = true;
        }
        [Test]
        public void CreateUninitialized_BypassesConstructor()
        {
            var fixture = Fixture.New<ClassWithConstructorSideEffect>();

            fixture.CreateUninitialized(InitializeMembers.None);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.ConstructorWasCalled, Is.False);
        }

        [Test]
        public void CreateUninitialized_TypeThatCannotBeCreatedUninitialized_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<string>();

            Assert.Throws<InvalidOperationException>(() => fixture.CreateUninitialized(InitializeMembers.None));
        }

        [Test]
        public void CreateUninitialized_AlreadyConstructed_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<SimpleTestClass>().CreateUninitialized(InitializeMembers.None);

            var ex = Assert.Throws<InvalidOperationException>(() =>
                ((IFixtureConstructor<SimpleTestClass>)fixture).CreateUninitialized(InitializeMembers.None));

            Assert.That(ex!.Message, Does.Contain("already constructed"));
        }

        [Test]
        public void CreateUninitialized_NoParameter_OptionAll_InitializesAll()
        {
            var fixture = Fixture.New<ClassWithNullableAndNonNullable>();
            TestHelper.SetOption(fixture, o => o.DefaultInitializeMembers = InitializeMembers.All);

            fixture.CreateUninitialized();

            var field = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.Required, Is.Not.Null);
                Assert.That(field.Optional, Is.Not.Null);
            }
        }

        [Test]
        public void CreateUninitialized_NoParameter_OptionNonNullables_InitializesNonNullables()
        {
            var fixture = Fixture.New<ClassWithNullableAndNonNullable>();
            TestHelper.SetOption(fixture, o => o.DefaultInitializeMembers = InitializeMembers.NonNullables);

            fixture.CreateUninitialized();

            var field = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.Required, Is.Not.Null);
                Assert.That(field.Optional, Is.Null);
            }
        }

        [Test]
        public void CreateUninitialized_NoParameter_OptionNone_InitializesNone()
        {
            var fixture = Fixture.New<ClassWithNullableAndNonNullable>();
            TestHelper.SetOption(fixture, o => o.DefaultInitializeMembers = InitializeMembers.None);

            fixture.CreateUninitialized();

            var field = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.Required, Is.Null);
                Assert.That(field.Optional, Is.Null);
            }
        }

        interface IDependency;
        class WithInterfaceMember
        {
            public IDependency Dependency { get; set; } = null!;
        }

        [Test]
        public void CreateUninitialized_AllowSkipUninitializableMembersFalse_InterfaceMember_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<WithInterfaceMember>();

            TestHelper.SetOption(fixture, o => o.AllowSkipUninitializableMembers = false);

            Assert.Throws<InvalidOperationException>(() => fixture.CreateUninitialized(InitializeMembers.All));
        }

        abstract class AbstractDependency;
        class WithAbstractMember
        {
            public AbstractDependency Dependency { get; set; } = null!;
        }

        [Test]
        public void CreateUninitialized_AllowSkipUninitializableMembersFalse_AbstractMember_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<WithAbstractMember>();

            TestHelper.SetOption(fixture, o => o.AllowSkipUninitializableMembers = false);

            Assert.Throws<InvalidOperationException>(() => fixture.CreateUninitialized(InitializeMembers.All));
        }

        class Address
        {
            public string Street { get; set; } = null!;
        }
        class Order
        {
            public Address Billing { get; set; } = null!;
            public Address Shipping { get; set; } = null!;
        }

        [Test]
        public void CreateUninitialized_SiblingMembersOfSameType_InitializesBoth()
        {
            var fixture = Fixture.New<Order>();

            fixture.CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Billing.Street, Is.EqualTo("Street"));
                Assert.That(result.Shipping.Street, Is.EqualTo("Street"));
            }
        }

        class ImmutableCustomer
        {
            public string Name { get; } = null!;
            public Address Address { get; } = null!;
            public string Note { get; set; } = null!;
        }

        [Test]
        public void CreateUninitialized_GetOnlyAutoProperties_AreInitializedThroughBackingFields()
        {
            var fixture = Fixture.New<ImmutableCustomer>();

            fixture.CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("Name"));
                Assert.That(result.Address.Street, Is.EqualTo("Street"));
                Assert.That(result.Note, Is.EqualTo("Note"));
            }
        }

        class EntityBase
        {
            private readonly string _createdBy = null!;
            public string GetCreatedBy() => _createdBy;
        }
        class Product : EntityBase
        {
            public string Name { get; set; } = null!;
        }

        [Test]
        public void CreateUninitialized_InheritedPrivateField_IsInitialized()
        {
            var fixture = Fixture.New<Product>();

            fixture.CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("Name"));
                Assert.That(result.GetCreatedBy(), Is.EqualTo("_createdBy"));
            }
        }

        class AddressView
        {
            private readonly Address _address = null!;
            public string Street { get => _address.Street; set => _address.Street = value; }
        }

        [Test]
        public void CreateUninitialized_PropertyGetterReadingUninitializedField_InitializesField()
        {
            var fixture = Fixture.New<AddressView>();

            fixture.CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Street, Is.EqualTo("Street"));
        }

        class InvoiceHolder
        {
            public SystemUnderTest.Invoice Invoice { get; set; } = null!;
        }

        [Test]
        public void CreateUninitialized_MemberTypeInNamespaceStartingWithSystem_IsCreatedUninitializedWithMembers()
        {
            var fixture = Fixture.New<InvoiceHolder>();

            fixture.CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Invoice.CreatedBy, Is.Not.EqualTo("constructor"));
                Assert.That(result.Invoice.Number, Is.EqualTo("Number"));
            }
        }

#pragma warning disable IDE0250 // Make struct 'readonly'
        record struct SettingsWithDefaults()
#pragma warning restore IDE0250 // Make struct 'readonly'
        {
            public string Name { get; init; } = "default";
        }
        class WithStructWithDefaults
        {
            public SettingsWithDefaults Settings { get; set; }
        }

        [Test]
        public void CreateUninitialized_StructMemberWithParameterlessConstructor_InitializesMembers()
        {
            var fixture = Fixture.New<WithStructWithDefaults>();

            fixture.CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Settings.Name, Is.EqualTo("Name"));
        }

        struct ThrowingConstructorStruct
        {
            public string Value { get; set; }
            public ThrowingConstructorStruct() => throw new InvalidOperationException();
        }
        class WithThrowingConstructorStruct
        {
            public ThrowingConstructorStruct Member { get; set; }
        }

        [Test]
        public void CreateUninitialized_StructMemberWithThrowingParameterlessConstructor_InitializesMembers()
        {
            var fixture = Fixture.New<WithThrowingConstructorStruct>();

            fixture.CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Member.Value, Is.EqualTo("Value"));
        }

        struct ThrowingEqualsStruct
        {
            public string Value { get; set; }
            public override readonly bool Equals(object? obj) => throw new InvalidOperationException();
            public override readonly int GetHashCode() => 0;
        }
        class WithThrowingEqualsStruct
        {
            public ThrowingEqualsStruct Member { get; set; }
        }

        [Test]
        public void CreateUninitialized_StructMemberWithThrowingEquals_InitializesMembers()
        {
            var fixture = Fixture.New<WithThrowingEqualsStruct>();

            fixture.CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Member.Value, Is.EqualTo("Value"));
        }

        class WithNullableValueTypes
        {
            public int? Count { get; set; }
            public SettingsWithDefaults? Settings { get; set; }
        }

        [Test]
        public void CreateUninitialized_All_InitializesNullableValueTypeMembers()
        {
            var fixture = Fixture.New<WithNullableValueTypes>();

            fixture.CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Count, Is.Not.Null);
                Assert.That(result.Settings?.Name, Is.EqualTo("Name"));
            }
        }

        class WithThrowingGetter
        {
            public string Broken { get => throw new InvalidOperationException(); set { } }
            public string Name { get; set; } = null!;
        }

        [Test]
        public void CreateUninitialized_PropertyWithThrowingGetter_InitializesOtherMembers()
        {
            var fixture = Fixture.New<WithThrowingGetter>();

            fixture.CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("Name"));
        }

        class WithThrowingSetter
        {
            public string Broken { get => null!; set => throw new InvalidOperationException(); }
            public string Name { get; set; } = null!;
        }

        [Test]
        public void CreateUninitialized_PropertyWithThrowingSetter_InitializesOtherMembers()
        {
            var fixture = Fixture.New<WithThrowingSetter>();

            fixture.CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("Name"));
        }

        class WithRefStructProperty
        {
            private int[] _values = [];
            public Span<int> Values { get => _values; set => _values = value.ToArray(); }
            public string Name { get; set; } = null!;
        }

        [Test]
        public void CreateUninitialized_RefStructProperty_InitializesOtherMembers()
        {
            var fixture = Fixture.New<WithRefStructProperty>();

            fixture.CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("Name"));
        }

        class OuterAndInner
        {
            public Middle Middle { get; set; } = null!;
            public string Name { get; set; } = null!;
        }
        class Middle
        {
            public OuterAndInner OuterAndInner { get; set; } = null!;
            public string Name { get; set; } = null!;
        }

        [Test]
        public void CircularDependency_InitializeNonNullables_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                Fixture.New<OuterAndInner>().CreateUninitialized(InitializeMembers.NonNullables));
        }

        [Test]
        public void CircularDependency_InitializeNonNullables_WithAllowResolve_ResolvesWithShell()
        {
            var fixture = Fixture.New<OuterAndInner>();

            TestHelper.SetOption(fixture, o => o.AllowResolveCircularDependencies = true);
            fixture.CreateUninitialized(InitializeMembers.NonNullables);
            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("Name"));
                Assert.That(result.Middle, Is.Not.Null);
                Assert.That(result.Middle.Name, Is.EqualTo("Name"));
                Assert.That(result.Middle.OuterAndInner, Is.Not.Null);
                Assert.That(result.Middle.OuterAndInner.Name, Is.EqualTo("Name"));
            }
        }

        class TaggedBase
        {
            private string _tag = null!;
            public string Tag { get => _tag; set => _tag = value; }
        }
        class DerivedOuter : TaggedBase
        {
            public DerivedInner Inner { get; set; } = null!;
        }
        class DerivedInner
        {
            public DerivedOuter Outer { get; set; } = null!;
        }

        [Test]
        public void CircularDependency_WithAllowResolve_ShellReceivesBaseClassPrivateFields()
        {
            var fixture = Fixture.New<DerivedOuter>();

            TestHelper.SetOption(fixture, o => o.AllowResolveCircularDependencies = true);
            fixture.CreateUninitialized(InitializeMembers.NonNullables);
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Inner.Outer.Tag, Is.EqualTo("Tag"));
        }

        class HashedOuter
        {
            public HashedMiddle Middle { get; set; } = null!;
            public string Id { get; set; } = null!;
            public override int GetHashCode() => Id.GetHashCode();
        }
        class HashedMiddle
        {
            public HashedOuter Outer { get; set; } = null!;
            public HashedInner Inner { get; set; } = null!;
        }
        class HashedInner
        {
            public HashedMiddle Middle { get; set; } = null!;
        }

        [Test]
        public void CircularDependency_WithAllowResolve_GetHashCodeOverride_DoesNotThrow()
        {
            var fixture = Fixture.New<HashedOuter>();

            TestHelper.SetOption(fixture, o => o.AllowResolveCircularDependencies = true);

            Assert.DoesNotThrow(() => fixture.CreateUninitialized(InitializeMembers.NonNullables));
        }
    }
}
