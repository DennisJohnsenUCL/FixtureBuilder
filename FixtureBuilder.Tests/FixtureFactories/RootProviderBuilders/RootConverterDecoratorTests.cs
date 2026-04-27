using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories.RootConfigurationBuilders;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories.RootProviderBuilders
{
    internal sealed class RootConverterDecoratorTests
    {
        private Mock<IValueConverter> _innerMock;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _innerMock = new Mock<IValueConverter>();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void Constructor_NullConverter_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => new RootConverterDecorator(null!, typeof(string)));
        }

        [Test]
        public void Constructor_NullRootType_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => new RootConverterDecorator(_innerMock.Object, null!));
        }

        [Test]
        public void Convert_MatchingRootType_DelegatesToInner()
        {
            var request = new FixtureRequest(typeof(string), "source", typeof(string), null);
            var value = "input";
            _innerMock
                .Setup(c => c.Convert(request, value, _contextMock.Object))
                .Returns("converted");
            var sut = new RootConverterDecorator(_innerMock.Object, typeof(string));

            var result = sut.Convert(request, value, _contextMock.Object);

            Assert.That(result, Is.EqualTo("converted"));
            _innerMock.Verify(c => c.Convert(request, value, _contextMock.Object), Times.Once);
        }

        [Test]
        public void Convert_NonMatchingRootType_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(string), "source", typeof(string), null);
            var sut = new RootConverterDecorator(_innerMock.Object, typeof(int));

            var result = sut.Convert(request, "input", _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
            _innerMock.Verify(
                c => c.Convert(It.IsAny<FixtureRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()),
                Times.Never);
        }

        [Test]
        public void Convert_InnerReturnsNull_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(string), "source", typeof(string), null);
            _innerMock
                .Setup(c => c.Convert(request, "input", _contextMock.Object))
                .Returns((object?)null);
            var sut = new RootConverterDecorator(_innerMock.Object, typeof(string));

            var result = sut.Convert(request, "input", _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_PassesContextToInner()
        {
            var request = new FixtureRequest(typeof(string), "source", typeof(string), null);
            var sut = new RootConverterDecorator(_innerMock.Object, typeof(string));

            sut.Convert(request, "input", _contextMock.Object);

            _innerMock.Verify(c => c.Convert(request, "input", _contextMock.Object), Times.Once);
        }
    }
}
