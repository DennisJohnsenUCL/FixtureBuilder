using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class TaskGenericProviderTests
    {
        private TaskGenericProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _sut = new TaskGenericProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new TaskGenericProvider());
        }

        [Test]
        public void Resolve_TaskOfValueType_ReturnsCompletedTaskWithDefault()
        {
            var request = new FixtureRequest(typeof(Task<int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.InstanceOf<Task<int>>());
            var task = (Task<int>)result!;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(task.IsCompletedSuccessfully, Is.True);
                Assert.That(task.Result, Is.Zero);
            }
        }

        [Test]
        public void Resolve_TaskOfReferenceType_ReturnsCompletedTaskWithNull()
        {
            var request = new FixtureRequest(typeof(Task<string>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.InstanceOf<Task<string>>());
            var task = (Task<string>)result!;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(task.IsCompletedSuccessfully, Is.True);
                Assert.That(task.Result, Is.Null);
            }
        }

        [TestCase(typeof(Task), TestName = "Resolve_NonGenericTask_ReturnsNoResult")]
        [TestCase(typeof(ValueTask<int>), TestName = "Resolve_GenericValueTask_ReturnsNoResult")]
        [TestCase(typeof(List<int>), TestName = "Resolve_OtherGenericType_ReturnsNoResult")]
        public void Resolve_OtherType_ReturnsNoResult(Type type)
        {
            var request = new FixtureRequest(type);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
