using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class TaskProviderTests
    {
        [Test]
        public void ResolveValue_Task_ReturnsCompletedTask()
        {
            var provider = new TaskProvider();
            var request = TestHelper.MakeRequest(typeof(Task));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            var task = (Task)result!;
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }

        [TestCase(typeof(Task<int>))]
        [TestCase(typeof(ValueTask))]
        [TestCase(typeof(string))]
        public void ResolveValue_OtherType_ReturnsNoResult(Type type)
        {
            var provider = new TaskProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
