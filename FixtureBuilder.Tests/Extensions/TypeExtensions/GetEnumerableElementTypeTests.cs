using System.Collections;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal class GetEnumerableElementTypeTests
    {
        #region Test helpers

        private class CustomEnumerable : IEnumerable<double>
        {
            public IEnumerator<double> GetEnumerator() => throw new NotImplementedException();
            IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        }

        private class NotEnumerable { }

        #endregion

        [Test]
        public void GetEnumerableElementType_NullType_ThrowsArgumentNullException()
        {
            Type type = null!;

            Assert.Throws<ArgumentNullException>(() =>
                type.GetEnumerableElementType());
        }

        [Test]
        public void GetEnumerableElementType_ListOfInt_ReturnsInt()
        {
            var result = typeof(List<int>).GetEnumerableElementType();

            Assert.That(result, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void GetEnumerableElementType_StringArray_ReturnsString()
        {
            var result = typeof(string[]).GetEnumerableElementType();

            Assert.That(result, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void GetEnumerableElementType_HashSetOfGuid_ReturnsGuid()
        {
            var result = typeof(HashSet<Guid>).GetEnumerableElementType();

            Assert.That(result, Is.EqualTo(typeof(Guid)));
        }

        [Test]
        public void GetEnumerableElementType_Dictionary_ReturnsKeyValuePair()
        {
            var result = typeof(Dictionary<string, int>).GetEnumerableElementType();

            Assert.That(result, Is.EqualTo(typeof(KeyValuePair<string, int>)));
        }

        [Test]
        public void GetEnumerableElementType_String_ReturnsChar()
        {
            var result = typeof(string).GetEnumerableElementType();

            Assert.That(result, Is.EqualTo(typeof(char)));
        }

        [Test]
        public void GetEnumerableElementType_CustomEnumerable_ReturnsDouble()
        {
            var result = typeof(CustomEnumerable).GetEnumerableElementType();

            Assert.That(result, Is.EqualTo(typeof(double)));
        }

        [Test]
        public void GetEnumerableElementType_NonEnumerableType_ReturnsNull()
        {
            var result = typeof(NotEnumerable).GetEnumerableElementType();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetEnumerableElementType_NonGenericEnumerableOnly_ReturnsNull()
        {
            var result = typeof(IEnumerable).GetEnumerableElementType();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetEnumerableElementType_PlainValueType_ReturnsNull()
        {
            var result = typeof(int).GetEnumerableElementType();

            Assert.That(result, Is.Null);
        }
    }
}
