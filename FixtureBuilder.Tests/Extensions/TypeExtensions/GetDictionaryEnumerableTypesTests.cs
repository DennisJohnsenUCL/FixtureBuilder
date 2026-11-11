using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.Specialized;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class GetDictionaryEnumerableTypesTests
    {
        #region Arguments

        [Test]
        public void NullType_ThrowsArgumentNullException()
        {
            Type type = null!;

            Assert.Throws<ArgumentNullException>(() => type.GetDictionaryEnumerableTypes());
        }

        #endregion

        #region Generic dictionaries

        [TestCase(typeof(Dictionary<string, int>), typeof(string), typeof(int))]
        [TestCase(typeof(SortedDictionary<Guid, DateTime>), typeof(Guid), typeof(DateTime))]
        [TestCase(typeof(ConcurrentDictionary<int, string>), typeof(int), typeof(string))]
        [TestCase(typeof(SortedList<string, double>), typeof(string), typeof(double))]
        [TestCase(typeof(ImmutableDictionary<string, int>), typeof(string), typeof(int))]
        [TestCase(typeof(ImmutableSortedDictionary<int, string>), typeof(int), typeof(string))]
        [TestCase(typeof(IDictionary<string, int>), typeof(string), typeof(int))]
        [TestCase(typeof(IReadOnlyDictionary<long, bool>), typeof(long), typeof(bool))]
        [TestCase(typeof(IImmutableDictionary<string, object>), typeof(string), typeof(object))]
        [TestCase(typeof(Dictionary<string, List<int>>), typeof(string), typeof(List<int>))]
        [TestCase(typeof(Dictionary<string, Dictionary<int, bool>>), typeof(string), typeof(Dictionary<int, bool>))]
        public void GenericDictionary_ReturnsKeyAndValueTypes(Type type, Type expectedKeyType, Type expectedValueType)
        {
            var (keyType, valueType) = type.GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(expectedKeyType));
                Assert.That(valueType, Is.EqualTo(expectedValueType));
            }
        }

        private class StringEnumeratingHashtable : Hashtable, IEnumerable<string>
        {
            IEnumerator<string> IEnumerable<string>.GetEnumerator() => throw new NotSupportedException();
        }

        [Test]
        public void DictionaryNotEnumeratingKeyValuePairs_ReturnsNulls()
        {
            var (keyType, valueType) = typeof(StringEnumeratingHashtable).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.Null);
                Assert.That(valueType, Is.Null);
            }
        }

        #endregion

        #region Non-generic dictionaries

        [Test]
        public void Hashtable_ReturnsObjects()
        {
            var (keyType, valueType) = typeof(Hashtable).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(typeof(object)));
                Assert.That(valueType, Is.EqualTo(typeof(object)));
            }
        }

        [Test]
        public void OrderedDictionary_ReturnsObjects()
        {
            var (keyType, valueType) = typeof(OrderedDictionary).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(typeof(object)));
                Assert.That(valueType, Is.EqualTo(typeof(object)));
            }
        }

        #endregion

        #region Other types

        [Test]
        public void List_ReturnsNulls()
        {
            var (keyType, valueType) = typeof(List<int>).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.Null);
                Assert.That(valueType, Is.Null);
            }
        }

        [Test]
        public void String_ReturnsNulls()
        {
            var (keyType, valueType) = typeof(string).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.Null);
                Assert.That(valueType, Is.Null);
            }
        }

        [Test]
        public void ValueType_ReturnsNulls()
        {
            var (keyType, valueType) = typeof(int).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.Null);
                Assert.That(valueType, Is.Null);
            }
        }

        #endregion
    }
}
