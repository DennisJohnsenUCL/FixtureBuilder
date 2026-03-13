using FixtureBuilder.ValueConverters.ConverterBuilders;
using FixtureBuilder.ValueConverters.Decorators;

namespace FixtureBuilder.Tests.ValueConverters.ConverterBuilders
{
    internal class ConverterFactoryTests
    {
        private ConverterFactory _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new ConverterFactory();
        }

        #region Constructor

        [Test]
        public void Constructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new ConverterFactory());
        }

        #endregion

        #region CreateDefaultConverter

        [Test]
        public void CreateDefaultConverter_ReturnsNonNullResult()
        {
            var result = _sut.CreateDefaultConverter();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CreateDefaultConverter_ReturnsValidatingConverter()
        {
            var result = _sut.CreateDefaultConverter();

            Assert.That(result, Is.InstanceOf<ValidatingConverter>());
        }

        #endregion
    }
}
