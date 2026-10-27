using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class TaskProviderTests
    {
        private TaskProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _sut = new TaskProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new TaskProvider());
        }

        [Test]
        public void Resolve_Task_ReturnsCompletedTask()
        {
            var request = new FixtureRequest(typeof(Task));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.InstanceOf<Task>());
                Assert.That(((Task)result!).IsCompletedSuccessfully, Is.True);
            }
        }

        [TestCase(typeof(Task<int>), TestName = "Resolve_GenericTask_ReturnsNoResult")]
        [TestCase(typeof(ValueTask), TestName = "Resolve_ValueTask_ReturnsNoResult")]
        [TestCase(typeof(string), TestName = "Resolve_String_ReturnsNoResult")]
        public void Resolve_OtherType_ReturnsNoResult(Type type)
        {
            var request = new FixtureRequest(type);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
