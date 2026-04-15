using FixtureBuilder.Assignment.TypeLinks;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactoryTests
{
    internal sealed class AddTypeLinkTests
    {
        public class TestClass { }

        [Test]
        public void AddTypeLink_FixtureUsesAddedLink()
        {
            var factory = new FixtureFactory();
            var targetType = typeof(string);
            var expectedResult = typeof(int);

            var link = new Mock<ITypeLink>();
            link.Setup(x => x.Link(targetType)).Returns(expectedResult);

            factory.AddTypeLink(link.Object);

            var fixture = factory.New<TestClass>();
            var context = TestHelper.GetContext(fixture);
            var result = context.Link(targetType);

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
