using System.Text;

namespace FixtureBuilder.Tests.FixtureTests.CreateUninitialized
{
    internal sealed class CreateUninitializedMembersTests
    {
        #region Member kinds

        [Test]
        public void Field_InitializesField()
        {
            var fixture = Fixture.New<FieldHolder<string>>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("Value"));
        }

        [Test]
        public void ValueTypeMember_InitializesMember()
        {
            var fixture = Fixture.New<PropertyHolder<int>>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.InRange(1, 10));
        }

        private class SameTypeSiblingsClass
        {
            public PropertyHolder<string> First { get; set; } = null!;
            public PropertyHolder<string> Second { get; set; } = null!;
        }

        [Test]
        public void SiblingMembersOfSameType_InitializesBoth()
        {
            var fixture = Fixture.New<SameTypeSiblingsClass>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.First.Value, Is.EqualTo("Value"));
                Assert.That(result.Second.Value, Is.EqualTo("Value"));
            }
        }

        private class GetOnlyAutoPropertiesClass
        {
            public string Value { get; } = null!;
            public PropertyHolder<string> Nested { get; } = null!;
        }

        [Test]
        public void GetOnlyAutoProperties_InitializesBackingFields()
        {
            var fixture = Fixture.New<GetOnlyAutoPropertiesClass>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Value, Is.EqualTo("Value"));
                Assert.That(result.Nested.Value, Is.EqualTo("Value"));
            }
        }

        private class PrivateFieldBaseClass
        {
            private readonly string _value = null!;
            public string GetValue() => _value;
        }

        private class DerivedFromPrivateFieldBaseClass : PrivateFieldBaseClass;

        [Test]
        public void InheritedPrivateField_InitializesField()
        {
            var fixture = Fixture.New<DerivedFromPrivateFieldBaseClass>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            var value = result.GetValue();
            Assert.That(value, Is.EqualTo("_value"));
        }

        private class FieldBackedPropertyClass
        {
            private readonly PropertyHolder<string> _holder = null!;
            public string Value { get => _holder.Value; set => _holder.Value = value; }
        }

        [Test]
        public void PropertyGetterReadingUninitializedField_InitializesField()
        {
            var fixture = Fixture.New<FieldBackedPropertyClass>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("Value"));
        }

        private class ExplicitBackingFieldClass
        {
            private string _value = null!;
            public string Value { get => _value; set => _value = value; }
        }

        [Test]
        public void FieldSetThroughEarlierInitializedProperty_IsNotOverwritten()
        {
            var fixture = Fixture.New<ExplicitBackingFieldClass>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("Value"));
        }

        [Test]
        public void MemberOfSystemType_CreatesMemberWithParameterlessConstructor()
        {
            var fixture = Fixture.New<PropertyHolder<StringBuilder>>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            var text = result.Value.ToString();
            Assert.That(text, Is.Empty);
        }

        [Test]
        public void MemberTypeInNamespaceStartingWithSystem_CreatesMemberUninitializedWithMembers()
        {
            var fixture = Fixture.New<PropertyHolder<SystemUnderTest.Invoice>>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Value.CreatedBy, Is.EqualTo("CreatedBy"));
                Assert.That(result.Value.Number, Is.EqualTo("Number"));
            }
        }

        private class NullableValueTypesClass
        {
            public int? Number { get; set; }
            public PropertyHolderStruct<string>? Struct { get; set; }
        }

        [Test]
        public void All_InitializesNullableValueTypeMembers()
        {
            var fixture = Fixture.New<NullableValueTypesClass>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Number, Is.InRange(1, 10));
                Assert.That(result.Struct?.Value, Is.EqualTo("Value"));
            }
        }

        [Test]
        public void InterfaceMember_TypeLink_InitializesLinkedType()
        {
            var factory = new FixtureFactory().AddTypeLink<IMarkerInterface, MarkerImplementationClass>();
            var fixture = factory.New<PropertyHolder<IMarkerInterface>>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.TypeOf<MarkerImplementationClass>());
        }

        #endregion

        #region Struct members

        private record struct DefaultConstructorStruct()
        {
            public string Value { get; init; } = "constructor";
        }

        [Test]
        public void StructWithParameterlessConstructor_InitializesStructMembers()
        {
            var fixture = Fixture.New<PropertyHolder<DefaultConstructorStruct>>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value.Value, Is.EqualTo("Value"));
        }

        private struct ThrowingConstructorStruct
        {
            public string Value { get; set; }

            public ThrowingConstructorStruct() => throw new InvalidOperationException();
        }

        [Test]
        public void StructWithThrowingParameterlessConstructor_InitializesStructMembers()
        {
            var fixture = Fixture.New<PropertyHolder<ThrowingConstructorStruct>>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value.Value, Is.EqualTo("Value"));
        }

        private struct ThrowingEqualsStruct
        {
            public string Value { get; set; }

            public override readonly bool Equals(object? obj) => throw new InvalidOperationException();
            public override readonly int GetHashCode() => 0;
        }

        [Test]
        public void StructWithThrowingEquals_InitializesStructMembers()
        {
            var fixture = Fixture.New<PropertyHolder<ThrowingEqualsStruct>>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value.Value, Is.EqualTo("Value"));
        }

        #endregion

        #region Members that cannot be initialized

        private class ThrowingGetterClass
        {
            public string Throwing { get => throw new InvalidOperationException(); set { } }
            public string Value { get; set; } = null!;
        }

        [Test]
        public void PropertyWithThrowingGetter_InitializesOtherMembers()
        {
            var fixture = Fixture.New<ThrowingGetterClass>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("Value"));
        }

        private class ThrowingSetterClass
        {
            public string Throwing { get => null!; set => throw new InvalidOperationException(); }
            public string Value { get; set; } = null!;
        }

        [Test]
        public void PropertyWithThrowingSetter_InitializesOtherMembers()
        {
            var fixture = Fixture.New<ThrowingSetterClass>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("Value"));
        }

        private class RefStructPropertyClass
        {
            private int[] _numbers = [];
            public Span<int> Numbers { get => _numbers; set => _numbers = value.ToArray(); }
            public string Value { get; set; } = null!;
        }

        [Test]
        public void RefStructProperty_InitializesOtherMembers()
        {
            var fixture = Fixture.New<RefStructPropertyClass>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("Value"));
        }

        #endregion
    }
}
