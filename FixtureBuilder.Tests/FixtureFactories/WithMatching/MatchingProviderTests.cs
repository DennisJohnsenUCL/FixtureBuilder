using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories.WithMatching;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories.WithMatching
{
    internal sealed class MatchingProviderTests
    {
        private Mock<IFixtureContext> _contextMock;
        private FixtureRequest _request;

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IFixtureContext>();
            _request = new FixtureRequest(typeof(string));
        }

        [Test]
        public void Constructor_NullRules_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new MatchingProvider(null!, "value"));
        }

        [Test]
        public void ResolveValue_AllRulesMatch_ReturnsValue()
        {
            var expected = "matched";
            var rule1 = new Mock<IWithRule>();
            rule1.Setup(r => r.IsMatch(_request)).Returns(true);
            var rule2 = new Mock<IWithRule>();
            rule2.Setup(r => r.IsMatch(_request)).Returns(true);

            var sut = new MatchingProvider([rule1.Object, rule2.Object], expected);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void ResolveValue_OneRuleDoesNotMatch_ReturnsNoResult()
        {
            var rule1 = new Mock<IWithRule>();
            rule1.Setup(r => r.IsMatch(_request)).Returns(true);
            var rule2 = new Mock<IWithRule>();
            rule2.Setup(r => r.IsMatch(_request)).Returns(false);

            var sut = new MatchingProvider([rule1.Object, rule2.Object], "value");

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void ResolveValue_NoRules_ReturnsValue()
        {
            var expected = "matched";
            var sut = new MatchingProvider([], expected);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void ResolveValue_AllRulesMatch_NullValue_ReturnsNull()
        {
            var rule = new Mock<IWithRule>();
            rule.Setup(r => r.IsMatch(_request)).Returns(true);

            var sut = new MatchingProvider([rule.Object], (object?)null);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveValue_NoRulesMatch_ReturnsNoResult()
        {
            var rule = new Mock<IWithRule>();
            rule.Setup(r => r.IsMatch(_request)).Returns(false);

            var sut = new MatchingProvider([rule.Object], "value");

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void ResolveValue_AllRulesMatch_InvokesFunc()
        {
            var expected = "from func";
            var rule = new Mock<IWithRule>();
            rule.Setup(r => r.IsMatch(_request)).Returns(true);

            var sut = new MatchingProvider([rule.Object], () => expected);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void ResolveValue_RulesDoNotMatch_DoesNotInvokeFunc()
        {
            var wasCalled = false;
            var rule = new Mock<IWithRule>();
            rule.Setup(r => r.IsMatch(_request)).Returns(false);

            var sut = new MatchingProvider([rule.Object], () =>
            {
                wasCalled = true;
                return "value";
            });

            sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(wasCalled, Is.False);
        }
    }
}
