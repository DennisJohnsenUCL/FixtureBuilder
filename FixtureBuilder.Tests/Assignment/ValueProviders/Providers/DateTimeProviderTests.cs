using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class DateTimeProviderTests
    {
        [Test]
        public void ResolveValue_DateTime_ReturnsUtcNow()
        {
            var provider = new DateTimeProvider();
            var request = TestHelper.MakeRequest(typeof(DateTime));
            var before = DateTime.UtcNow;

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            var after = DateTime.UtcNow;
            var dateTime = (DateTime)result!;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(dateTime.Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(dateTime, Is.InRange(before, after));
            }
        }

        [TestCase(typeof(DateTime?))]
        [TestCase(typeof(DateTimeOffset))]
        [TestCase(typeof(string))]
        public void ResolveValue_OtherType_ReturnsNoResult(Type type)
        {
            var provider = new DateTimeProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
