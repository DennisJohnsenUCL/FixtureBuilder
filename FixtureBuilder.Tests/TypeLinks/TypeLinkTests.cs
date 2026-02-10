using FixtureBuilder.TypeLinks;

namespace FixtureBuilder.Tests.TypeLinks
{
    internal sealed class TypeLinkTests
    {
        [Test]
        public void Constructor_InTypeNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new TypeLink(null!, typeof(int)));
        }

        [Test]
        public void Constructor_OutTypeNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new TypeLink(typeof(int), null!));
        }

        [Test]
        public void Constructor_InTypeOpenGeneric_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new TypeLink(typeof(List<>), typeof(string)));
        }

        [Test]
        public void Constructor_OutTypeOpenGeneric_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new TypeLink(typeof(string), typeof(List<>)));
        }

        [Test]
        public void Constructor_Constructs()
        {
            Assert.DoesNotThrow(() => new TypeLink(typeof(int), typeof(long)));
        }

        [Test]
        public void Constructor_InOutTypesOpenGeneric_Constructs()
        {
            Assert.DoesNotThrow(() => new TypeLink(typeof(IEnumerable<>), typeof(List<>)));
        }

        [Test]
        public void Link_TargetNull_ThrowsException()
        {
            var link = new TypeLink(typeof(IEnumerable<>), typeof(List<>));

            Assert.Throws<ArgumentNullException>(() => link.Link(null!));
        }

        [Test]
        public void Link_TargetInType_ReturnsOutType()
        {
            var inType = typeof(int);
            var outType = typeof(long);
            var link = new TypeLink(inType, outType);

            var result = link.Link(inType);

            Assert.That(result, Is.EqualTo(outType));
        }

        [Test]
        public void Link_TargetNotInType_ReturnsNull()
        {
            var inType = typeof(int);
            var outType = typeof(long);
            var link = new TypeLink(inType, outType);

            var result = link.Link(typeof(string));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Link_OpenGenericLink_TargetGenericInType_ReturnsGenericOutType()
        {
            var inType = typeof(IEnumerable<>);
            var outType = typeof(List<>);
            var link = new TypeLink(inType, outType);

            var result = link.Link(inType.MakeGenericType(typeof(string)));

            Assert.That(result, Is.EqualTo(outType.MakeGenericType(typeof(string))));
        }

        [Test]
        public void Link_OpenGenericLink_TargetNotGeneric_ReturnsNull()
        {
            var inType = typeof(IEnumerable<>);
            var outType = typeof(List<>);
            var link = new TypeLink(inType, outType);

            var result = link.Link(typeof(string));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Link_OpenGenericLink_TargetNotGenericInType_ReturnsNull()
        {
            var inType = typeof(IEnumerable<>);
            var outType = typeof(List<>);
            var link = new TypeLink(inType, outType);

            var result = link.Link(typeof(Stack<>));

            Assert.That(result, Is.Null);
        }
    }
}
