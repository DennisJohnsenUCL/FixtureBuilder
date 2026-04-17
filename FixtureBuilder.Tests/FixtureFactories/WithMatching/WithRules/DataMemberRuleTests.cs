using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.Tests.FixtureFactories.WithMatching.WithRules
{
    internal sealed class DataMemberRuleTests
    {
        private class SampleClass
        {
            public string Property { get; set; } = "";
            public int Field;
        }

        private static PropertyInfo GetPropertyInfo() =>
            typeof(SampleClass).GetProperty(nameof(SampleClass.Property))!;

        private static FieldInfo GetFieldInfo() =>
            typeof(SampleClass).GetField(nameof(SampleClass.Field))!;

        [Test]
        public void IsMatch_RequestSourceIsPropertyInfo_ReturnsTrue()
        {
            var prop = GetPropertyInfo();
            var request = new FixtureRequest(prop.PropertyType, prop, prop.Name);
            var sut = new DataMemberRule();

            Assert.That(sut.IsMatch(request), Is.True);
        }

        [Test]
        public void IsMatch_RequestSourceIsFieldInfo_ReturnsTrue()
        {
            var field = GetFieldInfo();
            var request = new FixtureRequest(field.FieldType, field, field.Name);
            var sut = new DataMemberRule();

            Assert.That(sut.IsMatch(request), Is.True);
        }

        [Test]
        public void IsMatch_RequestSourceIsNull_ReturnsFalse()
        {
            var request = new FixtureRequest(typeof(string));
            var sut = new DataMemberRule();

            Assert.That(sut.IsMatch(request), Is.False);
        }

        [Test]
        public void IsMatch_RequestSourceIsParameterInfo_ReturnsFalse()
        {
            var paramInfo = typeof(SampleClass).GetConstructors()[0].GetParameters();
            var request = new FixtureRequest(typeof(string), paramInfo, "Name");
            var sut = new DataMemberRule();

            Assert.That(sut.IsMatch(request), Is.False);
        }
    }
}
