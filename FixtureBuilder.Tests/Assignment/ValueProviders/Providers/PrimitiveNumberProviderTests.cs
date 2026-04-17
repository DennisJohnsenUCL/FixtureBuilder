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

        [TestCase(typeof(byte), TestName = "Resolve_Byte_ReturnsNonZeroValue")]
        [TestCase(typeof(sbyte), TestName = "Resolve_SByte_ReturnsNonZeroValue")]
        [TestCase(typeof(short), TestName = "Resolve_Short_ReturnsNonZeroValue")]
        [TestCase(typeof(ushort), TestName = "Resolve_UShort_ReturnsNonZeroValue")]
        [TestCase(typeof(int), TestName = "Resolve_Int_ReturnsNonZeroValue")]
        [TestCase(typeof(uint), TestName = "Resolve_UInt_ReturnsNonZeroValue")]
        [TestCase(typeof(long), TestName = "Resolve_Long_ReturnsNonZeroValue")]
        [TestCase(typeof(ulong), TestName = "Resolve_ULong_ReturnsNonZeroValue")]
        [TestCase(typeof(float), TestName = "Resolve_Float_ReturnsNonZeroValue")]
        [TestCase(typeof(double), TestName = "Resolve_Double_ReturnsNonZeroValue")]
        [TestCase(typeof(decimal), TestName = "Resolve_Decimal_ReturnsNonZeroValue")]
        public void Resolve_NumericType_ReturnsNonZeroValue(Type numericType)
        {
            var request = new FixtureRequest(numericType);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.Not.EqualTo(Convert.ChangeType(0, numericType)));
        }

        [TestCase(typeof(string), TestName = "Resolve_String_ReturnsNoResult")]
        [TestCase(typeof(bool), TestName = "Resolve_Bool_ReturnsNoResult")]
        [TestCase(typeof(char), TestName = "Resolve_Char_ReturnsNoResult")]
        public void Resolve_UnsupportedType_ReturnsNoResult(Type type)
        {
            var request = new FixtureRequest(type);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
