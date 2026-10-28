#pragma warning disable IDE0060

using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.Tests.FixtureFactories.WithMatching.WithRules
{
    internal sealed class ParameterRuleTests
    {
        private static ParameterInfo GetParameterInfo()
        {
            return typeof(SampleMethods)
                .GetMethod(nameof(SampleMethods.Method), BindingFlags.Static | BindingFlags.NonPublic)!
                .GetParameters()
                .First();
        }

        private static class SampleMethods
        {
            internal static void Method(string value) { }
        }

        private class SampleClass
        {
            public string Name { get; set; } = "";
        }

        [Test]
        public void IsMatch_RequestSourceIsParameterInfo_ReturnsTrue()
        {
            var paramInfo = GetParameterInfo();
            var request = new FixtureRequest(paramInfo.ParameterType, paramInfo, typeof(object));
            var sut = new ParameterRule();

            Assert.That(sut.IsMatch(request), Is.True);
        }

        [Test]
        public void IsMatch_RequestSourceIsNull_ReturnsFalse()
        {
            var request = new FixtureRequest(typeof(string));
            var sut = new ParameterRule();

            Assert.That(sut.IsMatch(request), Is.False);
        }

        [Test]
        public void IsMatch_RequestSourceIsNotParameterInfo_ReturnsFalse()
        {
            var property = typeof(SampleClass).GetProperty(nameof(SampleClass.Name))!;
            var request = new FixtureRequest(property.PropertyType, property, typeof(object));
            var sut = new ParameterRule();

            Assert.That(sut.IsMatch(request), Is.False);
        }
    }
}
