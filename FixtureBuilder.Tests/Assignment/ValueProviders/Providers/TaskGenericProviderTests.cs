using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class TaskGenericProviderTests
    {
        [Test]
        public void ResolveValue_TaskOfValueType_ReturnsCompletedTaskWithDefault()
        {
            var provider = new TaskGenericProvider();
            var request = TestHelper.MakeRequest(typeof(Task<int>));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            var task = (Task<int>)result!;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(task.IsCompletedSuccessfully, Is.True);
                Assert.That(task.Result, Is.Zero);
            }
        }

        [Test]
        public void ResolveValue_TaskOfReferenceType_ReturnsCompletedTaskWithNull()
        {
            var provider = new TaskGenericProvider();
            var request = TestHelper.MakeRequest(typeof(Task<string>));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            var task = (Task<string>)result!;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(task.IsCompletedSuccessfully, Is.True);
                Assert.That(task.Result, Is.Null);
            }
        }

        [TestCase(typeof(Task))]
        [TestCase(typeof(ValueTask<int>))]
        [TestCase(typeof(List<int>))]
        public void ResolveValue_OtherType_ReturnsNoResult(Type type)
        {
            var provider = new TaskGenericProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
