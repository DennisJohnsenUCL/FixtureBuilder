using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class UnwrapTests
    {
        [Test]
        public void Unwrap_NullType_ThrowsArgumentNullException()
        {
            Type type = null!;

            Assert.Throws<ArgumentNullException>(() =>
                type.Unwrap());
        }

        [Test]
        public void Unwrap_PlainValueType_ReturnsSameType()
        {
            var result = typeof(int).Unwrap();

            Assert.That(result, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void Unwrap_PlainReferenceType_ReturnsSameType()
        {
            var result = typeof(string).Unwrap();

            Assert.That(result, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void Unwrap_NullableValueType_ReturnsUnderlyingType()
        {
            var result = typeof(int?).Unwrap();

            Assert.That(result, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void Unwrap_ByRefValueType_ReturnsElementType()
        {
            var result = typeof(int).MakeByRefType().Unwrap();

            Assert.That(result, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void Unwrap_ByRefReferenceType_ReturnsElementType()
        {
            var result = typeof(string).MakeByRefType().Unwrap();

            Assert.That(result, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void Unwrap_ByRefNullableValueType_ReturnsUnderlyingType()
        {
            var result = typeof(int?).MakeByRefType().Unwrap();

            Assert.That(result, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void Unwrap_ArrayOfNullable_ReturnsSameType()
        {
            var result = typeof(int?[]).Unwrap();

            Assert.That(result, Is.EqualTo(typeof(int?[])));
        }

        [Test]
        public void Unwrap_GenericTypeWithNullableArgument_ReturnsSameType()
        {
            var result = typeof(List<int?>).Unwrap();

            Assert.That(result, Is.EqualTo(typeof(List<int?>)));
        }
    }
}
