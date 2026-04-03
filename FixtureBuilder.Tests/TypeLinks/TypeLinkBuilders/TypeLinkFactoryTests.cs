using FixtureBuilder.TypeLinks.TypeLinkBuilders;

namespace FixtureBuilder.Tests.TypeLinks.TypeLinkBuilders
{
    internal sealed class TypeLinkFactoryTests
    {
        private TypeLinkFactory _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new TypeLinkFactory();
        }

        #region Constructor

        [Test]
        public void Constructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new TypeLinkFactory());
        }

        #endregion

        #region CreateDefaultTypeLink

        [Test]
        public void CreateDefaultTypeLink_ReturnsNonNullResult()
        {
            var result = _sut.CreateDefaultTypeLink();

            Assert.That(result, Is.Not.Null);
        }

        #endregion
    }
}
