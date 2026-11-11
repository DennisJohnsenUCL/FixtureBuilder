using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class ImmutableFrozenEnumerableProviderTests
    {
        [TestCase(typeof(ImmutableList<int>))]
        [TestCase(typeof(ReadOnlyCollection<int>))]
        [TestCase(typeof(ImmutableHashSet<string>))]
        [TestCase(typeof(ImmutableSortedSet<int>))]
        [TestCase(typeof(ImmutableStack<int>))]
        [TestCase(typeof(ImmutableQueue<int>))]
        [TestCase(typeof(ImmutableArray<int>))]
        [TestCase(typeof(ImmutableDictionary<string, int>))]
        [TestCase(typeof(ReadOnlyDictionary<string, int>))]
        [TestCase(typeof(ImmutableSortedDictionary<string, int>))]
        public void ResolveValue_ImmutableCollectionType_ReturnsEmptyInstanceOfThatType(Type collectionType)
        {
            var provider = new ImmutableFrozenEnumerableProvider();
            var request = TestHelper.MakeRequest(collectionType);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            var collection = (IEnumerable)result!;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(collection, Is.TypeOf(collectionType));
                Assert.That(collection, Is.Empty);
            }
        }

        [TestCase(typeof(FrozenSet<int>))]
        [TestCase(typeof(FrozenDictionary<string, int>))]
        public void ResolveValue_FrozenCollectionType_ReturnsEmptyInstanceAssignableToThatType(Type collectionType)
        {
            var provider = new ImmutableFrozenEnumerableProvider();
            var request = TestHelper.MakeRequest(collectionType);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            var collection = (IEnumerable)result!;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(collection, Is.AssignableTo(collectionType));
                Assert.That(collection, Is.Empty);
            }
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(string))]
        [TestCase(typeof(int))]
        public void ResolveValue_OtherType_ReturnsNoResult(Type type)
        {
            var provider = new ImmutableFrozenEnumerableProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
