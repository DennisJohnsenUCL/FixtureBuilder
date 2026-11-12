namespace FixtureBuilder.Tests.FixtureTests.InvokePrivate
{
    internal sealed class InvokePrivateExpressionTests
    {
        [Test]
        public void PrivateMethodOnProperty_InvokesMethod()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PrivateMethodClass>>();

            var result = fixture.InvokePrivate(x => x.Value, "Call").Build();

            Assert.That(result.Value.Calls, Is.EqualTo(1));
        }

        [Test]
        public void PublicMethodOnProperty_InvokesMethod()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PublicMethodClass>>();

            var result = fixture.InvokePrivate(x => x.Value, "Call").Build();

            Assert.That(result.Value.Calls, Is.EqualTo(1));
        }

        [Test]
        public void Arguments_InvokesMatchingOverloadWithArguments()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<OverloadedPrivateMethodClass>>();

            var result = fixture.InvokePrivate(x => x.Value, "Receive", "value").Build();

            Assert.That(result.Value.Received, Is.EqualTo("value"));
        }

        [Test]
        public void ArgumentsMatchNoOverload_ThrowsMissingMethodException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<OverloadedPrivateMethodClass>>();

            Assert.Throws<MissingMethodException>(() => fixture.InvokePrivate(x => x.Value, "Receive", 42));
        }

        [Test]
        public void UnknownMethod_ThrowsMissingMethodException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PrivateMethodClass>>();

            Assert.Throws<MissingMethodException>(() => fixture.InvokePrivate(x => x.Value, "Unknown"));
        }

        [Test]
        public void MethodCallInExpression_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PrivateMethodClass>>();

            Assert.Throws<InvalidOperationException>(() => fixture.InvokePrivate(x => x.Value.GetHashCode(), "Call"));
        }

        [Test]
        public void NullExpression_ThrowsArgumentNullException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PrivateMethodClass>>();

            var ex = Assert.Throws<ArgumentNullException>(() => fixture.InvokePrivate<PrivateMethodClass>(null!, "Call"));

            Assert.That(ex.ParamName, Is.EqualTo("expr"));
        }

        [Test]
        public void NullMethodName_ThrowsArgumentNullException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PrivateMethodClass>>();

            var ex = Assert.Throws<ArgumentNullException>(() => fixture.InvokePrivate(x => x.Value, null!));

            Assert.That(ex.ParamName, Is.EqualTo("methodName"));
        }

        [Test]
        public void NullArguments_ThrowsArgumentNullException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PrivateMethodClass>>();

            var ex = Assert.Throws<ArgumentNullException>(() => fixture.InvokePrivate(x => x.Value, "Call", null!));

            Assert.That(ex.ParamName, Is.EqualTo("arguments"));
        }

        [Test]
        public void PropertyChain_InitializesChainAndInvokesMethodOnLastMember()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PropertyHolder<PrivateMethodClass>>>();

            var result = fixture.InvokePrivate(x => x.Value.Value, "Call").Build();

            Assert.That(result.Value.Value.Calls, Is.EqualTo(1));
        }

        [Test]
        public void Field_InvokesMethodOnField()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<PrivateMethodClass>>();

            var result = fixture.InvokePrivate(x => x.Value, "Call").Build();

            Assert.That(result.Value.Calls, Is.EqualTo(1));
        }

        private struct PrivateMethodStruct
        {
            public int Calls { get; private set; }

            private void Call() => Calls++;
        }

        [Test]
        public void StructProperty_InvokesMethodAndWritesStructBack()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PrivateMethodStruct>>();

            var result = fixture.InvokePrivate(x => x.Value, "Call").Build();

            Assert.That(result.Value.Calls, Is.EqualTo(1));
        }

        [Test]
        public void PrivateMethodDeclaredOnBaseClassOfMember_InvokesMethod()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<DerivedFromPrivateMethodClass>>();

            var result = fixture.InvokePrivate(x => x.Value, "Call").Build();

            Assert.That(result.Value.Calls, Is.EqualTo(1));
        }

        [Test]
        public void PrivateMethodDeclaredOnRuntimeTypeOfMember_InvokesMethod()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<object>>();
            fixture.With(x => x.Value, new PrivateMethodClass());

            var result = fixture.InvokePrivate(x => x.Value, "Call").Build();

            var value = (PrivateMethodClass)result.Value;
            Assert.That(value.Calls, Is.EqualTo(1));
        }

        [Test]
        public void CalledTwice_InvokesOnSameInstance()
        {
            var fixture = Fixture.New<PropertyHolder<PrivateMethodClass>>();

            var result = fixture.InvokePrivate(x => x.Value, "Call").InvokePrivate(x => x.Value, "Call").Build();

            Assert.That(result.Value.Calls, Is.EqualTo(2));
        }

        [Test]
        public void NotConstructed_ConstructsImplicitlyAndInvokesMethod()
        {
            var fixture = Fixture.New<PropertyHolder<PrivateMethodClass>>();

            var result = fixture.InvokePrivate(x => x.Value, "Call").Build();

            Assert.That(result.Value.Calls, Is.EqualTo(1));
        }
    }
}
