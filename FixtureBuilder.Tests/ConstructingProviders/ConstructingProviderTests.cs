using FixtureBuilder.ConstructingProviders;

namespace FixtureBuilder.Tests.Constructors
{
    internal sealed class ConstructingProviderTests
    {
        private ConstructingProvider _sut;

        private class DefaultCtorClass { }

        private class PublicCtorClass(string name, int value)
        {
            public string Name { get; } = name;
            public int Value { get; } = value;
        }

        private class PrivateCtorClass
        {
            public string Secret { get; }

            private PrivateCtorClass(string secret)
            {
                Secret = secret;
            }
        }

        private abstract class AbstractClass { }

        [SetUp]
        public void SetUp()
        {
            _sut = new ConstructingProvider();
        }

        [Test]
        public void DefaultConstructor_CreatesInstance()
        {
            var sut = new ConstructingProvider();

            Assert.That(sut, Is.Not.Null);
        }

        [Test]
        public void Resolve_DefaultConstructor_CreatesInstance()
        {
            var request = new FixtureRequest(typeof(DefaultCtorClass));

            var result = _sut.ResolveWithArguments(request);

            Assert.That(result, Is.InstanceOf<DefaultCtorClass>());
        }

        [Test]
        public void Resolve_PublicConstructorWithArgs_CreatesInstance()
        {
            var request = new FixtureRequest(typeof(PublicCtorClass));

            var result = (PublicCtorClass)_sut.ResolveWithArguments(request, "hello", 42);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("hello"));
                Assert.That(result.Value, Is.EqualTo(42));
            }
        }

        [Test]
        public void Resolve_NonPublicConstructor_CreatesInstance()
        {
            var request = new FixtureRequest(typeof(PrivateCtorClass));

            var result = (PrivateCtorClass)_sut.ResolveWithArguments(request, "hidden");

            Assert.That(result.Secret, Is.EqualTo("hidden"));
        }

        [Test]
        public void Resolve_MismatchedArgs_ThrowsInvalidOperationException()
        {
            var request = new FixtureRequest(typeof(PublicCtorClass));

            var ex = Assert.Throws<InvalidOperationException>(
                () => _sut.ResolveWithArguments(request, "only one arg"));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex.Message, Does.Contain("PublicCtorClass"));
                Assert.That(ex.InnerException, Is.Not.Null);
            }
        }

        [Test]
        public void Resolve_AbstractType_ThrowsInvalidOperationException()
        {
            var request = new FixtureRequest(typeof(AbstractClass));

            var ex = Assert.Throws<InvalidOperationException>(
                () => _sut.ResolveWithArguments(request));

            Assert.That(ex.Message, Does.Contain("AbstractClass"));
        }

        [Test]
        public void Resolve_NullArgWhereExpected_CreatesInstance()
        {
            var request = new FixtureRequest(typeof(PublicCtorClass));

            var result = (PublicCtorClass)_sut.ResolveWithArguments(request, null!, 7);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.Null);
                Assert.That(result.Value, Is.EqualTo(7));
            }
        }
    }
}
