using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.Specialized;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class IsDictionaryTests
    {
        [Test]
        public void IsDictionary_NullType_ThrowsArgumentNullException()
        {
            Type type = null!;

            Assert.Throws<ArgumentNullException>(() =>
                type.IsDictionary());
        }

        #region Concrete dictionary types (via IDictionary assignability)

        [Test]
        public void IsDictionary_Dictionary_ReturnsTrue()
        {
            Assert.That(typeof(Dictionary<string, int>).IsDictionary(), Is.True);
        }

        [Test]
        public void IsDictionary_SortedDictionary_ReturnsTrue()
        {
            Assert.That(typeof(SortedDictionary<string, int>).IsDictionary(), Is.True);
        }

        [Test]
        public void IsDictionary_ConcurrentDictionary_ReturnsTrue()
        {
            Assert.That(typeof(ConcurrentDictionary<string, int>).IsDictionary(), Is.True);
        }

        [Test]
        public void IsDictionary_Hashtable_ReturnsTrue()
        {
            Assert.That(typeof(Hashtable).IsDictionary(), Is.True);
        }

        [Test]
        public void IsDictionary_SortedList_ReturnsTrue()
        {
            Assert.That(typeof(SortedList<string, int>).IsDictionary(), Is.True);
        }

        [Test]
        public void IsDictionary_OrderedDictionary_ReturnsTrue()
        {
            Assert.That(typeof(OrderedDictionary).IsDictionary(), Is.True);
        }

        #endregion

        #region Generic dictionary interfaces

        [Test]
        public void IsDictionary_IDictionaryInterface_ReturnsTrue()
        {
            Assert.That(typeof(IDictionary<string, int>).IsDictionary(), Is.True);
        }

        [Test]
        public void IsDictionary_IReadOnlyDictionaryInterface_ReturnsTrue()
        {
            Assert.That(typeof(IReadOnlyDictionary<string, int>).IsDictionary(), Is.True);
        }

        [Test]
        public void IsDictionary_IImmutableDictionaryInterface_ReturnsTrue()
        {
            Assert.That(typeof(IImmutableDictionary<string, int>).IsDictionary(), Is.True);
        }

        #endregion

        #region Open generic dictionary interfaces

        [Test]
        public void IsDictionary_OpenIDictionary_ReturnsTrue()
        {
            Assert.That(typeof(IDictionary<,>).IsDictionary(), Is.True);
        }

        [Test]
        public void IsDictionary_OpenIReadOnlyDictionary_ReturnsTrue()
        {
            Assert.That(typeof(IReadOnlyDictionary<,>).IsDictionary(), Is.True);
        }

        [Test]
        public void IsDictionary_OpenIImmutableDictionary_ReturnsTrue()
        {
            Assert.That(typeof(IImmutableDictionary<,>).IsDictionary(), Is.True);
        }

        #endregion

        #region Non-generic IDictionary interface

        [Test]
        public void IsDictionary_IDictionaryNonGenericInterface_ReturnsTrue()
        {
            Assert.That(typeof(IDictionary).IsDictionary(), Is.True);
        }

        #endregion

        #region Immutable dictionary implementations

        [Test]
        public void IsDictionary_ImmutableDictionary_ReturnsTrue()
        {
            Assert.That(typeof(ImmutableDictionary<string, int>).IsDictionary(), Is.True);
        }

        [Test]
        public void IsDictionary_ImmutableSortedDictionary_ReturnsTrue()
        {
            Assert.That(typeof(ImmutableSortedDictionary<string, int>).IsDictionary(), Is.True);
        }

        #endregion

        #region Non-dictionary types

        [Test]
        public void IsDictionary_List_ReturnsFalse()
        {
            Assert.That(typeof(List<int>).IsDictionary(), Is.False);
        }

        [Test]
        public void IsDictionary_String_ReturnsFalse()
        {
            Assert.That(typeof(string).IsDictionary(), Is.False);
        }

        [Test]
        public void IsDictionary_Int_ReturnsFalse()
        {
            Assert.That(typeof(int).IsDictionary(), Is.False);
        }

        [Test]
        public void IsDictionary_HashSet_ReturnsFalse()
        {
            Assert.That(typeof(HashSet<string>).IsDictionary(), Is.False);
        }

        [Test]
        public void IsDictionary_Array_ReturnsFalse()
        {
            Assert.That(typeof(int[]).IsDictionary(), Is.False);
        }

        [Test]
        public void IsDictionary_KeyValuePair_ReturnsFalse()
        {
            Assert.That(typeof(KeyValuePair<string, int>).IsDictionary(), Is.False);
        }

        [Test]
        public void IsDictionary_IEnumerableOfKeyValuePair_ReturnsFalse()
        {
            Assert.That(typeof(IEnumerable<KeyValuePair<string, int>>).IsDictionary(), Is.False);
        }

        #endregion
    }
}
