using System.Collections.Immutable;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders
{
    internal sealed class ValueProviderFactoryTests
    {
        [TestCase(typeof(string))]
        [TestCase(typeof(DayOfWeek))]
        [TestCase(typeof(Guid))]
        [TestCase(typeof(DateTime))]
        [TestCase(typeof(TimeSpan))]
        [TestCase(typeof(int))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(List<int>))]
        [TestCase(typeof(ImmutableList<int>))]
        [TestCase(typeof(bool))]
        [TestCase(typeof(char))]
        [TestCase(typeof(Func<int>))]
        [TestCase(typeof(Task))]
        [TestCase(typeof(Task<int>))]
        public void CreateDefaultValueProvider_ProvidesValueOfRequestedType(Type type)
        {
            var provider = ValueProviderFactory.CreateDefaultValueProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.InstanceOf(type));
        }

        [Test]
        public void CreateDefaultValueProvider_ParameterWithDefaultValue_ProvidesDefaultValueBeforeTypeProviders()
        {
            static void Method(int value = 3) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = ValueProviderFactory.CreateDefaultValueProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void CreateDefaultValueProvider_NullableParameter_ProvidesNullBeforeTypeProviders()
        {
            static void Method(string? value) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = ValueProviderFactory.CreateDefaultValueProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.Null);
        }
    }
}
