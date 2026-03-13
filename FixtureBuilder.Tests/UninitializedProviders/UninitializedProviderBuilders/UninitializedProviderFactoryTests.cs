using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.UninitializedProviders.UninitializedProviderBuilders;

namespace FixtureBuilder.Tests.UninitializedProviders.UninitializedProviderBuilders
{
    internal class UninitializedProviderFactoryTests
    {
        private UninitializedProviderFactory _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new UninitializedProviderFactory();
        }

        #region Constructor

        [Test]
        public void Constructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new UninitializedProviderFactory());
        }

        #endregion

        #region CreateDefaultUninitializedProvider

        [Test]
        public void CreateDefaultUninitializedProvider_ReturnsNonNullResult()
        {
            var result = _sut.CreateDefaultUninitializedProvider();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CreateDefaultUninitializedProvider_ReturnsRootUninitializedProvider()
        {
            var result = _sut.CreateDefaultUninitializedProvider();

            Assert.That(result, Is.InstanceOf<RootUninitializedProvider>());
        }

        #endregion
    }
}
