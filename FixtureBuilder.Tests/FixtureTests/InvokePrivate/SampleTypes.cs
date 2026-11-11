namespace FixtureBuilder.Tests.FixtureTests.InvokePrivate
{
    internal class PrivateMethodClass
    {
        public int Calls { get; private set; }

        private void Call() => Calls++;
    }

    internal class DerivedFromPrivateMethodClass : PrivateMethodClass;

    internal class OverloadedPrivateMethodClass
    {
        public string Received { get; private set; } = null!;

        private void Receive() => Received = "no arguments";
        private void Receive(string value) => Received = value;
    }
}
