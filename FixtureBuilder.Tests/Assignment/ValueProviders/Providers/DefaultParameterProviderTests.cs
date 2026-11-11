using System.Reflection;
using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class DefaultParameterProviderTests
    {
        #region Default values

        [Test]
        public void ResolveValue_ValueTypeDefault_ReturnsDefault()
        {
            static void Method(int value = 42) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = new DefaultParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void ResolveValue_ReferenceTypeDefault_ReturnsDefault()
        {
            static void Method(string value = "default") { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = new DefaultParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.EqualTo("default"));
        }

        [Test]
        public void ResolveValue_NullDefaultForReferenceType_ReturnsNull()
        {
            static void Method(string? value = null) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = new DefaultParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveValue_NullDefaultForNullableValueType_ReturnsNull()
        {
            static void Method(int? value = null) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = new DefaultParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveValue_DefaultLiteralForStruct_ReturnsDefaultStruct()
        {
            static void Method(CancellationToken value = default) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = new DefaultParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.EqualTo(default(CancellationToken)));
        }

        #endregion

        #region Nullable enum defaults

        [Test]
        public void ResolveValue_NullableEnumDefault_ReturnsEnumValue()
        {
            static void Method(DayOfWeek? value = DayOfWeek.Friday) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = new DefaultParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.EqualTo(DayOfWeek.Friday));
        }

        private enum ByteEnum : byte
        {
            First,
            Second
        }

        [Test]
        public void ResolveValue_NullableByteEnumDefault_ReturnsEnumValue()
        {
            static void Method(ByteEnum? value = ByteEnum.Second) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = new DefaultParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.EqualTo(ByteEnum.Second));
        }

        [Test]
        public void ResolveValue_NullableFlagsEnumCombinedDefault_ReturnsEnumValue()
        {
            static void Method(BindingFlags? value = BindingFlags.Public | BindingFlags.Instance) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = new DefaultParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.EqualTo(BindingFlags.Public | BindingFlags.Instance));
        }

        [Test]
        public void ResolveValue_NullableEnumNullDefault_ReturnsNull()
        {
            static void Method(DayOfWeek? value = null) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = new DefaultParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.Null);
        }

        #endregion

        #region No result

        [Test]
        public void ResolveValue_DefaultValuesNotPreferred_ReturnsNoResult()
        {
            static void Method(int value = 42) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var options = new FixtureOptions { PreferDefaultParameterValues = false };
            var provider = new DefaultParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void ResolveValue_ParameterWithoutDefault_ReturnsNoResult()
        {
            static void Method(int value) { }
            var request = TestHelper.MakeRequest(TestHelper.FirstParameter(Method));
            var provider = new DefaultParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void ResolveValue_SourceIsNotParameter_ReturnsNoResult()
        {
            var request = TestHelper.MakeRequest(typeof(int));
            var provider = new DefaultParameterProvider();

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        #endregion
    }
}
