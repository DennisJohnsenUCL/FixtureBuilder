using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders
{
    internal sealed class CompositeValueProviderTests
    {
        private class FixedValueProvider(object? value) : IValueProvider
        {
            public object? ResolveValue(FixtureRequest request, IFixtureContext context) => value;
        }

        private class NoResultProvider : IValueProvider
        {
            public object? ResolveValue(FixtureRequest request, IFixtureContext context) => NoResult.Instance;
        }

        #region Arguments

        [Test]
        public void Constructor_NullProviders_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CompositeValueProvider(null!));
        }

        #endregion

        #region Resolution

        [Test]
        public void ResolveValue_NoProviderResolves_ReturnsNoResult()
        {
            var composite = new CompositeValueProvider([new NoResultProvider(), new NoResultProvider()]);
            var request = TestHelper.MakeRequest(typeof(string));

            var result = composite.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        // Later providers can be user providers with side effects (fakers, counters), so they must not run once a value is found.
        [Test]
        public void ResolveValue_FirstProviderResolves_DoesNotCallLaterProviders()
        {
            var laterProvider = new Mock<IValueProvider>();
            var composite = new CompositeValueProvider([new FixedValueProvider("first"), laterProvider.Object]);
            var request = TestHelper.MakeRequest(typeof(string));

            composite.ResolveValue(request, Mock.Of<IFixtureContext>());

            laterProvider.Verify(x => x.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        #endregion

        #region Null results

        [Test]
        public void ResolveValue_NullForNullableValueType_ReturnsNull()
        {
            var composite = new CompositeValueProvider([new FixedValueProvider(null)]);
            var request = TestHelper.MakeRequest(typeof(int?));

            var result = composite.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveValue_NullForUnwrappedNullableParameter_ReturnsNull()
        {
            static void Method(int? value) { }
            var parameter = TestHelper.FirstParameter(Method);
            var request = new FixtureRequest(typeof(int), parameter, typeof(object));
            var composite = new CompositeValueProvider([new FixedValueProvider(null)]);

            var result = composite.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveValue_NullForByRefNonNullableParameter_ThrowsInvalidOperationException()
        {
            static void Method(in int value) { }
            var parameter = TestHelper.FirstParameter(Method);
            var request = TestHelper.MakeRequest(parameter);
            var composite = new CompositeValueProvider([new FixedValueProvider(null)]);

            Assert.Throws<InvalidOperationException>(() => composite.ResolveValue(request, Mock.Of<IFixtureContext>()));
        }

        #endregion
    }
}
