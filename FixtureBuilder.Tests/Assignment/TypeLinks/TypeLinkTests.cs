using System.Collections.ObjectModel;
using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.Assignment.TypeLinks
{
    internal sealed class TypeLinkTests
    {
        [Test]
        public void Link_OpenGenericBaseClassLink_ReturnsClosedOutType()
        {
            var link = new TypeLink(typeof(Collection<>), typeof(ObservableCollection<>));
            var request = new TypeRequest(typeof(Collection<int>), typeof(object));

            var result = link.Link(request);

            Assert.That(result, Is.EqualTo(typeof(ObservableCollection<int>)));
        }

        [Test]
        public void Link_OtherType_ReturnsNull()
        {
            var link = new TypeLink(typeof(IMarkerInterface), typeof(MarkerImplementationClass));
            var request = new TypeRequest(typeof(string), typeof(object));

            var result = link.Link(request);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Link_OpenGenericLinkAndNonGenericType_ReturnsNull()
        {
            var link = new TypeLink(typeof(IEnumerable<>), typeof(List<>));
            var request = new TypeRequest(typeof(string), typeof(object));

            var result = link.Link(request);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Link_OpenGenericLinkAndOtherGenericType_ReturnsNull()
        {
            var link = new TypeLink(typeof(IEnumerable<>), typeof(List<>));
            var request = new TypeRequest(typeof(Stack<int>), typeof(object));

            var result = link.Link(request);

            Assert.That(result, Is.Null);
        }
    }
}
