#pragma warning disable CA1822 // Mark members as static

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
            public void PublicMethod(string value) => ReceivedValue = value;
            private void PrivateMethod() => PrivateWasCalled = true;
            private void MethodWithArgs(string value) => ReceivedValue = value;
        }

        [Test]
        public void InvokePrivate_CallsPrivateMethod()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture.InvokePrivate("PrivateMethod");

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.PrivateWasCalled, Is.True);
        }

        [Test]
        public void InvokePrivate_CallsPublicMethod()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture.InvokePrivate("PublicMethod");

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.PublicWasCalled, Is.True);
        }

        [Test]
        public void InvokePrivate_CallsPublicMethodOverload()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture.InvokePrivate("PublicMethod", "hello");

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.ReceivedValue, Is.EqualTo("hello"));
        }

        [Test]
        public void InvokePrivate_PassesArguments()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture.InvokePrivate("MethodWithArgs", "hello");

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.ReceivedValue, Is.EqualTo("hello"));
        }

        [Test]
        public void InvokePrivate_NonExistentMethod_ThrowsException()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            Assert.Throws<MissingMethodException>(() =>
                fixture.InvokePrivate("NonExistent"));
        }

        [Test]
        public void InvokePrivate_IncorrectArguments_ThrowsException()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            Assert.Throws<MissingMethodException>(() =>
                fixture.InvokePrivate("MethodWithArgs", 42));
        }

        [Test]
        public void InvokePrivate_InitializesFixtureOnFirstCall()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture.InvokePrivate("PrivateMethod");

            Assert.That(fixture.Build(), Is.Not.Null);
        }

        [Test]
        public void InvokePrivate_CalledMultipleTimes_DoesNotReinstantiate()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture.InvokePrivate("PrivateMethod");
            var first = TestHelper.GetFixture(fixture);

            fixture.InvokePrivate("PublicMethod");
            var second = TestHelper.GetFixture(fixture);

            Assert.That(second, Is.SameAs(first));
        }

        [Test]
        public void InvokePrivate_InstantiatesFixture()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            using (Assert.EnterMultipleScope())
            {
                Assert.DoesNotThrow(() => fixture.InvokePrivate("PublicMethod"));
                Assert.That(TestHelper.GetFixture(fixture), Is.Not.Null);
            }
        }

        private class BaseEntity
        {
            public bool BaseWasCalled { get; private set; }

            private void BaseMethod() => BaseWasCalled = true;
            private void SharedMethod() => BaseWasCalled = true;
        }

        private class DerivedEntity : BaseEntity
        {
            public bool DerivedWasCalled { get; private set; }

            private void SharedMethod() => DerivedWasCalled = true;
        }

        private class ThrowingEntity : BaseEntity
        {
            private void SharedMethod() => throw new InvalidOperationException("Thrown by SharedMethod");
        }

        private class ThrowingMissingMethodEntity : BaseEntity
        {
            private void SharedMethod() => throw new MissingMethodException("Thrown by SharedMethod");
        }

        [Test]
        public void InvokePrivate_PrivateMethodDeclaredOnBaseClass_CallsMethod()
        {
            var fixture = TestHelper.MakeFixture<DerivedEntity>();

            fixture.InvokePrivate("BaseMethod");

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.BaseWasCalled, Is.True);
        }

        [Test]
        public void InvokePrivate_MethodDeclaredOnDerivedAndBaseClass_CallsOnlyDerivedMethod()
        {
            var fixture = TestHelper.MakeFixture<DerivedEntity>();

            fixture.InvokePrivate("SharedMethod");

            var result = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.DerivedWasCalled, Is.True);
                Assert.That(result.BaseWasCalled, Is.False);
            }
        }

        [Test]
        public void InvokePrivate_MethodThrows_RethrowsOriginalExceptionWithoutCallingBaseMethod()
        {
            var fixture = TestHelper.MakeFixture<ThrowingEntity>();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.InvokePrivate("SharedMethod"));

            var result = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex!.Message, Is.EqualTo("Thrown by SharedMethod"));
                Assert.That(result.BaseWasCalled, Is.False);
            }
        }

        [Test]
        public void InvokePrivate_MethodThrowsMissingMethodException_RethrowsItWithoutCallingBaseMethod()
        {
            var fixture = TestHelper.MakeFixture<ThrowingMissingMethodEntity>();

            var ex = Assert.Throws<MissingMethodException>(() => fixture.InvokePrivate("SharedMethod"));

            var result = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex!.Message, Is.EqualTo("Thrown by SharedMethod"));
                Assert.That(result.BaseWasCalled, Is.False);
            }
        }
    }
}
