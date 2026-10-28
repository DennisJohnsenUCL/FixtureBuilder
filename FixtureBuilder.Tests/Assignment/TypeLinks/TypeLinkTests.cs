using System.Collections.ObjectModel;
using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.Assignment.TypeLinks
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
            Assert.DoesNotThrow(() => new TypeLink(typeof(IDisposable), typeof(MemoryStream)));
        }

        [Test]
        public void Constructor_InOutTypesOpenGeneric_Constructs()
        {
            Assert.DoesNotThrow(() => new TypeLink(typeof(IEnumerable<>), typeof(List<>)));
        }

        [Test]
        public void Constructor_OpenGenericBaseClass_Constructs()
        {
            Assert.DoesNotThrow(() => new TypeLink(typeof(Collection<>), typeof(ObservableCollection<>)));
        }

        [Test]
        public void Constructor_OutTypeNotAssignable_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new TypeLink(typeof(IDisposable), typeof(string)));
        }

        [Test]
        public void Constructor_OpenGenericOutTypeNotAssignable_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new TypeLink(typeof(ISet<>), typeof(List<>)));
        }

        [Test]
        public void Link_TargetInType_ReturnsOutType()
        {
            var inType = typeof(IDisposable);
            var outType = typeof(MemoryStream);
            var link = new TypeLink(inType, outType);

            var result = link.Link(new TypeRequest(inType, inType));

            Assert.That(result, Is.EqualTo(outType));
        }

        [Test]
        public void Link_TargetNotInType_ReturnsNull()
        {
            var inType = typeof(IDisposable);
            var outType = typeof(MemoryStream);
            var link = new TypeLink(inType, outType);

            var result = link.Link(new TypeRequest(typeof(string), typeof(string)));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Link_OpenGenericLink_TargetGenericInType_ReturnsGenericOutType()
        {
            var inType = typeof(IEnumerable<>);
            var outType = typeof(List<>);
            var link = new TypeLink(inType, outType);

            var target = inType.MakeGenericType(typeof(string));
            var result = link.Link(new TypeRequest(target, target));

            Assert.That(result, Is.EqualTo(outType.MakeGenericType(typeof(string))));
        }

        [Test]
        public void Link_OpenGenericLink_TargetNotGeneric_ReturnsNull()
        {
            var inType = typeof(IEnumerable<>);
            var outType = typeof(List<>);
            var link = new TypeLink(inType, outType);

            var result = link.Link(new TypeRequest(typeof(string), typeof(string)));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Link_OpenGenericLink_TargetNotGenericInType_ReturnsNull()
        {
            var inType = typeof(IEnumerable<>);
            var outType = typeof(List<>);
            var link = new TypeLink(inType, outType);

            var result = link.Link(new TypeRequest(typeof(Stack<>), typeof(Stack<>)));

            Assert.That(result, Is.Null);
        }
    }
}
