using System.Reflection;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class GetDataMembersTests
    {
        private class PublicPropertyAndFieldClass
        {
            public string Property { get; set; } = null!;
            public int Field;
        }

        [Test]
        public void PublicInstance_ReturnsPublicPropertiesThenFields()
        {
            var result = typeof(PublicPropertyAndFieldClass).GetDataMembers(BindingFlags.Public | BindingFlags.Instance);

            var names = result.Select(member => member.Name);
            Assert.That(names, Is.EqualTo(["Property", "Field"]));
        }

        private class PrivatePropertyAndFieldClass
        {
            private readonly string _field = null!;
            private string Property => _field;
        }

        [Test]
        public void NonPublicInstance_ReturnsPrivatePropertiesThenFields()
        {
            var result = typeof(PrivatePropertyAndFieldClass).GetDataMembers(BindingFlags.NonPublic | BindingFlags.Instance);

            var names = result.Select(member => member.Name);
            Assert.That(names, Is.EqualTo(["Property", "_field"]));
        }

        [Test]
        public void EmptyClass_ReturnsEmpty()
        {
            var result = typeof(EmptyClass).GetDataMembers(BindingFlags.Public | BindingFlags.Instance);

            Assert.That(result, Is.Empty);
        }
    }
}
