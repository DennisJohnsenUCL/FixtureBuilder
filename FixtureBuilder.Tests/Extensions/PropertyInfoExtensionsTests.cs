using System.Reflection;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions
{
    internal sealed class PropertyInfoExtensionsTests
    {
        [Test]
        public void IsStatic_StaticProperty_ReturnsTrue()
        {
            var property = typeof(StaticMembersClass).GetProperty(nameof(StaticMembersClass.StaticProperty))!;

            var result = property.IsStatic();

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsStatic_InstanceProperty_ReturnsFalse()
        {
            var property = typeof(PropertyHolder<int>).GetProperty("Value")!;

            var result = property.IsStatic();

            Assert.That(result, Is.False);
        }

        private class StaticGetOnlyPropertyClass
        {
            public static int Value => 1;
        }

        [Test]
        public void IsStatic_StaticGetOnlyProperty_ReturnsTrue()
        {
            var property = typeof(StaticGetOnlyPropertyClass).GetProperty(nameof(StaticGetOnlyPropertyClass.Value))!;

            var result = property.IsStatic();

            Assert.That(result, Is.True);
        }

        private class PrivateStaticPropertyClass
        {
            private static int Value { get; set; }
        }

        [Test]
        public void IsStatic_PrivateStaticPropertyWithoutNonPublic_ReturnsFalse()
        {
            var property = typeof(PrivateStaticPropertyClass).GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Static)!;

            var result = property.IsStatic(nonPublic: false);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsStatic_PrivateStaticPropertyWithNonPublic_ReturnsTrue()
        {
            var property = typeof(PrivateStaticPropertyClass).GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Static)!;

            var result = property.IsStatic(nonPublic: true);

            Assert.That(result, Is.True);
        }
    }
}
