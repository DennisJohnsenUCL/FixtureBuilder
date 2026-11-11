using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class DelegateProviderTests
    {
        [Test]
        public void ResolveValue_ValueTypeReturn_ReturnsDelegateReturningDefault()
        {
            var provider = new DelegateProvider();
            var request = TestHelper.MakeRequest(typeof(Func<string, int>));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            var parse = (Func<string, int>)result!;
            var returned = parse("text");
            Assert.That(returned, Is.Zero);
        }

        [Test]
        public void ResolveValue_ReferenceTypeReturn_ReturnsDelegateReturningNull()
        {
            var provider = new DelegateProvider();
            var request = TestHelper.MakeRequest(typeof(Func<string>));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            var create = (Func<string>)result!;
            var returned = create();
            Assert.That(returned, Is.Null);
        }

        [Test]
        public void ResolveValue_TaskReturn_ReturnsDelegateReturningCompletedTask()
        {
            var provider = new DelegateProvider();
            var request = TestHelper.MakeRequest(typeof(Func<string, Task>));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            var run = (Func<string, Task>)result!;
            var returned = run("text");
            Assert.That(returned.IsCompletedSuccessfully, Is.True);
        }

        [Test]
        public void ResolveValue_TaskOfValueTypeReturn_ReturnsDelegateReturningCompletedTaskWithDefault()
        {
            var provider = new DelegateProvider();
            var request = TestHelper.MakeRequest(typeof(Func<string, Task<int>>));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            var parse = (Func<string, Task<int>>)result!;
            var returned = parse("text");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(returned.IsCompletedSuccessfully, Is.True);
                Assert.That(returned.Result, Is.Zero);
            }
        }

        [Test]
        public void ResolveValue_TaskOfReferenceTypeReturn_ReturnsDelegateReturningCompletedTaskWithNull()
        {
            var provider = new DelegateProvider();
            var request = TestHelper.MakeRequest(typeof(Func<Task<string>>));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            var create = (Func<Task<string>>)result!;
            var returned = create();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(returned.IsCompletedSuccessfully, Is.True);
                Assert.That(returned.Result, Is.Null);
            }
        }

        [Test]
        public void ResolveValue_Action_ReturnsInvokableDelegate()
        {
            var provider = new DelegateProvider();
            var request = TestHelper.MakeRequest(typeof(Action<int, string>));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            var action = (Action<int, string>)result!;
            Assert.DoesNotThrow(() => action(1, "text"));
        }

        private delegate bool TryParseDelegate(string text, out int value);

        [Test]
        public void ResolveValue_OutParameter_ReturnsDelegateLeavingOutParameterDefault()
        {
            var provider = new DelegateProvider();
            var request = TestHelper.MakeRequest(typeof(TryParseDelegate));

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            var tryParse = (TryParseDelegate)result!;
            var returned = tryParse("1", out var value);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(returned, Is.False);
                Assert.That(value, Is.Zero);
            }
        }

        private delegate ref int RefReturnDelegate();

        [Test]
        public void ResolveValue_RefReturn_ThrowsInvalidOperationException()
        {
            var provider = new DelegateProvider();
            var request = TestHelper.MakeRequest(typeof(RefReturnDelegate));

            var ex = Assert.Throws<InvalidOperationException>(() => provider.ResolveValue(request, Mock.Of<IFixtureContext>()));

            Assert.That(ex.Message, Does.Contain(nameof(RefReturnDelegate)));
        }

        [TestCase(typeof(Delegate))]
        [TestCase(typeof(MulticastDelegate))]
        [TestCase(typeof(string))]
        public void ResolveValue_OtherType_ReturnsNoResult(Type type)
        {
            var provider = new DelegateProvider();
            var request = TestHelper.MakeRequest(type);

            var result = provider.ResolveValue(request, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
