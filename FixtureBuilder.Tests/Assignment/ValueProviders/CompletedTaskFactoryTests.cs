using FixtureBuilder.Assignment.ValueProviders;

namespace FixtureBuilder.Tests.Assignment.ValueProviders
{
    internal sealed class CompletedTaskFactoryTests
    {
        [Test]
        public void Create_ValueType_ReturnsCompletedTaskWithDefault()
        {
            var result = CompletedTaskFactory.Create(typeof(int));

            Assert.That(result, Is.InstanceOf<Task<int>>());
            var task = (Task<int>)result;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(task.IsCompletedSuccessfully, Is.True);
                Assert.That(task.Result, Is.Zero);
            }
        }

        [Test]
        public void Create_ReferenceType_ReturnsCompletedTaskWithNull()
        {
            var result = CompletedTaskFactory.Create(typeof(string));

            Assert.That(result, Is.InstanceOf<Task<string>>());
            var task = (Task<string>)result;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(task.IsCompletedSuccessfully, Is.True);
                Assert.That(task.Result, Is.Null);
            }
        }
    }
}
