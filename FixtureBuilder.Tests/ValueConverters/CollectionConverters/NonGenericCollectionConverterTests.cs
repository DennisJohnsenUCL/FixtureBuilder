using FixtureBuilder.ValueConverters.CollectionConverters;
using System.Collections;

namespace FixtureBuilder.Tests.ValueConverters.CollectionConverters
{
    internal sealed class NonGenericCollectionConverterTests
    {
        [Test]
        public void Constructor_Constructs()
        {
            Assert.DoesNotThrow(() => new MutableGenericCollectionConverter());
        }

        [Test]
        public void Convert_TargetArrayList_ValueEnumerable_Converts()
        {
            var target = typeof(ArrayList);
            var value = new Queue(new ArrayList { "test1", "test2", "test3" });
            var expected = new ArrayList { "test1", "test2", "test3" };

            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetStack_ValueEnumerable_Converts()
        {
            var target = typeof(Stack);
            var value = new ArrayList { "test1", "test2", "test3" };
            var expected = new Stack(new ArrayList { "test1", "test2", "test3" });

            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetQueue_ValueEnumerable_Converts()
        {
            var target = typeof(Queue);
            var value = new ArrayList { "test1", "test2", "test3" };
            var expected = new Queue(new ArrayList { "test1", "test2", "test3" });

            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetArrayList_ValueGenericEnumerable_Converts()
        {
            var target = typeof(ArrayList);
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = new ArrayList { "test1", "test2", "test3" };

            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetArrayList_ValueNotICollection_Converts()
        {
            var target = typeof(ArrayList);
            var value = new HashSet<string> { "test1", "test2", "test3" };
            var expected = new ArrayList { "test1", "test2", "test3" };

            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetNotNonGeneric_ReturnsNull()
        {
            var target = typeof(List<string>);
            var value = new ArrayList { "test1", "test2", "test3" };

            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNull()
        {
            var target = typeof(ArrayList);
            var value = 42;

            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.Null);
        }
    }
}
