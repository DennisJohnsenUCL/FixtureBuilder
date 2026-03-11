using FixtureBuilder.FixtureContexts;
using FixtureBuilder.FixtureProviders.Providers;
using Moq;

namespace FixtureBuilder.Tests.FixtureProviders.Providers
{
    internal sealed class ArrayProviderTests
    {
        private ArrayProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _sut = new ArrayProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new ArrayProvider());
        }

        [Test]
        public void Resolve_IntArray_ReturnsIntArrayOfLength10()
        {
            var request = new FixtureRequest(typeof(int[]));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<int[]>());
                Assert.That(result, Has.Length.EqualTo(10));
            }
        }

        [Test]
        public void Resolve_StringArray_ReturnsStringArrayOfLength10()
        {
            var request = new FixtureRequest(typeof(string[]));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<string[]>());
                Assert.That(result, Has.Length.EqualTo(10));
            }
        }

        [Test]
        public void Resolve_IntArray_ReturnsArrayWithDefaultValues()
        {
            var request = new FixtureRequest(typeof(int[]));

            var result = (int[])_sut.Resolve(request, _contextMock.Object)!;

            Assert.That(result, Is.All.EqualTo(0));
        }

        [Test]
        public void Resolve_StringArray_ReturnsArrayWithNullElements()
        {
            var request = new FixtureRequest(typeof(string[]));

            var result = (string?[])_sut.Resolve(request, _contextMock.Object)!;

            Assert.That(result, Is.All.Null);
        }

        [Test]
        public void Resolve_NonArrayType_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(int));

            var result = _sut.Resolve(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_ClassType_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(string));

            var result = _sut.Resolve(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_MultidimensionalArray_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(int[,]));

            var result = _sut.Resolve(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_JaggedArray_ReturnsOuterArrayOfLength10()
        {
            var request = new FixtureRequest(typeof(int[][]));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<int[][]>());
                Assert.That(result, Has.Length.EqualTo(10));
            }
        }
    }
}
