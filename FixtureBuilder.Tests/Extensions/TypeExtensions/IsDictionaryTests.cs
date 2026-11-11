using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.Specialized;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class IsDictionaryTests
    {
        [TestCase(typeof(Dictionary<string, int>))]
        [TestCase(typeof(SortedDictionary<string, int>))]
        [TestCase(typeof(ConcurrentDictionary<string, int>))]
        [TestCase(typeof(SortedList<string, int>))]
        [TestCase(typeof(ImmutableDictionary<string, int>))]
        [TestCase(typeof(ImmutableSortedDictionary<string, int>))]
        [TestCase(typeof(Hashtable))]
        [TestCase(typeof(OrderedDictionary))]
        [TestCase(typeof(IDictionary))]
        public void TypeAssignableToIDictionary_ReturnsTrue(Type type)
        {
            var result = type.IsDictionary();

            Assert.That(result, Is.True);
        }

        [TestCase(typeof(IDictionary<string, int>))]
        [TestCase(typeof(IReadOnlyDictionary<string, int>))]
        [TestCase(typeof(IImmutableDictionary<string, int>))]
        [TestCase(typeof(IDictionary<,>))]
        [TestCase(typeof(IReadOnlyDictionary<,>))]
        [TestCase(typeof(IImmutableDictionary<,>))]
        public void GenericDictionaryInterface_ReturnsTrue(Type type)
        {
            var result = type.IsDictionary();

            Assert.That(result, Is.True);
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(HashSet<string>))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(string))]
        [TestCase(typeof(int))]
        [TestCase(typeof(KeyValuePair<string, int>))]
        [TestCase(typeof(IEnumerable<KeyValuePair<string, int>>))]
        public void OtherType_ReturnsFalse(Type type)
        {
            var result = type.IsDictionary();

            Assert.That(result, Is.False);
        }
    }
}
