using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters
{
    internal sealed class ConverterFactoryTests
    {
        [TestCase(typeof(List<string>))]
        [TestCase(typeof(Stack<string>))]
        [TestCase(typeof(Queue<string>))]
        [TestCase(typeof(SortedSet<string>))]
        [TestCase(typeof(HashSet<string>))]
        [TestCase(typeof(LinkedList<string>))]
        [TestCase(typeof(ConcurrentBag<string>))]
        [TestCase(typeof(ConcurrentQueue<string>))]
        [TestCase(typeof(ConcurrentStack<string>))]
        [TestCase(typeof(Collection<string>))]
        [TestCase(typeof(ReadOnlyCollection<string>))]
        [TestCase(typeof(ImmutableList<string>))]
        [TestCase(typeof(ImmutableHashSet<string>))]
        [TestCase(typeof(ImmutableStack<string>))]
        [TestCase(typeof(ImmutableQueue<string>))]
        [TestCase(typeof(ImmutableArray<string>))]
        [TestCase(typeof(ImmutableSortedSet<string>))]
        [TestCase(typeof(FrozenSet<string>))]
        [TestCase(typeof(ArrayList))]
        [TestCase(typeof(Stack))]
        [TestCase(typeof(Queue))]
        [TestCase(typeof(BlockingCollection<string>))]
        public void CreateDefaultConverter_ConvertsArrayToCollectionType(Type collectionType)
        {
            var converter = ConverterFactory.CreateDefaultConverter();
            var request = new TypeRequest(collectionType, typeof(object));
            string[] value = ["value"];

            var result = converter.Convert(request, value, collectionType, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.InstanceOf(collectionType));
                Assert.That(result, Is.EqualTo(["value"]));
            }
        }

        [Test]
        public void CreateDefaultConverter_ConvertsListToArray()
        {
            var converter = ConverterFactory.CreateDefaultConverter();
            var request = new TypeRequest(typeof(string[]), typeof(object));
            List<string> value = ["value"];

            var result = converter.Convert(request, value, typeof(string[]), Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.InstanceOf<string[]>());
                Assert.That(result, Is.EqualTo(["value"]));
            }
        }

        [TestCase(typeof(Dictionary<string, int>))]
        [TestCase(typeof(ConcurrentDictionary<string, int>))]
        [TestCase(typeof(OrderedDictionary<string, int>))]
        [TestCase(typeof(ImmutableDictionary<string, int>))]
        [TestCase(typeof(ImmutableSortedDictionary<string, int>))]
        [TestCase(typeof(FrozenDictionary<string, int>))]
        [TestCase(typeof(ReadOnlyDictionary<string, int>))]
        [TestCase(typeof(SortedDictionary<string, int>))]
        [TestCase(typeof(SortedList<string, int>))]
        [TestCase(typeof(Hashtable))]
        [TestCase(typeof(SortedList))]
        public void CreateDefaultConverter_ConvertsKeyValuePairsToDictionaryType(Type dictionaryType)
        {
            var converter = ConverterFactory.CreateDefaultConverter();
            var request = new TypeRequest(dictionaryType, typeof(object));
            List<KeyValuePair<string, int>> value = [new("one", 1)];

            var result = converter.Convert(request, value, dictionaryType, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            var dictionary = (IDictionary)result!;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(dictionary, Is.InstanceOf(dictionaryType));
                Assert.That(dictionary["one"], Is.EqualTo(1));
            }
        }

        [Test]
        public void CreateDefaultConverter_ImplicitConversionAllowed_ConvertsWithImplicitOperator()
        {
            var converter = ConverterFactory.CreateDefaultConverter();
            var request = new TypeRequest(typeof(ImplicitTargetStruct), typeof(object));
            var value = new ImplicitSourceStruct { Number = 42 };
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, value, typeof(ImplicitTargetStruct), Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            var converted = (ImplicitTargetStruct)result!;
            Assert.That(converted.Number, Is.EqualTo(42));
        }
    }
}
