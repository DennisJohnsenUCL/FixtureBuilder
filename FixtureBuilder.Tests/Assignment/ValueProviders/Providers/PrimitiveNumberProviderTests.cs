using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class PrimitiveNumberProviderTests
    {
        private PrimitiveNumberProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _sut = new PrimitiveNumberProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new PrimitiveNumberProvider());
        }

        // --- Integer types ---

        [Test]
        public void Resolve_Byte_ReturnsByteInRange()
        {
            var request = new FixtureRequest(typeof(byte));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<byte>());
                Assert.That(result, Is.InRange((byte)1, (byte)10));
            }
        }

        [Test]
        public void Resolve_SByte_ReturnsSByteInRange()
        {
            var request = new FixtureRequest(typeof(sbyte));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<sbyte>());
                Assert.That(result, Is.InRange((sbyte)1, (sbyte)10));
            }
        }

        [Test]
        public void Resolve_Short_ReturnsShortInRange()
        {
            var request = new FixtureRequest(typeof(short));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<short>());
                Assert.That(result, Is.InRange((short)1, (short)10));
            }
        }

        [Test]
        public void Resolve_UShort_ReturnsUShortInRange()
        {
            var request = new FixtureRequest(typeof(ushort));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<ushort>());
                Assert.That(result, Is.InRange((ushort)1, (ushort)10));
            }
        }

        [Test]
        public void Resolve_Int_ReturnsIntInRange()
        {
            var request = new FixtureRequest(typeof(int));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<int>());
                Assert.That(result, Is.InRange(1, 10));
            }
        }

        [Test]
        public void Resolve_UInt_ReturnsUIntInRange()
        {
            var request = new FixtureRequest(typeof(uint));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<uint>());
                Assert.That(result, Is.InRange(1u, 10u));
            }
        }

        [Test]
        public void Resolve_Long_ReturnsLongInRange()
        {
            var request = new FixtureRequest(typeof(long));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<long>());
                Assert.That(result, Is.InRange(1L, 10L));
            }
        }

        [Test]
        public void Resolve_ULong_ReturnsULongInRange()
        {
            var request = new FixtureRequest(typeof(ulong));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<ulong>());
                Assert.That(result, Is.InRange(1UL, 10UL));
            }
        }

        // --- Floating-point and decimal types ---

        [Test]
        public void Resolve_Float_ReturnsFloatInRange()
        {
            var request = new FixtureRequest(typeof(float));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<float>());
                Assert.That(result, Is.InRange(1f, 10f));
            }
        }

        [Test]
        public void Resolve_Double_ReturnsDoubleInRange()
        {
            var request = new FixtureRequest(typeof(double));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<double>());
                Assert.That(result, Is.InRange(1d, 10d));
            }
        }

        [Test]
        public void Resolve_Decimal_ReturnsDecimalInRange()
        {
            var request = new FixtureRequest(typeof(decimal));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<decimal>());
                Assert.That(result, Is.InRange(1m, 10m));
            }
        }

        // --- Whole-number values for floating-point types ---

        [Test]
        public void Resolve_Float_ReturnsWholeNumber()
        {
            var request = new FixtureRequest(typeof(float));

            var result = (float)_sut.ResolveValue(request, _contextMock.Object)!;

            Assert.That(result % 1, Is.Zero);
        }

        [Test]
        public void Resolve_Double_ReturnsWholeNumber()
        {
            var request = new FixtureRequest(typeof(double));

            var result = (double)_sut.ResolveValue(request, _contextMock.Object)!;

            Assert.That(result % 1, Is.Zero);
        }

        // --- Unsupported types ---

        [Test]
        public void Resolve_String_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(string));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Resolve_Bool_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(bool));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Resolve_Char_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(char));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
