using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.Specialized;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class GetDictionaryEnumerableTypesTests
    {
        [Test]
        public void GetDictionaryEnumerableTypes_NullType_ThrowsArgumentNullException()
        {
            Type type = null!;

            Assert.Throws<ArgumentNullException>(() =>
                type.GetDictionaryEnumerableTypes());
        }

        #region Non-dictionary types return (null, null)

        [Test]
        public void GetDictionaryEnumerableTypes_List_ReturnsNulls()
        {
            var (keyType, valueType) = typeof(List<int>).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.Null);
                Assert.That(valueType, Is.Null);
            }
        }

        [Test]
        public void GetDictionaryEnumerableTypes_String_ReturnsNulls()
        {
            var (keyType, valueType) = typeof(string).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.Null);
                Assert.That(valueType, Is.Null);
            }
        }

        [Test]
        public void GetDictionaryEnumerableTypes_Int_ReturnsNulls()
        {
            var (keyType, valueType) = typeof(int).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.Null);
                Assert.That(valueType, Is.Null);
            }
        }

        #endregion

        #region Non-generic dictionaries return (object, object)

        [Test]
        public void GetDictionaryEnumerableTypes_Hashtable_ReturnsObjectObject()
        {
            var (keyType, valueType) = typeof(Hashtable).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(typeof(object)));
                Assert.That(valueType, Is.EqualTo(typeof(object)));
            }
        }

        [Test]
        public void GetDictionaryEnumerableTypes_OrderedDictionary_ReturnsObjectObject()
        {
            var (keyType, valueType) = typeof(OrderedDictionary).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(typeof(object)));
                Assert.That(valueType, Is.EqualTo(typeof(object)));
            }
        }

        #endregion

        #region Generic dictionary concrete types

        [Test]
        public void GetDictionaryEnumerableTypes_DictionaryStringInt_ReturnsStringInt()
        {
            var (keyType, valueType) = typeof(Dictionary<string, int>).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(typeof(string)));
                Assert.That(valueType, Is.EqualTo(typeof(int)));
            }
        }

        [Test]
        public void GetDictionaryEnumerableTypes_SortedDictionary_ReturnsCorrectTypes()
        {
            var (keyType, valueType) = typeof(SortedDictionary<Guid, DateTime>).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(typeof(Guid)));
                Assert.That(valueType, Is.EqualTo(typeof(DateTime)));
            }
        }

        [Test]
        public void GetDictionaryEnumerableTypes_ConcurrentDictionary_ReturnsCorrectTypes()
        {
            var (keyType, valueType) = typeof(ConcurrentDictionary<int, string>).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(typeof(int)));
                Assert.That(valueType, Is.EqualTo(typeof(string)));
            }
        }

        [Test]
        public void GetDictionaryEnumerableTypes_SortedList_ReturnsCorrectTypes()
        {
            var (keyType, valueType) = typeof(SortedList<string, double>).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(typeof(string)));
                Assert.That(valueType, Is.EqualTo(typeof(double)));
            }
        }

        #endregion

        #region Generic dictionary interfaces

        [Test]
        public void GetDictionaryEnumerableTypes_IDictionaryInterface_ReturnsCorrectTypes()
        {
            var (keyType, valueType) = typeof(IDictionary<string, int>).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(typeof(string)));
                Assert.That(valueType, Is.EqualTo(typeof(int)));
            }
        }

        [Test]
        public void GetDictionaryEnumerableTypes_IReadOnlyDictionaryInterface_ReturnsCorrectTypes()
        {
            var (keyType, valueType) = typeof(IReadOnlyDictionary<long, bool>).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(typeof(long)));
                Assert.That(valueType, Is.EqualTo(typeof(bool)));
            }
        }

        [Test]
        public void GetDictionaryEnumerableTypes_IImmutableDictionaryInterface_ReturnsCorrectTypes()
        {
            var (keyType, valueType) = typeof(IImmutableDictionary<string, object>).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(typeof(string)));
                Assert.That(valueType, Is.EqualTo(typeof(object)));
            }
        }

        #endregion

        #region Immutable dictionary implementations

        [Test]
        public void GetDictionaryEnumerableTypes_ImmutableDictionary_ReturnsCorrectTypes()
        {
            var (keyType, valueType) = typeof(ImmutableDictionary<string, int>).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(typeof(string)));
                Assert.That(valueType, Is.EqualTo(typeof(int)));
            }
        }

        [Test]
        public void GetDictionaryEnumerableTypes_ImmutableSortedDictionary_ReturnsCorrectTypes()
        {
            var (keyType, valueType) = typeof(ImmutableSortedDictionary<int, string>).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(typeof(int)));
                Assert.That(valueType, Is.EqualTo(typeof(string)));
            }
        }

        #endregion

        #region Nested generic types

        [Test]
        public void GetDictionaryEnumerableTypes_NestedGenericValue_ReturnsCorrectTypes()
        {
            var (keyType, valueType) = typeof(Dictionary<string, List<int>>).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(typeof(string)));
                Assert.That(valueType, Is.EqualTo(typeof(List<int>)));
            }
        }

        [Test]
        public void GetDictionaryEnumerableTypes_DictionaryOfDictionaries_ReturnsCorrectTypes()
        {
            var (keyType, valueType) = typeof(Dictionary<string, Dictionary<int, bool>>).GetDictionaryEnumerableTypes();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(keyType, Is.EqualTo(typeof(string)));
                Assert.That(valueType, Is.EqualTo(typeof(Dictionary<int, bool>)));
            }
        }

        #endregion
    }
}
