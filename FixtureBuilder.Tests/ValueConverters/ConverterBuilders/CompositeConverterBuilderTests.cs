using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.ValueConverters.ConverterBuilders;
using Moq;

namespace FixtureBuilder.Tests.ValueConverters.ConverterBuilders
{
    internal class CompositeConverterBuilderTests
    {
        private ConverterBuilder _parentBuilder;
        private CompositeConverterBuilder _sut;
        private IFixtureContext _context;

        [SetUp]
        public void SetUp()
        {
            _parentBuilder = new ConverterBuilder();
            _sut = new CompositeConverterBuilder(_parentBuilder);
            _context = Mock.Of<IFixtureContext>();
        }

        #region Constructor

        [Test]
        public void Constructor_NullBuilder_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new CompositeConverterBuilder(null!));

            Assert.That(ex!.ParamName, Is.EqualTo("builder"));
        }

        [Test]
        public void Constructor_ValidBuilder_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new CompositeConverterBuilder(_parentBuilder));
        }

        #endregion

        #region AddEnumerableStrategies

        [Test]
        public void AddEnumerableStrategies_ReturnsSameBuilder()
        {
            var result = _sut.AddEnumerableStrategies();

            Assert.That(result, Is.SameAs(_sut));
        }

        [Test]
        public void AddEnumerableStrategies_And_Build_ConvertsToArray()
        {
            var converter = _sut
                .AddEnumerableStrategies()
                .And()
                .Build();

            var source = new List<int> { 1, 2, 3 };

            var result = converter.Convert(typeof(int[]), source, _context);

            Assert.That(result, Is.InstanceOf<int[]>());
        }

        [Test]
        public void AddEnumerableStrategies_And_Build_ConvertsToImmutableCollection()
        {
            var converter = _sut
                .AddEnumerableStrategies()
                .And()
                .Build();

            var source = new List<string> { "a", "b" };

            var result = converter.Convert(typeof(ImmutableList<string>), source, _context);

            Assert.That(result, Is.InstanceOf<ImmutableList<string>>());
        }

        [Test]
        public void AddEnumerableStrategies_And_Build_ConvertsToNonGenericCollection()
        {
            var converter = _sut
                .AddEnumerableStrategies()
                .And()
                .Build();

            var source = new ArrayList { 1, 2, 3 };

            var result = converter.Convert(typeof(ArrayList), source, _context);

            Assert.That(result, Is.InstanceOf<ArrayList>());
        }

        [Test]
        public void AddEnumerableStrategies_And_Build_ConvertsToBlockingCollection()
        {
            var converter = _sut
                .AddEnumerableStrategies()
                .And()
                .Build();

            var source = new List<int> { 1, 2, 3 };

            var result = converter.Convert(typeof(BlockingCollection<int>), source, _context);

            Assert.That(result, Is.InstanceOf<BlockingCollection<int>>());
        }

        #endregion

        #region AddDictionaryStrategies

        [Test]
        public void AddDictionaryStrategies_ReturnsSameBuilder()
        {
            var result = _sut.AddDictionaryStrategies();

            Assert.That(result, Is.SameAs(_sut));
        }

        [Test]
        public void AddDictionaryStrategies_And_Build_ConvertsToImmutableDictionary()
        {
            var converter = _sut
                .AddDictionaryStrategies()
                .And()
                .Build();

            var source = new Dictionary<string, int> { ["a"] = 1 };

            var result = converter.Convert(typeof(ImmutableDictionary<string, int>), source, _context);

            Assert.That(result, Is.InstanceOf<ImmutableDictionary<string, int>>());
        }

        [Test]
        public void AddDictionaryStrategies_And_Build_ConvertsToFrozenDictionary()
        {
            var converter = _sut
                .AddDictionaryStrategies()
                .And()
                .Build();

            var source = new Dictionary<string, int> { ["a"] = 1 };

            var result = converter.Convert(typeof(FrozenDictionary<string, int>), source, _context);

            Assert.That(result, Is.InstanceOf<FrozenDictionary<string, int>>());
        }

        [Test]
        public void AddDictionaryStrategies_And_Build_ConvertsToNonGenericDictionary()
        {
            var converter = _sut
                .AddDictionaryStrategies()
                .And()
                .Build();

            var source = new Hashtable { ["a"] = 1 };

            var result = converter.Convert(typeof(Hashtable), source, _context);

            Assert.That(result, Is.InstanceOf<Hashtable>());
        }

        #endregion

        #region And

        [Test]
        public void And_ReturnsParentBuilder()
        {
            _sut.AddEnumerableStrategies();

            var result = _sut.And();

            Assert.That(result, Is.SameAs(_parentBuilder));
        }

        [Test]
        public void And_WithBothStrategies_Build_ConvertsBothEnumerableAndDictionaryTypes()
        {
            var converter = _sut
                .AddEnumerableStrategies()
                .AddDictionaryStrategies()
                .And()
                .Build();

            using (Assert.EnterMultipleScope())
            {
                var enumerableResult = converter.Convert(typeof(int[]), new List<int> { 1 }, _context);
                Assert.That(enumerableResult, Is.InstanceOf<int[]>());

                var dictionaryResult = converter.Convert(
                    typeof(ImmutableDictionary<string, int>),
                    new Dictionary<string, int> { ["a"] = 1 },
                    _context);
                Assert.That(dictionaryResult, Is.InstanceOf<ImmutableDictionary<string, int>>());
            }
        }

        #endregion
    }
}
