using System.Reflection;
using System.Runtime.CompilerServices;
using FixtureBuilder.Core;
using FixtureBuilder.Creation.UninitializedProviders.MemberInitialization;
using Microsoft.Contoso;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Contoso
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    internal class Widget
    {
        public string Name { get; set; } = null!;
    }
}

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MicrosoftGraphClient
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    internal class GraphUser
    {
        public string Name { get; set; } = null!;
    }
}

namespace FixtureBuilder.Tests.Creation.UninitializedProviders.MemberInitialization
{
    internal sealed class DataMemberSkipFilterTests
    {
        #region Namespaces

        [Test]
        public void ShouldSkip_PropertyFromSystemNamespace_ReturnsTrue()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(Exception), nameof(Exception.Source));

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldSkip_PropertyFromSystemChildNamespace_ReturnsTrue()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(List<int>), nameof(List<>.Capacity));

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldSkip_PropertyFromMicrosoftChildNamespace_ReturnsTrue()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(Widget), nameof(Widget.Name));

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldSkip_PropertyFromNamespaceStartingWithSystem_ReturnsFalse()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(SystemUnderTest.Invoice), nameof(SystemUnderTest.Invoice.Number));

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldSkip_PropertyFromNamespaceStartingWithMicrosoft_ReturnsFalse()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(MicrosoftGraphClient.GraphUser), nameof(MicrosoftGraphClient.GraphUser.Name));

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldSkip_PropertyFromGlobalNamespace_ReturnsFalse()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(GlobalNamespaceClass), nameof(GlobalNamespaceClass.Value));

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldSkip_PropertyFromUserNamespace_ReturnsFalse()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(PropertyHolder<string>), "Value");

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.False);
        }

        #endregion

        #region Inaccessible properties

        private class GetOnlyAutoPropertyClass
        {
            public string Value { get; } = null!;
        }

        [Test]
        public void ShouldSkip_GetOnlyAutoProperty_ReturnsFalse()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(GetOnlyAutoPropertyClass), nameof(GetOnlyAutoPropertyClass.Value));

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldSkip_ReadOnlyPropertyWithoutAutoBackingField_ReturnsTrue()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(ReadOnlyPropertyHolder<string>), "Value");

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldSkip_WriteOnlyProperty_ReturnsTrue()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(WriteOnlyPropertyClass), nameof(WriteOnlyPropertyClass.Value));

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.True);
        }

        private class IndexerClass
        {
            public string this[int index] { get => null!; set { } }
        }

        [Test]
        public void ShouldSkip_Indexer_ReturnsTrue()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(IndexerClass), "Item");

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.True);
        }

        private class SpanPropertyClass
        {
            private int[] _numbers = [];
            public Span<int> Numbers { get => _numbers; set => _numbers = value.ToArray(); }
        }

        [Test]
        public void ShouldSkip_RefStructProperty_ReturnsTrue()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(SpanPropertyClass), nameof(SpanPropertyClass.Numbers));

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.True);
        }

        #endregion

        #region Fields

        [Test]
        public void ShouldSkip_Field_ReturnsFalse()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldSkip_CompilerGeneratedBackingField_ReturnsTrue()
        {
            var filter = new DataMemberSkipFilter();
            var field = typeof(PropertyHolder<string>)
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(candidate => candidate.IsDefined(typeof(CompilerGeneratedAttribute), false));
            var member = new DataMemberInfo(field);

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.True);
        }

        #endregion

        #region Nullable members

        private class NullableAndNonNullableMembersClass
        {
            public string NonNullableString { get; set; } = null!;
            public string? NullableString { get; set; }
            public int NonNullableInt { get; set; }
            public int? NullableInt { get; set; }
        }

        [Test]
        public void ShouldSkip_NonNullablesAndNullableValueType_ReturnsTrue()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(NullableAndNonNullableMembersClass), nameof(NullableAndNonNullableMembersClass.NullableInt));

            var result = filter.ShouldSkip(member, InitializeMembers.NonNullables);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldSkip_NonNullablesAndNonNullableValueType_ReturnsFalse()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(NullableAndNonNullableMembersClass), nameof(NullableAndNonNullableMembersClass.NonNullableInt));

            var result = filter.ShouldSkip(member, InitializeMembers.NonNullables);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldSkip_NonNullablesAndNullableReferenceType_ReturnsTrue()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(NullableAndNonNullableMembersClass), nameof(NullableAndNonNullableMembersClass.NullableString));

            var result = filter.ShouldSkip(member, InitializeMembers.NonNullables);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ShouldSkip_NonNullablesAndNonNullableReferenceType_ReturnsFalse()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(NullableAndNonNullableMembersClass), nameof(NullableAndNonNullableMembersClass.NonNullableString));

            var result = filter.ShouldSkip(member, InitializeMembers.NonNullables);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldSkip_AllAndNullableValueType_ReturnsFalse()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(NullableAndNonNullableMembersClass), nameof(NullableAndNonNullableMembersClass.NullableInt));

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ShouldSkip_AllAndNullableReferenceType_ReturnsFalse()
        {
            var filter = new DataMemberSkipFilter();
            var member = TestHelper.PropertyMember(typeof(NullableAndNonNullableMembersClass), nameof(NullableAndNonNullableMembersClass.NullableString));

            var result = filter.ShouldSkip(member, InitializeMembers.All);

            Assert.That(result, Is.False);
        }

        #endregion
    }
}
