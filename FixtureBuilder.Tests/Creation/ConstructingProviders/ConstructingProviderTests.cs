using FixtureBuilder.Creation.ConstructingProviders;

namespace FixtureBuilder.Tests.Creation.ConstructingProviders
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
            var result = _sut.ResolveWithArguments(typeof(DefaultCtorClass));

            Assert.That(result, Is.InstanceOf<DefaultCtorClass>());
        }

        [Test]
        public void Resolve_PublicConstructorWithArgs_CreatesInstance()
        {
            var result = (PublicCtorClass)_sut.ResolveWithArguments(typeof(PublicCtorClass), "hello", 42);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("hello"));
                Assert.That(result.Value, Is.EqualTo(42));
            }
        }

        [Test]
        public void Resolve_NonPublicConstructor_CreatesInstance()
        {
            var result = (PrivateCtorClass)_sut.ResolveWithArguments(typeof(PrivateCtorClass), "hidden");

            Assert.That(result.Secret, Is.EqualTo("hidden"));
        }

        [Test]
        public void Resolve_MismatchedArgs_ThrowsInvalidOperationException()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => _sut.ResolveWithArguments(typeof(PublicCtorClass), "only one arg"));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex.Message, Does.Contain("PublicCtorClass"));
                Assert.That(ex.InnerException, Is.Not.Null);
            }
        }

        [Test]
        public void Resolve_AbstractType_ThrowsInvalidOperationException()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => _sut.ResolveWithArguments(typeof(AbstractClass)));

            Assert.That(ex.Message, Does.Contain("AbstractClass"));
        }

        [Test]
        public void Resolve_NullArgWhereExpected_CreatesInstance()
        {
            var result = (PublicCtorClass)_sut.ResolveWithArguments(typeof(PublicCtorClass), null!, 7);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.Null);
                Assert.That(result.Value, Is.EqualTo(7));
            }
        }
    }
}
