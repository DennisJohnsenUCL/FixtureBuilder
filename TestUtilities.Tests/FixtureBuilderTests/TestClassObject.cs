namespace TestUtilities.Tests.FixtureBuilderTests
{
    internal sealed class TestClassObject
    {
        internal string Text { get; } = null!;
        internal int Number { get; }
        private readonly string _privateField = null!;
        internal string PrivateField => _privateField;
    }
}
