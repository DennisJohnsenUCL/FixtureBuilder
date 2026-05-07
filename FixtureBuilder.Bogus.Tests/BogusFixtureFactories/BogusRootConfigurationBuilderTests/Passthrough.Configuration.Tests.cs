using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories;
using Moq;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories.BogusRootConfigurationBuilderTests
{
    internal sealed class PassthroughConfigurationBuilderTests
    {
        private BogusFixtureFactory _factory = null!;

        public class RootClass(string name)
        {
            public string Name { get; } = name;
        }

        public class OtherClass(string name)
        {
            public string Name { get; } = name;
        }

        public interface ITestInterface;
        public class TestImplementation : ITestInterface;

        public class RootWithInterface(ITestInterface child)
        {
            public ITestInterface Child { get; } = child;
        }

        [SetUp]
        public void SetUp()
        {
            _factory = FixtureFactory.WithBogus();
        }

        #region Options

        public class PrivateCtorClass
        {
            private PrivateCtorClass() { }
        }

        [Test]
        public void Options_Set_AppliesOptions()
        {
            _factory.WhenBuilding<PrivateCtorClass>(b =>
            {
                b.Options = new FixtureOptions { AllowPrivateConstructors = false };
            });

            Assert.Throws<InvalidOperationException>(() => _factory.Build<PrivateCtorClass>());
        }

        [Test]
        public void SetOptions_Action_AppliesOptions()
        {
            _factory.WhenBuilding<PrivateCtorClass>(b =>
            {
                b.SetOptions(o => o.AllowPrivateConstructors = false);
            });

            Assert.Throws<InvalidOperationException>(() => _factory.Build<PrivateCtorClass>());
        }

        #endregion

        #region AddProvider

        [Test]
        public void AddProvider_ProviderIsUsedByFixture()
        {
            var provider = new Mock<ICustomProvider>();
            provider.Setup(p => p.ResolveValue(It.Is<FixtureRequest>(r => r.Type == typeof(string))))
                .Returns("from-provider");

            _factory.WhenBuilding<RootClass>(b => b.AddProvider(provider.Object));

            var result = _factory.Build<RootClass>();

            Assert.That(result.Name, Is.EqualTo("from-provider"));
        }

        [Test]
        public void AddProvider_ScopedToRoot_DoesNotAffectOtherTypes()
        {
            var provider = new Mock<ICustomProvider>();
            provider.Setup(p => p.ResolveValue(It.Is<FixtureRequest>(r => r.Type == typeof(string))))
                .Returns("from-provider");

            _factory.WhenBuilding<RootClass>(b => b.AddProvider(provider.Object));

            var result = _factory.Build<OtherClass>();

            Assert.That(result.Name, Is.Not.EqualTo("from-provider"));
        }

        #endregion

        #region AddConverter

        [Test]
        public void AddConverter_DoesNotThrow()
        {
            var converter = new Mock<ICustomConverter>();

            Assert.DoesNotThrow(() => _factory.WhenBuilding<RootClass>(b => b.AddConverter(converter.Object)));
        }

        #endregion

        #region AddTypeLink

        [Test]
        public void AddTypeLink_TypeParams_FixtureUsesLink()
        {
            _factory.WhenBuilding<RootWithInterface>(b => b.AddTypeLink<ITestInterface, TestImplementation>());

            var result = _factory.Build<RootWithInterface>();

            Assert.That(result.Child, Is.InstanceOf<TestImplementation>());
        }

        [Test]
        public void AddTypeLink_TypeArguments_FixtureUsesLink()
        {
#pragma warning disable CA2263 // Prefer generic overload when type is known
            _factory.WhenBuilding<RootWithInterface>(b => b.AddTypeLink(typeof(ITestInterface), typeof(TestImplementation)));
#pragma warning restore CA2263 // Prefer generic overload when type is known

            var result = _factory.Build<RootWithInterface>();

            Assert.That(result.Child, Is.InstanceOf<TestImplementation>());
        }

        [Test]
        public void AddTypeLink_CustomTypeLink_FixtureUsesLink()
        {
            var link = new Mock<ICustomTypeLink>();
            link.Setup(l => l.Link(typeof(ITestInterface))).Returns(typeof(TestImplementation));

            _factory.WhenBuilding<RootWithInterface>(b => b.AddTypeLink(link.Object));

            var result = _factory.Build<RootWithInterface>();

            Assert.That(result.Child, Is.InstanceOf<TestImplementation>());
        }

        #endregion
    }
}
