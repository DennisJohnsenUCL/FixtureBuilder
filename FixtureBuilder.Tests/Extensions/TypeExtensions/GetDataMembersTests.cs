#pragma warning disable CS0649
#pragma warning disable CS0169

using System.Reflection;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class GetDataMembersTests
    {
        private const BindingFlags PublicInstance =
            BindingFlags.Public | BindingFlags.Instance;

        private const BindingFlags NonPublicInstance =
            BindingFlags.NonPublic | BindingFlags.Instance;

        private const BindingFlags PublicStatic =
            BindingFlags.Public | BindingFlags.Static;


        private class SampleClass
        {
            public string PublicProperty { get; set; } = null!;
            public int PublicField;

            private string PrivateProperty { get; set; } = null!;
            private readonly int _privateField;

            public static string StaticProperty { get; set; } = null!;
            public static int StaticField;
        }

        private class EmptyClass { }

        [Test]
        public void GetDataMembers_NullType_ThrowsArgumentNullException()
        {
            Type type = null!;

            Assert.Throws<ArgumentNullException>(() =>
                type.GetDataMembers(PublicInstance).ToList());
        }

        [Test]
        public void GetDataMembers_PublicInstance_ReturnsPublicPropertiesAndFields()
        {
            var members = typeof(SampleClass).GetDataMembers(PublicInstance).ToList();

            var memberNames = members.Select(m => m.Name).ToList();

            Assert.That(memberNames, Does.Contain(nameof(SampleClass.PublicProperty)));
            Assert.That(memberNames, Does.Contain(nameof(SampleClass.PublicField)));
            Assert.That(memberNames, Does.Not.Contain(nameof(SampleClass.StaticProperty)));
            Assert.That(memberNames, Does.Not.Contain(nameof(SampleClass.StaticField)));
        }

        [Test]
        public void GetDataMembers_NonPublicInstance_ReturnsPrivateMembers()
        {
            var members = typeof(SampleClass).GetDataMembers(NonPublicInstance).ToList();

            var memberNames = members.Select(m => m.Name).ToList();

            Assert.That(memberNames, Does.Contain("PrivateProperty"));
            Assert.That(memberNames, Does.Contain("_privateField"));
        }

        [Test]
        public void GetDataMembers_PublicStatic_ReturnsStaticMembers()
        {
            var members = typeof(SampleClass).GetDataMembers(PublicStatic).ToList();

            var memberNames = members.Select(m => m.Name).ToList();

            Assert.That(memberNames, Does.Contain(nameof(SampleClass.StaticProperty)));
            Assert.That(memberNames, Does.Contain(nameof(SampleClass.StaticField)));
            Assert.That(memberNames, Does.Not.Contain(nameof(SampleClass.PublicProperty)));
        }

        [Test]
        public void GetDataMembers_EmptyClass_ReturnsEmpty()
        {
            var members = typeof(EmptyClass).GetDataMembers(PublicInstance).ToList();

            Assert.That(members, Is.Empty);
        }
    }
}
