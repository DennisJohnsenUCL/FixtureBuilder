using System.Collections;
using FixtureBuilder.TypeLinks.TypeLinkBuilders;

namespace FixtureBuilder.Tests.TypeLinks.TypeLinkBuilders
{
    internal class CompositeTypeLinkBuilderTests
    {
        private TypeLinkBuilder _parentBuilder;
        private CompositeTypeLinkBuilder _sut;

        [SetUp]
        public void SetUp()
        {
            _parentBuilder = new TypeLinkBuilder();
            _sut = new CompositeTypeLinkBuilder(_parentBuilder);
        }

        #region Constructor

        [Test]
        public void Constructor_NullBuilder_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new CompositeTypeLinkBuilder(null!));

            Assert.That(ex!.ParamName, Is.EqualTo("builder"));
        }

        [Test]
        public void Constructor_ValidBuilder_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new CompositeTypeLinkBuilder(_parentBuilder));
        }

        #endregion

        #region AddEnumerableTypeLinks

        [Test]
        public void AddEnumerableTypeLinks_ReturnsSameBuilder()
        {
            var result = _sut.AddEnumerableTypeLinks();

            Assert.That(result, Is.SameAs(_sut));
        }

        [Test]
        public void AddEnumerableTypeLinks_And_Build_ProducesTypeLinkThatResolvesIEnumerableOfT()
        {
            var result = _sut
                .AddEnumerableTypeLinks()
                .And()
                .Build();

            var resolved = result.Link(typeof(IEnumerable<int>));

            Assert.That(resolved, Is.EqualTo(typeof(List<int>)));
        }

        [Test]
        public void AddEnumerableTypeLinks_And_Build_ProducesTypeLinkThatResolvesIList()
        {
            var result = _sut
                .AddEnumerableTypeLinks()
                .And()
                .Build();

            var resolved = result.Link(typeof(IList));

            Assert.That(resolved, Is.EqualTo(typeof(ArrayList)));
        }

        #endregion

        #region AddDictionaryTypeLinks

        [Test]
        public void AddDictionaryTypeLinks_ReturnsSameBuilder()
        {
            var result = _sut.AddDictionaryTypeLinks();

            Assert.That(result, Is.SameAs(_sut));
        }

        [Test]
        public void AddDictionaryTypeLinks_And_Build_ProducesTypeLinkThatResolvesIDictionaryOfKV()
        {
            var result = _sut
                .AddDictionaryTypeLinks()
                .And()
                .Build();

            var resolved = result.Link(typeof(IDictionary<string, int>));

            Assert.That(resolved, Is.EqualTo(typeof(Dictionary<string, int>)));
        }

        [Test]
        public void AddDictionaryTypeLinks_And_Build_ProducesTypeLinkThatResolvesIDictionary()
        {
            var result = _sut
                .AddDictionaryTypeLinks()
                .And()
                .Build();

            var resolved = result.Link(typeof(IDictionary));

            Assert.That(resolved, Is.EqualTo(typeof(Hashtable)));
        }

        #endregion

        #region And

        [Test]
        public void And_ReturnsParentBuilder()
        {
            _sut.AddEnumerableTypeLinks();

            var result = _sut.And();

            Assert.That(result, Is.SameAs(_parentBuilder));
        }

        [Test]
        public void And_WithBothStrategies_Build_ResolvesEnumerableAndDictionaryTypes()
        {
            var typeLink = _sut
                .AddEnumerableTypeLinks()
                .AddDictionaryTypeLinks()
                .And()
                .Build();

            using (Assert.EnterMultipleScope())
            {
                var enumerableResult = typeLink.Link(typeof(ISet<string>));
                Assert.That(enumerableResult, Is.EqualTo(typeof(HashSet<string>)));

                var dictionaryResult = typeLink.Link(typeof(IDictionary<string, int>));
                Assert.That(dictionaryResult, Is.EqualTo(typeof(Dictionary<string, int>)));
            }
        }

        #endregion
    }
}
