using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class ConstructibleEnumerableProviderTests
    {
        [TestCase(typeof(List<int>))]
        [TestCase(typeof(Stack<string>))]
        [TestCase(typeof(Queue<double>))]
        [TestCase(typeof(SortedSet<int>))]
        [TestCase(typeof(HashSet<int>))]
        [TestCase(typeof(LinkedList<int>))]
        [TestCase(typeof(Collection<int>))]
        [TestCase(typeof(ConcurrentBag<int>))]
        [TestCase(typeof(ConcurrentQueue<int>))]
        [TestCase(typeof(ConcurrentStack<int>))]
        [TestCase(typeof(Dictionary<string, int>))]
        [TestCase(typeof(ConcurrentDictionary<string, int>))]
        [TestCase(typeof(OrderedDictionary<string, int>))]
        [TestCase(typeof(SortedDictionary<string, int>))]
        [TestCase(typeof(SortedList<string, int>))]
        [TestCase(typeof(ArrayList))]
        [TestCase(typeof(Stack))]
        [TestCase(typeof(Queue))]
        [TestCase(typeof(Hashtable))]
        [TestCase(typeof(SortedList))]
        public void ResolveValue_SupportedCollectionType_ReturnsEmptyInstance(Type collectionType)
        {
            var provider = new ConstructibleEnumerableProvider();
            var request = TestHelper.MakeRequest(collectionType);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            var collection = (IEnumerable)result!;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(collection, Is.TypeOf(collectionType));
                Assert.That(collection, Is.Empty);
            }
        }

        [TestCase(typeof(ImmutableList<int>))]
        [TestCase(typeof(IList<int>))]
        [TestCase(typeof(string))]
        public void ResolveValue_OtherType_ReturnsNoResult(Type type)
        {
            var provider = new ConstructibleEnumerableProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
