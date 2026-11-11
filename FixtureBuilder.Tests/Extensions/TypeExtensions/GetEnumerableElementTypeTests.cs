using System.Collections;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class GetEnumerableElementTypeTests
    {
        [Test]
        public void NullType_ThrowsArgumentNullException()
        {
            Type type = null!;

            Assert.Throws<ArgumentNullException>(() => type.GetEnumerableElementType());
        }

        private class DoubleEnumerableClass : IEnumerable<double>
        {
            public IEnumerator<double> GetEnumerator() => throw new NotSupportedException();
            IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
        }

        [TestCase(typeof(List<int>), typeof(int))]
        [TestCase(typeof(string[]), typeof(string))]
        [TestCase(typeof(HashSet<Guid>), typeof(Guid))]
        [TestCase(typeof(Dictionary<string, int>), typeof(KeyValuePair<string, int>))]
        [TestCase(typeof(string), typeof(char))]
        [TestCase(typeof(DoubleEnumerableClass), typeof(double))]
        public void GenericEnumerable_ReturnsElementType(Type type, Type expectedElementType)
        {
            var result = type.GetEnumerableElementType();

            Assert.That(result, Is.EqualTo(expectedElementType));
        }

        [Test]
        public void NotEnumerable_ReturnsNull()
        {
            var result = typeof(EmptyClass).GetEnumerableElementType();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void OnlyNonGenericEnumerable_ReturnsNull()
        {
            var result = typeof(ArrayList).GetEnumerableElementType();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ValueType_ReturnsNull()
        {
            var result = typeof(int).GetEnumerableElementType();

            Assert.That(result, Is.Null);
        }
    }
}
