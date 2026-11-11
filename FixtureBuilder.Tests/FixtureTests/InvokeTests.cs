namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class InvokeTests
    {
        [Test]
        public void Method_InvokesMethod()
        {
            var fixture = TestHelper.MakeFixture<PublicMethodClass>();

            var result = fixture.Invoke(x => x.Call()).Build();

            Assert.That(result.Calls, Is.EqualTo(1));
        }

        [Test]
        public void CalledTwice_InvokesOnSameInstance()
        {
            var fixture = Fixture.New<PublicMethodClass>();

            var result = fixture.Invoke(x => x.Call()).Invoke(x => x.Call()).Build();

            Assert.That(result.Calls, Is.EqualTo(2));
        }

        [Test]
        public void MethodOnPropertyChain_InitializesChainAndInvokesMethod()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PublicMethodClass>>();

            var result = fixture.Invoke(x => x.Value.Call()).Build();

            Assert.That(result.Value.Calls, Is.EqualTo(1));
        }

        [Test]
        public void MethodOnFieldChain_InitializesChainAndInvokesMethod()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<PublicMethodClass>>();

            var result = fixture.Invoke(x => x.Value.Call()).Build();

            Assert.That(result.Value.Calls, Is.EqualTo(1));
        }

        [Test]
        public void MethodOnClassInStructProperty_InitializesChainAndInvokesMethod()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PropertyHolderStruct<PublicMethodClass>>>();

            var result = fixture.Invoke(x => x.Value.Value.Call()).Build();

            Assert.That(result.Value.Value.Calls, Is.EqualTo(1));
        }

        [Test]
        public void MethodNotOnFixture_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<PublicMethodClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.Invoke(_ => Console.WriteLine()));
        }

        [Test]
        public void NotConstructed_ConstructsImplicitlyAndInvokesMethod()
        {
            var fixture = Fixture.New<PublicMethodClass>();

            var result = fixture.Invoke(x => x.Call()).Build();

            Assert.That(result.Calls, Is.EqualTo(1));
        }
    }
}
