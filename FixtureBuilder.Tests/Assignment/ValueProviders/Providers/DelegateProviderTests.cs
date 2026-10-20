using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class DelegateProviderTests
    {
        private DelegateProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        private delegate bool TryParse(string text, out int value);
        private delegate ref int RefReturn();

        [SetUp]
        public void SetUp()
        {
            _sut = new DelegateProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new DelegateProvider());
        }

        [Test]
        public void Resolve_FuncWithValueTypeReturn_ReturnsDelegateReturningDefault()
        {
            var request = new FixtureRequest(typeof(Func<string, int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<Func<string, int>>());
            Assert.That(((Func<string, int>)result!)("text"), Is.Zero);
        }

        [Test]
        public void Resolve_FuncWithReferenceTypeReturn_ReturnsDelegateReturningNull()
        {
            var request = new FixtureRequest(typeof(Func<string>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<Func<string>>());
            Assert.That(((Func<string>)result!)(), Is.Null);
        }

        [Test]
        public void Resolve_Action_ReturnsInvokableDelegate()
        {
            var request = new FixtureRequest(typeof(Action<int, string>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<Action<int, string>>());
            Assert.DoesNotThrow(() => ((Action<int, string>)result!)(1, "text"));
        }

        [Test]
        public void Resolve_DelegateWithOutParameter_LeavesOutParameterDefault()
        {
            var request = new FixtureRequest(typeof(TryParse));

            var result = (TryParse)_sut.ResolveValue(request, _contextMock.Object)!;
            var returned = result("1", out var value);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(returned, Is.False);
                Assert.That(value, Is.Zero);
            }
        }

        [Test]
        public void Resolve_RefReturnDelegate_ThrowsInvalidOperationException()
        {
            var request = new FixtureRequest(typeof(RefReturn));

            var ex = Assert.Throws<InvalidOperationException>(() => _sut.ResolveValue(request, _contextMock.Object));

            Assert.That(ex!.Message, Does.Contain(nameof(RefReturn)));
        }

        [TestCase(typeof(Delegate), TestName = "Resolve_DelegateBaseType_ReturnsNoResult")]
        [TestCase(typeof(MulticastDelegate), TestName = "Resolve_MulticastDelegateBaseType_ReturnsNoResult")]
        [TestCase(typeof(string), TestName = "Resolve_String_ReturnsNoResult")]
        public void Resolve_NonDelegateOrAbstractDelegate_ReturnsNoResult(Type type)
        {
            var request = new FixtureRequest(type);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
