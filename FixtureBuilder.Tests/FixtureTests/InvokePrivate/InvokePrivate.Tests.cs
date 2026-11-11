namespace FixtureBuilder.Tests.FixtureTests.InvokePrivate
{
    internal sealed class InvokePrivateTests
    {
        #region Invocation

        [Test]
        public void PrivateMethod_InvokesMethod()
        {
            var fixture = TestHelper.MakeFixture<PrivateMethodClass>();

            var result = fixture.InvokePrivate("Call").Build();

            Assert.That(result.Calls, Is.EqualTo(1));
        }

        [Test]
        public void PublicMethod_InvokesMethod()
        {
            var fixture = TestHelper.MakeFixture<PublicMethodClass>();

            var result = fixture.InvokePrivate("Call").Build();

            Assert.That(result.Calls, Is.EqualTo(1));
        }

        [Test]
        public void NoArguments_InvokesParameterlessOverload()
        {
            var fixture = TestHelper.MakeFixture<OverloadedPrivateMethodClass>();

            var result = fixture.InvokePrivate("Receive").Build();

            Assert.That(result.Received, Is.EqualTo("no arguments"));
        }

        [Test]
        public void Arguments_InvokesMatchingOverloadWithArguments()
        {
            var fixture = TestHelper.MakeFixture<OverloadedPrivateMethodClass>();

            var result = fixture.InvokePrivate("Receive", "value").Build();

            Assert.That(result.Received, Is.EqualTo("value"));
        }

        [Test]
        public void ArgumentsMatchNoOverload_ThrowsMissingMethodException()
        {
            var fixture = TestHelper.MakeFixture<OverloadedPrivateMethodClass>();

            Assert.Throws<MissingMethodException>(() => fixture.InvokePrivate("Receive", 42));
        }

        [Test]
        public void UnknownMethod_ThrowsMissingMethodException()
        {
            var fixture = TestHelper.MakeFixture<PrivateMethodClass>();

            Assert.Throws<MissingMethodException>(() => fixture.InvokePrivate("Unknown"));
        }

        [Test]
        public void CalledTwice_InvokesOnSameInstance()
        {
            var fixture = Fixture.New<PrivateMethodClass>();

            var result = fixture.InvokePrivate("Call").InvokePrivate("Call").Build();

            Assert.That(result.Calls, Is.EqualTo(2));
        }

        [Test]
        public void NotConstructed_ConstructsImplicitlyAndInvokesMethod()
        {
            var fixture = Fixture.New<PrivateMethodClass>();

            var result = fixture.InvokePrivate("Call").Build();

            Assert.That(result.Calls, Is.EqualTo(1));
        }

        #endregion

        #region Inheritance

        private class BaseWithSharedMethodClass
        {
            public string CalledOn { get; protected set; } = null!;

            private void Shared() => CalledOn = "base";
        }

        [Test]
        public void PrivateMethodDeclaredOnBaseClass_InvokesMethod()
        {
            var fixture = TestHelper.MakeFixture<DerivedFromPrivateMethodClass>();

            var result = fixture.InvokePrivate("Call").Build();

            Assert.That(result.Calls, Is.EqualTo(1));
        }

        private class DerivedWithSharedMethodClass : BaseWithSharedMethodClass
        {
            private void Shared() => CalledOn = "derived";
        }

        [Test]
        public void MethodDeclaredOnDerivedAndBaseClass_InvokesOnlyDerivedMethod()
        {
            var fixture = TestHelper.MakeFixture<DerivedWithSharedMethodClass>();

            var result = fixture.InvokePrivate("Shared").Build();

            Assert.That(result.CalledOn, Is.EqualTo("derived"));
        }

        private class DerivedThrowingClass : BaseWithSharedMethodClass
        {
            private void Shared() => throw new InvalidOperationException("thrown by derived");
        }

        [Test]
        public void DerivedMethodThrows_RethrowsExceptionWithoutInvokingBaseMethod()
        {
            var fixture = TestHelper.MakeFixture<DerivedThrowingClass>();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.InvokePrivate("Shared"));

            var result = fixture.Build();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex.Message, Is.EqualTo("thrown by derived"));
                Assert.That(result.CalledOn, Is.Null);
            }
        }

        private class DerivedThrowingMissingMethodClass : BaseWithSharedMethodClass
        {
            private void Shared() => throw new MissingMethodException("thrown by derived");
        }

        [Test]
        public void DerivedMethodThrowsMissingMethodException_RethrowsItWithoutInvokingBaseMethod()
        {
            var fixture = TestHelper.MakeFixture<DerivedThrowingMissingMethodClass>();

            var ex = Assert.Throws<MissingMethodException>(() => fixture.InvokePrivate("Shared"));

            var result = fixture.Build();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex.Message, Is.EqualTo("thrown by derived"));
                Assert.That(result.CalledOn, Is.Null);
            }
        }

        #endregion
    }
}
