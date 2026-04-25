using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories
{
    internal sealed class CustomConverterAdapterTests
    {
        private Mock<ICustomConverter> _adapteeMock;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _adapteeMock = new Mock<ICustomConverter>();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void Constructor_NullAdaptee_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomConverterAdapter(null!));
        }

        [Test]
        public void Convert_DelegatesToAdapteeWithTargetAndValue()
        {
            var target = typeof(string);
            var request = new FixtureRequest(target);
            var value = "input";
            _adapteeMock.Setup(a => a.Convert(target, value)).Returns("converted");
            var sut = new CustomConverterAdapter(_adapteeMock.Object);

            var result = sut.Convert(request, value, _contextMock.Object);

            Assert.That(result, Is.EqualTo("converted"));
            _adapteeMock.Verify(a => a.Convert(target, value), Times.Once);
        }

        [Test]
        public void Convert_DoesNotPassContextToAdaptee()
        {
            var request = new FixtureRequest(typeof(string));
            _adapteeMock.Setup(a => a.Convert(It.IsAny<Type>(), It.IsAny<object>())).Returns("converted");
            var sut = new CustomConverterAdapter(_adapteeMock.Object);

            sut.Convert(request, "input", _contextMock.Object);

            _contextMock.VerifyNoOtherCalls();
        }
    }
}
