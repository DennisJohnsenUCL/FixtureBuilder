using FixtureBuilder.Core;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class AddConverterTests
    {
        [Test]
        public void AddConverter_FixtureUsesAddedConverter()
        {
            var factory = new FixtureFactory();
            var target = typeof(string);
            var value = 42;
            var expected = "converted";

            var converter = new Mock<ICustomConverter>();
            converter.Setup(c => c.Convert(target, value)).Returns(expected);

            factory.AddConverter(converter.Object);

            var fixture = factory.New<TestClass>();
            var context = TestHelper.GetContext(fixture);
            var result = context.Converter.Root.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void AddConverter_NullConverter_Throws()
        {
            var factory = new FixtureFactory();

            Assert.Throws<ArgumentNullException>(() => factory.AddConverter(null!));
        }

        public class TestClass { }
    }
}
