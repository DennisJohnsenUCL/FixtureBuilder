namespace FixtureBuilder.Tests.FixtureTests.InvokePrivate
{
    internal sealed class InvokePrivateTests
    {
        private class TestEntity
        {
            public bool PublicWasCalled { get; private set; }
            public bool PrivateWasCalled { get; private set; }
            public string ReceivedValue { get; private set; } = null!;

            public void PublicMethod() => PublicWasCalled = true;
            private void PrivateMethod() => PrivateWasCalled = true;
            private void MethodWithArgs(string value) => ReceivedValue = value;
        }

        [Test]
        public void InvokePrivate_CallsPrivateMethod()
        {
            var fixture = Fixture.New<TestEntity>().CreateUninitialized();

            fixture.InvokePrivate("PrivateMethod");

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.PrivateWasCalled, Is.True);
        }

        [Test]
        public void InvokePrivate_CallsPublicMethod()
        {
            var fixture = Fixture.New<TestEntity>().CreateUninitialized();

            fixture.InvokePrivate("PublicMethod");

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.PublicWasCalled, Is.True);
        }

        [Test]
        public void InvokePrivate_PassesArguments()
        {
            var fixture = Fixture.New<TestEntity>().CreateUninitialized();

            fixture.InvokePrivate("MethodWithArgs", "hello");

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.ReceivedValue, Is.EqualTo("hello"));
        }

        [Test]
        public void InvokePrivate_NonExistentMethod_ThrowsException()
        {
            var fixture = Fixture.New<TestEntity>().CreateUninitialized();

            Assert.Throws<InvalidOperationException>(() =>
                fixture.InvokePrivate("NonExistent"));
        }

        [Test]
        public void InvokePrivate_IncorrectArguments_ThrowsException()
        {
            var fixture = Fixture.New<TestEntity>().CreateUninitialized();

            Assert.Throws<ArgumentException>(() =>
                fixture.InvokePrivate("MethodWithArgs", 42));
        }

        [Test]
        public void InvokePrivate_InitializesFixtureOnFirstCall()
        {
            var fixture = Fixture.New<TestEntity>().CreateUninitialized();

            fixture.InvokePrivate("PrivateMethod");

            Assert.That(fixture.Build(), Is.Not.Null);
        }

        [Test]
        public void InvokePrivate_CalledMultipleTimes_DoesNotReinstantiate()
        {
            var fixture = Fixture.New<TestEntity>().CreateUninitialized();

            fixture.InvokePrivate("PrivateMethod");
            var first = TestHelper.GetFixture(fixture);

            fixture.InvokePrivate("PublicMethod");
            var second = TestHelper.GetFixture(fixture);

            Assert.That(second, Is.SameAs(first));
        }
    }
}
