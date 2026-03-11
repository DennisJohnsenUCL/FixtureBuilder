using System.Reflection;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions
{
    internal sealed class PropertyInfoExtensionsTests
    {
        private class TestClass
        {
            public static int StaticProperty { get; set; }
            public int InstanceProperty { get; set; }
            public static int StaticGetOnly => 1;
            private static int PrivateStaticProperty { get; set; }
            public int PublicGetPrivateSet { get; private set; }
        }

        private static PropertyInfo GetProperty(string name) =>
            typeof(TestClass).GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)!;

        [Test]
        public void IsStatic_StaticProperty_ReturnsTrue()
        {
            Assert.That(GetProperty(nameof(TestClass.StaticProperty)).IsStatic(), Is.True);
        }

        [Test]
        public void IsStatic_InstanceProperty_ReturnsFalse()
        {
            Assert.That(GetProperty(nameof(TestClass.InstanceProperty)).IsStatic(), Is.False);
        }

        [Test]
        public void IsStatic_StaticGetOnlyProperty_ReturnsTrue()
        {
            Assert.That(GetProperty(nameof(TestClass.StaticGetOnly)).IsStatic(), Is.True);
        }

        [Test]
        public void IsStatic_PrivateStaticProperty_WithNonPublicFalse_ReturnsFalse()
        {
            Assert.That(GetProperty("PrivateStaticProperty").IsStatic(nonPublic: false), Is.False);
        }

        [Test]
        public void IsStatic_PrivateStaticProperty_WithNonPublicTrue_ReturnsTrue()
        {
            Assert.That(GetProperty("PrivateStaticProperty").IsStatic(nonPublic: true), Is.True);
        }
    }
}
