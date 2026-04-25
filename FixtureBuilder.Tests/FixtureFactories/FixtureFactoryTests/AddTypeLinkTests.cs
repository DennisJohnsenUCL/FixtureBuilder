using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Core;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class AddTypeLinkTests
    {
        public class TestClass { }
        public interface ITestInterface { }
        public class TestImplementation : ITestInterface { }

        [Test]
        public void AddTypeLink_FixtureUsesAddedLink()
        {
            var factory = new FixtureFactory();
            var request = new FixtureRequest(typeof(string));
            var expectedResult = typeof(int);

            var link = new Mock<ITypeLink>();
            link.Setup(x => x.Link(It.Is<FixtureRequest>(r => r.Type == typeof(string)))).Returns(expectedResult);

            factory.AddTypeLink(link.Object);

            var fixture = factory.New<TestClass>();
            var context = TestHelper.GetContext(fixture);
            var result = context.TypeLink.Link(request);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void AddTypeLink_TypeParams_LinksInputToOutput()
        {
            var factory = new FixtureFactory();

            factory.AddTypeLink<ITestInterface, TestImplementation>();

            var fixture = factory.New<TestClass>();
            var context = TestHelper.GetContext(fixture);
            var result = context.TypeLink.Link(new FixtureRequest(typeof(ITestInterface)));

            Assert.That(result, Is.EqualTo(typeof(TestImplementation)));
        }

        [Test]
        public void AddTypeLink_TypeArguments_LinksInputToOutput()
        {
            var factory = new FixtureFactory();

#pragma warning disable CA2263 // Prefer generic overload when type is known
            factory.AddTypeLink(typeof(ITestInterface), typeof(TestImplementation));
#pragma warning restore CA2263 // Prefer generic overload when type is known

            var fixture = factory.New<TestClass>();
            var context = TestHelper.GetContext(fixture);
            var result = context.TypeLink.Link(new FixtureRequest(typeof(ITestInterface)));

            Assert.That(result, Is.EqualTo(typeof(TestImplementation)));
        }
    }
}
