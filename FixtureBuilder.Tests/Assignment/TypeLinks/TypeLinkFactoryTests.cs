using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.Assignment.TypeLinks
{
    internal sealed class TypeLinkFactoryTests
    {
        [TestCase(typeof(IEnumerable<int>), typeof(List<int>))]
        [TestCase(typeof(IList<int>), typeof(List<int>))]
        [TestCase(typeof(IReadOnlyList<int>), typeof(List<int>))]
        [TestCase(typeof(ICollection<int>), typeof(List<int>))]
        [TestCase(typeof(IReadOnlyCollection<int>), typeof(List<int>))]
        [TestCase(typeof(ISet<int>), typeof(HashSet<int>))]
        [TestCase(typeof(IReadOnlySet<int>), typeof(HashSet<int>))]
        [TestCase(typeof(IImmutableList<int>), typeof(ImmutableList<int>))]
        [TestCase(typeof(IImmutableStack<int>), typeof(ImmutableStack<int>))]
        [TestCase(typeof(IImmutableQueue<int>), typeof(ImmutableQueue<int>))]
        [TestCase(typeof(IImmutableSet<int>), typeof(ImmutableHashSet<int>))]
        [TestCase(typeof(IList), typeof(ArrayList))]
        [TestCase(typeof(ICollection), typeof(ArrayList))]
        [TestCase(typeof(IEnumerable), typeof(ArrayList))]
        [TestCase(typeof(IDictionary<string, int>), typeof(Dictionary<string, int>))]
        [TestCase(typeof(IImmutableDictionary<string, int>), typeof(ImmutableDictionary<string, int>))]
        [TestCase(typeof(IReadOnlyDictionary<string, int>), typeof(ReadOnlyDictionary<string, int>))]
        [TestCase(typeof(IDictionary), typeof(Hashtable))]
        public void CreateDefaultTypeLink_LinksCollectionInterfaceToImplementation(Type interfaceType, Type expected)
        {
            var typeLink = TypeLinkFactory.CreateDefaultTypeLink();
            var request = new TypeRequest(interfaceType, typeof(object));

            var result = typeLink.Link(request);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
