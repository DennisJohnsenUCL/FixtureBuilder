using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class UnwrapTests
    {
        [TestCase(typeof(int))]
        [TestCase(typeof(string))]
        [TestCase(typeof(int?[]))]
        [TestCase(typeof(List<int?>))]
        public void NeitherByRefNorNullable_ReturnsSameType(Type type)
        {
            var result = type.Unwrap();

            Assert.That(result, Is.EqualTo(type));
        }

        [Test]
        public void NullableValueType_ReturnsUnderlyingType()
        {
            var result = typeof(int?).Unwrap();

            Assert.That(result, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void ByRefValueType_ReturnsElementType()
        {
            var type = typeof(int).MakeByRefType();

            var result = type.Unwrap();

            Assert.That(result, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void ByRefReferenceType_ReturnsElementType()
        {
            var type = typeof(string).MakeByRefType();

            var result = type.Unwrap();

            Assert.That(result, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void ByRefNullableValueType_ReturnsUnderlyingType()
        {
            var type = typeof(int?).MakeByRefType();

            var result = type.Unwrap();

            Assert.That(result, Is.EqualTo(typeof(int)));
        }
    }
}
