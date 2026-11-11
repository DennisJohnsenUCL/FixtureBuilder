using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class GetDefaultValueTests
    {
        #region Arguments

        [Test]
        public void NullType_ThrowsArgumentNullException()
        {
            Type type = null!;

            Assert.Throws<ArgumentNullException>(() => type.GetDefaultValue());
        }

        #endregion

        #region Value types

        [Test]
        public void PrimitiveValueType_ReturnsZero()
        {
            var result = typeof(int).GetDefaultValue();

            Assert.That(result, Is.Zero);
        }

        [Test]
        public void Enum_ReturnsZeroValue()
        {
            var result = typeof(DayOfWeek).GetDefaultValue();

            Assert.That(result, Is.EqualTo(DayOfWeek.Sunday));
        }

        [Test]
        public void Struct_ReturnsZeroedStruct()
        {
            var result = typeof(PointStruct).GetDefaultValue();

            Assert.That(result, Is.EqualTo(default(PointStruct)));
        }

        private struct FieldInitializerStruct
        {
            public int Value = 42;

            public FieldInitializerStruct() { }
        }

        [Test]
        public void StructWithFieldInitializer_ReturnsZeroedStructWithoutRunningConstructor()
        {
            var result = typeof(FieldInitializerStruct).GetDefaultValue();

            Assert.That(result, Is.EqualTo(default(FieldInitializerStruct)));
        }

        #endregion

        #region Types without a value-type default

        [Test]
        public void NullableValueType_ReturnsNull()
        {
            var result = typeof(int?).GetDefaultValue();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ReferenceType_ReturnsNull()
        {
            var result = typeof(string).GetDefaultValue();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Interface_ReturnsNull()
        {
            var result = typeof(IDisposable).GetDefaultValue();

            Assert.That(result, Is.Null);
        }

        #endregion
    }
}
