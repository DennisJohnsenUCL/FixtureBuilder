using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class GetDefaultValueTests
    {
        private struct StructWithInitializer
        {
            public int Value = 42;

            public StructWithInitializer() { }
        }

        [Test]
        public void GetDefaultValue_NullType_ThrowsArgumentNullException()
        {
            Type type = null!;

            Assert.Throws<ArgumentNullException>(() =>
                type.GetDefaultValue());
        }

        [Test]
        public void GetDefaultValue_PrimitiveValueType_ReturnsZero()
        {
            var result = typeof(int).GetDefaultValue();

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetDefaultValue_Enum_ReturnsZeroValue()
        {
            var result = typeof(DayOfWeek).GetDefaultValue();

            Assert.That(result, Is.EqualTo(default(DayOfWeek)));
        }

        [Test]
        public void GetDefaultValue_Struct_ReturnsDefaultStruct()
        {
            var result = typeof(CancellationToken).GetDefaultValue();

            Assert.That(result, Is.EqualTo(default(CancellationToken)));
        }

        [Test]
        public void GetDefaultValue_StructWithInitializer_DoesNotRunConstructor()
        {
            var result = typeof(StructWithInitializer).GetDefaultValue();

            Assert.That(result, Is.TypeOf<StructWithInitializer>());
            Assert.That(((StructWithInitializer)result!).Value, Is.EqualTo(0));
        }

        [Test]
        public void GetDefaultValue_NullableValueType_ReturnsNull()
        {
            var result = typeof(int?).GetDefaultValue();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDefaultValue_ReferenceType_ReturnsNull()
        {
            var result = typeof(string).GetDefaultValue();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDefaultValue_Interface_ReturnsNull()
        {
            var result = typeof(IDisposable).GetDefaultValue();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDefaultValue_ByRefValueType_ReturnsNull()
        {
            var result = typeof(int).MakeByRefType().GetDefaultValue();

            Assert.That(result, Is.Null);
        }
    }
}
