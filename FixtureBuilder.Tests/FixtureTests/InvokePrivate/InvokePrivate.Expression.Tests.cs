namespace FixtureBuilder.Tests.FixtureTests.InvokePrivate
{
    internal sealed class InvokePrivateExpressionTests
    {
        private class TestEntity
        {
            public ChildEntity Child { get; set; } = null!;
        }

        private class ChildEntity
        {
            public bool PublicWasCalled { get; private set; }
            public bool PrivateWasCalled { get; private set; }
            public string ReceivedValue { get; private set; } = null!;
            public GrandchildEntity Grandchild { get; set; } = null!;

            public void PublicMethod() => PublicWasCalled = true;
            public void PublicMethod(string value) => ReceivedValue = value;
            private void PrivateMethod() => PrivateWasCalled = true;
            private void MethodWithArgs(string value) => ReceivedValue = value;
        }

        private class GrandchildEntity
        {
            public bool PrivateWasCalled { get; private set; }
            private void PrivateMethod() => PrivateWasCalled = true;
        }

        [Test]
        public void InvokePrivate_CallsPrivateMethodOnProperty()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture.InvokePrivate(x => x.Child, "PrivateMethod");

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Child.PrivateWasCalled, Is.True);
        }

        [Test]
        public void InvokePrivate_CallsPublicMethodOnProperty()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture.InvokePrivate(x => x.Child, "PublicMethod");

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Child.PublicWasCalled, Is.True);
        }

        [Test]
        public void InvokePrivate_CallsPublicMethodOverloadOnProperty()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture.InvokePrivate(x => x.Child, "PublicMethod", "hello");

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Child.ReceivedValue, Is.EqualTo("hello"));
        }

        [Test]
        public void InvokePrivate_PassesArgumentsToMethod()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture.InvokePrivate(x => x.Child, "MethodWithArgs", "hello");

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Child.ReceivedValue, Is.EqualTo("hello"));
        }

        [Test]
        public void InvokePrivate_InitializesIntermediateProperty()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture.InvokePrivate(x => x.Child, "PrivateMethod");

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Child, Is.Not.Null);
        }

        [Test]
        public void InvokePrivate_WorksWithNestedPath()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture.InvokePrivate(x => x.Child.Grandchild, "PrivateMethod");

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Child, Is.Not.Null);
            Assert.That(result.Child.Grandchild, Is.Not.Null);
            Assert.That(result.Child.Grandchild.PrivateWasCalled, Is.True);
        }

        [Test]
        public void InvokePrivate_NonExistentMethod_ThrowsException()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            Assert.Throws<MissingMethodException>(() =>
                fixture.InvokePrivate(x => x.Child, "NonExistent"));
        }

        [Test]
        public void InvokePrivate_IncorrectArguments_ThrowsException()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            Assert.Throws<MissingMethodException>(() =>
                fixture.InvokePrivate(x => x.Child, "MethodWithArgs", 42));
        }

        [Test]
        public void InvokePrivate_InitializesFixtureOnFirstCall()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture.InvokePrivate(x => x.Child, "PrivateMethod");

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void InvokePrivate_CalledMultipleTimes_DoesNotReinstantiate()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture.InvokePrivate(x => x.Child, "PrivateMethod");
            var first = TestHelper.GetFixture(fixture);

            fixture.InvokePrivate(x => x.Child, "PublicMethod");
            var second = TestHelper.GetFixture(fixture);

            Assert.That(second, Is.SameAs(first));
        }

        [Test]
        public void InvokePrivate_InstantiatesFixture()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            using (Assert.EnterMultipleScope())
            {
                Assert.DoesNotThrow(() => fixture.InvokePrivate(x => x.Child, "PublicMethod"));
                Assert.That(TestHelper.GetFixture(fixture), Is.Not.Null);
            }
        }
    }
}
