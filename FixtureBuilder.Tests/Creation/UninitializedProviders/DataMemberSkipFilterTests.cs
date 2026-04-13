#pragma warning disable CS0649

using System.Reflection;
using System.Runtime.CompilerServices;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Tests.Creation.UninitializedProviders
{
    internal sealed class DataMemberSkipFilterTests
    {
        #region Test types

        private static DataMemberInfo PropertyMember<T>(string name)
        {
            return new DataMemberInfo(typeof(T).GetProperty(name)!);
        }

        private static DataMemberInfo FieldMember<T>(string name)
        {
            return new DataMemberInfo(typeof(T).GetField(name)!);
        }

        private class StandardClass
        {
            public string NonNullableString { get; set; } = "";
            public string? NullableString { get; set; }
            public int NonNullableInt { get; set; }
            public int? NullableInt { get; set; }
            public string ReadOnly { get; } = null!;
#pragma warning disable CA1822 // Mark members as static
            public string WriteOnly { set { } }
#pragma warning restore CA1822 // Mark members as static
            public string this[int index] { get => ""; set { } }
            public static string StaticProperty { get; set; } = "";
            public int RegularField;
            public static int StaticField;
        }

        private class CompilerGeneratedBackingFieldClass
        {
            public string AutoProp { get; set; } = "";
        }

        #endregion

        private DataMemberSkipFilter _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new DataMemberSkipFilter();
        }

        [Test]
        public void DefaultConstructor_CreatesInstance()
        {
            Assert.DoesNotThrow(() => new DataMemberSkipFilter());
        }

        #region Namespace skipping

        [Test]
        public void ShouldSkip_PropertyFromSystemNamespace_ReturnsTrue()
        {
            var member = PropertyMember<Exception>(nameof(Exception.Message));

            var result = _sut.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldSkip_PropertyFromUserNamespace_ReturnsFalse()
        {
            var member = PropertyMember<StandardClass>(nameof(StandardClass.NonNullableString));

            var result = _sut.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.False);
        }

        #endregion

        #region Static members

        [Test]
        public void ShouldSkip_StaticProperty_ReturnsTrue()
        {
            var member = PropertyMember<StandardClass>(nameof(StandardClass.StaticProperty));

            var result = _sut.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldSkip_StaticField_ReturnsTrue()
        {
            var member = FieldMember<StandardClass>(nameof(StandardClass.StaticField));

            var result = _sut.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.True);
        }

        #endregion

        #region Inaccessible properties

        [Test]
        public void ShouldSkip_ReadOnlyProperty_ReturnsTrue()
        {
            var member = PropertyMember<StandardClass>(nameof(StandardClass.ReadOnly));

            var result = _sut.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldSkip_WriteOnlyProperty_ReturnsTrue()
        {
            var member = PropertyMember<StandardClass>(nameof(StandardClass.WriteOnly));

            var result = _sut.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldSkip_IndexerProperty_ReturnsTrue()
        {
            var property = typeof(StandardClass).GetProperties()
                .First(p => p.GetIndexParameters().Length > 0);
            var member = new DataMemberInfo(property);

            var result = _sut.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldSkip_ReadWriteProperty_ReturnsFalse()
        {
            var member = PropertyMember<StandardClass>(nameof(StandardClass.NonNullableString));

            var result = _sut.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.False);
        }

        #endregion

        #region Compiler-generated fields

        [Test]
        public void ShouldSkip_CompilerGeneratedBackingField_ReturnsTrue()
        {
            var field = typeof(CompilerGeneratedBackingFieldClass)
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(f => f.IsDefined(typeof(CompilerGeneratedAttribute), false));
            var member = new DataMemberInfo(field);

            var result = _sut.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldSkip_RegularField_ReturnsFalse()
        {
            var member = FieldMember<StandardClass>(nameof(StandardClass.RegularField));

            var result = _sut.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.False);
        }

        #endregion

        #region Nullable with InitializeMembers.NonNullables

        [Test]
        public void ShouldSkip_NonNullablesAndNullableValueType_ReturnsTrue()
        {
            var member = PropertyMember<StandardClass>(nameof(StandardClass.NullableInt));

            var result = _sut.ShouldSkip(member, InitializeMembers.NonNullables);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldSkip_NonNullablesAndNonNullableValueType_ReturnsFalse()
        {
            var member = PropertyMember<StandardClass>(nameof(StandardClass.NonNullableInt));

            var result = _sut.ShouldSkip(member, InitializeMembers.NonNullables);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldSkip_NonNullablesAndNullableReferenceType_ReturnsTrue()
        {
            var member = PropertyMember<StandardClass>(nameof(StandardClass.NullableString));

            var result = _sut.ShouldSkip(member, InitializeMembers.NonNullables);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldSkip_NonNullablesAndNonNullableReferenceType_ReturnsFalse()
        {
            var member = PropertyMember<StandardClass>(nameof(StandardClass.NonNullableString));

            var result = _sut.ShouldSkip(member, InitializeMembers.NonNullables);

            Assert.That(result, Is.False);
        }

        #endregion

        #region Nullable with InitializeMembers.All

        [Test]
        public void ShouldSkip_AllAndNullableValueType_ReturnsFalse()
        {
            var member = PropertyMember<StandardClass>(nameof(StandardClass.NullableInt));

            var result = _sut.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldSkip_AllAndNullableReferenceType_ReturnsFalse()
        {
            var member = PropertyMember<StandardClass>(nameof(StandardClass.NullableString));

            var result = _sut.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.False);
        }

        #endregion
    }
}
