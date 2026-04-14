#pragma warning disable CA1806

using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core.FixtureContexts.ContextResolvers;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts.ContextResolvers
{
    [TestFixture]
    internal sealed class EagerContextResolverTests
    {
        private IValueConverter _converter;
        private ITypeLink _typeLink;
        private IUninitializedProvider _uninitializedProvider;
        private IValueProvider _valueProvider;
        private IAutoConstructingProvider _autoConstructingProvider;

        [SetUp]
        public void SetUp()
        {
            _converter = new Mock<IValueConverter>().Object;
            _typeLink = new Mock<ITypeLink>().Object;
            _uninitializedProvider = new Mock<IUninitializedProvider>().Object;
            _valueProvider = new Mock<IValueProvider>().Object;
            _autoConstructingProvider = new Mock<IAutoConstructingProvider>().Object;
        }

        private void CreateResolver() =>
            new EagerContextResolver(_converter,
                _typeLink,
                _uninitializedProvider,
                _valueProvider,
                _autoConstructingProvider);

        [Test]
        public void Constructor_NullConverter_Throws()
        {
            _converter = null!;
            Assert.Throws<ArgumentNullException>(CreateResolver);
        }

        [Test]
        public void Constructor_NullTypeLink_Throws()
        {
            _typeLink = null!;
            Assert.Throws<ArgumentNullException>(CreateResolver);
        }

        [Test]
        public void Constructor_NullUninitializedProvider_Throws()
        {
            _uninitializedProvider = null!;
            Assert.Throws<ArgumentNullException>(CreateResolver);
        }

        [Test]
        public void Constructor_NullValueProvider_Throws()
        {
            _valueProvider = null!;
            Assert.Throws<ArgumentNullException>(CreateResolver);
        }

        [Test]
        public void Constructor_NullAutoConstructingProvider_Throws()
        {
            _autoConstructingProvider = null!;
            Assert.Throws<ArgumentNullException>(CreateResolver);
        }
    }
}
