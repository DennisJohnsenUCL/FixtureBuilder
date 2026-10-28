#pragma warning disable CS0649

using System.Reflection;
using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.Core
{
    internal sealed class FixtureRequestTests
    {
        private class SampleClass
        {
            public string Property { get; set; } = "";
            public int Field;

            public SampleClass(string parameter)
            {
                Property = parameter;
            }
        }

        private static PropertyInfo GetPropertyInfo() =>
            typeof(SampleClass).GetProperty(nameof(SampleClass.Property))!;

        private static FieldInfo GetFieldInfo() =>
            typeof(SampleClass).GetField(nameof(SampleClass.Field))!;

        private static ParameterInfo GetParameterInfo() =>
            typeof(SampleClass).GetConstructors()[0].GetParameters()[0];

        [Test]
        public void Constructor_FullConstructor_SetsRootType()
        {
            var request = new FixtureRequest(typeof(string), GetPropertyInfo(), typeof(double));

            Assert.That(request.RootType, Is.EqualTo(typeof(double)));
        }

        [Test]
        public void Constructor_NoRootType_DefaultsToType()
        {
            var request = new FixtureRequest(typeof(string));

            Assert.That(request.RootType, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void Constructor_ParameterSource_SetsNameFromParameter()
        {
            var request = new FixtureRequest(typeof(string), GetParameterInfo(), typeof(double));

            Assert.That(request.Name, Is.EqualTo("parameter"));
        }

        [Test]
        public void Constructor_PropertySource_SetsNameFromProperty()
        {
            var request = new FixtureRequest(typeof(string), GetPropertyInfo(), typeof(double));

            Assert.That(request.Name, Is.EqualTo(nameof(SampleClass.Property)));
        }

        [Test]
        public void Constructor_FieldSource_SetsNameFromField()
        {
            var request = new FixtureRequest(typeof(int), GetFieldInfo(), typeof(double));

            Assert.That(request.Name, Is.EqualTo(nameof(SampleClass.Field)));
        }

        [Test]
        public void Constructor_UnsupportedSource_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new FixtureRequest(typeof(string), "source", typeof(double)));
        }
    }
}
