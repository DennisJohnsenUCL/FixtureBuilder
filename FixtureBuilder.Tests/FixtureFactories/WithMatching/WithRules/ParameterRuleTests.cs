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

        [Test]
        public void IsMatch_RequestSourceIsParameterInfo_ReturnsTrue()
        {
            var paramInfo = GetParameterInfo();
            var request = new FixtureRequest(paramInfo.ParameterType, paramInfo, typeof(object), null);
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
            var request = new FixtureRequest(typeof(string), this, typeof(object), "Name");
            var sut = new ParameterRule();

            Assert.That(sut.IsMatch(request), Is.False);
        }
    }
}
