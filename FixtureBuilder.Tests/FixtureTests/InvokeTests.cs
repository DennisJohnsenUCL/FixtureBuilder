namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class InvokeTests
    {
        private class TestEntity
        {
            public bool WasCalled { get; private set; }
            public bool WasCalledElse { get; private set; }
            public ChildEntity Child { get; set; } = null!;
            public void DoSomething() => WasCalled = true;
            public void DoSomethingElse() => WasCalledElse = true;
        }

        private class ChildEntity
        {
            public bool WasCalled { get; private set; }
            public void DoSomething() => WasCalled = true;
        }

        [Test]
        public void Invoke_ExecutesAction()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture = fixture.Invoke(x => x.DoSomething());

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.WasCalled, Is.True);
        }

        [Test]
        public void Invoke_WithNestedPath_InitializesIntermediates()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture = fixture.Invoke(x => x.Child.DoSomething());

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Child, Is.Not.Null);
            Assert.That(result.Child.WasCalled, Is.True);
        }

        [Test]
        public void Invoke_InitializesFixtureOnFirstCall()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture = fixture.Invoke(x => x.DoSomething());

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Invoke_CalledMultipleTimes_DoesNotReinstantiate()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            fixture.Invoke(x => x.DoSomething());
            var first = TestHelper.GetFixture(fixture);

            fixture.Invoke(x => x.DoSomethingElse());
            var second = TestHelper.GetFixture(fixture);

            Assert.That(second, Is.SameAs(first));
        }

        [Test]
        public void Invoke_ConstantExpression_ThrowsException()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            Assert.Throws<InvalidOperationException>(() => fixture.Invoke(x => Console.WriteLine()));
        }

        [Test]
        public void Invoke_InstantiatesFixture()
        {
            var fixture = TestHelper.MakeFixture<TestEntity>();

            using (Assert.EnterMultipleScope())
            {
                Assert.DoesNotThrow(() => fixture.Invoke(x => x.DoSomething()));
                Assert.That(TestHelper.GetFixture(fixture), Is.Not.Null);
            }
        }

        private class FieldIntermediateEntity
        {
            public ChildEntity Child = null!;
        }

        [Test]
        public void FieldIntermediate_InitializesAndInvokes()
        {
            var fixture = TestHelper.MakeFixture<FieldIntermediateEntity>();

            fixture.Invoke(x => x.Child.DoSomething());

            var result = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Child, Is.Not.Null);
                Assert.That(result.Child.WasCalled, Is.True);
            }
        }
    }
}
