using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories;
using Moq;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactoryTests
{
    internal sealed class PassthroughConfigurationBuilderTests
    {
        private BogusFixtureFactory _factory = null!;

        public class TestClass(string name, ITestInterface child)
        {
            public string Name { get; set; } = name;
            public ITestInterface Child { get; set; } = child;
        }

        [SetUp]
        public void SetUp()
        {
            _factory = FixtureFactory.WithBogus();
        }

        #region Options

        [Test]
        public void Options_Set_UpdatesOptions()
        {
            var options = new FixtureOptions { AllowPrivateConstructors = false };

            _factory.Options = options;

            Assert.That(_factory.Options.AllowPrivateConstructors, Is.False);
        }

        [Test]
        public void SetOptions_Action_UpdatesOptions()
        {
            _factory.SetOptions(o => o.AllowPrivateConstructors = false);

            Assert.That(_factory.Options.AllowPrivateConstructors, Is.False);
        }

        #endregion

        #region AddProvider

        [Test]
        public void AddProvider_ProviderIsUsedByFixture()
        {
            var provider = new Mock<ICustomProvider>();
            provider.Setup(p => p.ResolveValue(It.Is<FixtureRequest>(r => r.Type == typeof(string))))
                .Returns("from-provider");

            _factory.AddProvider(provider.Object);

            var result = _factory.Build<TestClass>();

            Assert.That(result.Name, Is.EqualTo("from-provider"));
        }

        #endregion

        #region AddConverter

        class NeedsConversion
        {
            public int _count;
            public string Count => _count.ToString();
        }

        [Test]
        public void AddConverter_ConverterIsUsedByFixture()
        {
            var converter = new Mock<ICustomConverter>();
            converter.Setup(c => c.Convert(typeof(int), "42")).Returns(42);

            _factory.AddConverter(converter.Object);

            var result = _factory.New<NeedsConversion>()
                .UseAutoConstructor()
                .WithBackingField(x => x.Count, "42")
                .Build();

            Assert.That(result._count, Is.EqualTo(42));
        }

        #endregion

        #region AddTypeLink

        public interface ITestInterface;
        public class TestImplementation : ITestInterface;

        [Test]
        public void AddTypeLink_TypeParams_FixtureUsesLink()
        {
            _factory.AddTypeLink<ITestInterface, TestImplementation>();

            var result = _factory.Build<TestClass>();

            Assert.That(result.Child, Is.InstanceOf<TestImplementation>());
        }

        [Test]
        public void AddTypeLink_TypeArguments_FixtureUsesLink()
        {
#pragma warning disable CA2263 // Prefer generic overload when type is known
            _factory.AddTypeLink(typeof(ITestInterface), typeof(TestImplementation));
#pragma warning restore CA2263 // Prefer generic overload when type is known

            var result = _factory.Build<TestClass>();

            Assert.That(result.Child, Is.InstanceOf<TestImplementation>());
        }

        [Test]
        public void AddTypeLink_CustomTypeLink_FixtureUsesLink()
        {
            var link = new Mock<ICustomTypeLink>();
            link.Setup(l => l.Link(typeof(ITestInterface))).Returns(typeof(TestImplementation));

            _factory.AddTypeLink(link.Object);

            var result = _factory.Build<TestClass>();

            Assert.That(result.Child, Is.InstanceOf<TestImplementation>());
        }

        #endregion
    }
}
