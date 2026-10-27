#pragma warning disable CA1861

using System.Collections;
using System.Collections.ObjectModel;
using FixtureBuilder.Configuration.ValueConverters.CollectionConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.CollectionConverters
{
    internal sealed class CollectionConverterTests
    {
        private CollectionConverter _sut;
        private IFixtureContext _context;

        [SetUp]
        public void SetUp()
        {
            _sut = new CollectionConverter();
            _context = new Mock<IFixtureContext>().Object;
        }

        [Test]
        public void Constructor_Constructs()
        {
            Assert.DoesNotThrow(() => new CollectionConverter());
        }

        [TestCase(typeof(ReadOnlyCollection<string>), TestName = "Convert_TargetReadOnlyCollection_Converts")]
        [TestCase(typeof(Collection<string>), TestName = "Convert_TargetCollection_Converts")]
        public void Convert_Target_Converts(Type target)
        {
            var request = new FixtureRequest(target);
            var value = new string[] { "test1", "test2", "test3" };

            var result = _sut.Convert(request, value, _context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf(target));
                Assert.That((IEnumerable)result!, Is.EqualTo(value));
            }
        }

        [TestCase(typeof(ReadOnlyCollection<string>), TestName = "Convert_TargetReadOnlyCollection_ValueNotList_Converts")]
        [TestCase(typeof(Collection<string>), TestName = "Convert_TargetCollection_ValueNotList_Converts")]
        public void Convert_ValueNotList_Converts(Type target)
        {
            var request = new FixtureRequest(target);
            var value = new Queue<string>(["test1", "test2", "test3"]);

            var result = _sut.Convert(request, value, _context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf(target));
                Assert.That((IEnumerable)result!, Is.EqualTo(value));
            }
        }

        [TestCase(typeof(ReadOnlyCollection<string>), TestName = "Convert_TargetReadOnlyCollection_ValueChangedAfterConvert_ResultUnchanged")]
        [TestCase(typeof(Collection<string>), TestName = "Convert_TargetCollection_ValueChangedAfterConvert_ResultUnchanged")]
        public void Convert_ValueChangedAfterConvert_ResultUnchanged(Type target)
        {
            var request = new FixtureRequest(target);
            var value = new List<string> { "test1", "test2", "test3" };

            var result = _sut.Convert(request, value, _context);
            value.Add("test4");

            Assert.That((IEnumerable)result!, Is.EqualTo(["test1", "test2", "test3"]));
        }

        [Test]
        public void Convert_TargetCollection_ValueArray_ResultCanBeAddedTo()
        {
            var request = new FixtureRequest(typeof(Collection<string>));
            var value = new string[] { "test1", "test2", "test3" };

            var result = (Collection<string>)_sut.Convert(request, value, _context)!;

            Assert.DoesNotThrow(() => result.Add("test4"));
            Assert.That(result, Is.EqualTo(["test1", "test2", "test3", "test4"]));
        }

        [Test]
        public void Convert_TargetCollection_ValueGenericEnumerable_DifferentElementType_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(Collection<string>));
            var result = _sut.Convert(request, new int[] { 1, 2, 3 }, _context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_TargetNotCollection_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(List<string>));
            var result = _sut.Convert(request, new string[] { "test1" }, _context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(Collection<string>));
            var result = _sut.Convert(request, 42, _context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericEnumerable_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(Collection<string>));
            var result = _sut.Convert(request, new ArrayList { "test1" }, _context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
