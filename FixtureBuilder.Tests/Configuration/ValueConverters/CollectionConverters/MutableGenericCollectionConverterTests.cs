#pragma warning disable CA1861

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using FixtureBuilder.Configuration.ValueConverters.CollectionConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.CollectionConverters
{
    internal sealed class MutableGenericCollectionConverterTests
    {
        private MutableGenericCollectionConverter _sut;
        private IFixtureContext _context;

        [SetUp]
        public void SetUp()
        {
            _sut = new MutableGenericCollectionConverter();
            _context = new Mock<IFixtureContext>().Object;
        }

        [Test]
        public void Constructor_Constructs()
        {
            Assert.DoesNotThrow(() => new MutableGenericCollectionConverter());
        }

        [TestCase(typeof(List<string>), TestName = "Convert_TargetList_Converts")]
        [TestCase(typeof(Queue<string>), TestName = "Convert_TargetQueue_Converts")]
        [TestCase(typeof(SortedSet<string>), TestName = "Convert_TargetSortedSet_Converts")]
        [TestCase(typeof(ReadOnlyCollection<string>), TestName = "Convert_TargetReadOnlyCollection_Converts")]
        [TestCase(typeof(Collection<string>), TestName = "Convert_TargetCollection_Converts")]
        [TestCase(typeof(HashSet<string>), TestName = "Convert_TargetHashSet_Converts")]
        [TestCase(typeof(LinkedList<string>), TestName = "Convert_TargetLinkedList_Converts")]
        public void Convert_OrderPreservingTarget_Converts(Type target)
        {
            var request = new FixtureRequest(target);
            var value = new string[] { "test1", "test2", "test3" };

            var result = _sut.Convert(request, value, _context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.AssignableTo(target));
                Assert.That((IEnumerable)result!, Is.EqualTo(value));
            }
        }

        [TestCase(typeof(Stack<string>), TestName = "Convert_TargetStack_Converts")]
        [TestCase(typeof(ConcurrentStack<string>), TestName = "Convert_TargetConcurrentStack_Converts")]
        [TestCase(typeof(ConcurrentBag<string>), TestName = "Convert_TargetConcurrentBag_Converts")]
        public void Convert_ReverseOrderTarget_Converts(Type target)
        {
            var request = new FixtureRequest(target);
            IEnumerable<string> value = ["test1", "test2", "test3"];

            var result = _sut.Convert(request, value, _context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.AssignableTo(target));
                Assert.That((IEnumerable)result!, Is.EqualTo(value.Reverse()));
            }
        }

        [Test]
        public void Convert_TargetList_ValueNotArray_Converts()
        {
            var request = new FixtureRequest(typeof(List<string>));
            var value = ImmutableList.CreateRange(["test1", "test2", "test3"]);

            var result = _sut.Convert(request, value, _context);

            Assert.That(result, Is.EqualTo(new List<string> { "test1", "test2", "test3" }));
        }

        [Test]
        public void Convert_TargetList_ValueGenericEnumerable_DifferentElementType_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(List<string>));
            var result = _sut.Convert(request, new int[] { 1, 2, 3 }, _context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_TargetNotMutableGenericCollection_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(ImmutableList<string>));
            var result = _sut.Convert(request, new string[] { "test1" }, _context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(List<string>));
            var result = _sut.Convert(request, 42, _context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericEnumerable_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(List<string>));
            var result = _sut.Convert(request, new ArrayList { "test1" }, _context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
