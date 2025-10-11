namespace TestUtilities.Tests.FixtureBuilderTests
{
	internal sealed class TestClassObject
	{
		internal string Text { get; } = null!;
		internal int Number { get; }
		private readonly string _privateExplicitField = null!;
		internal string PrivateExplicitField => _privateExplicitField;
		private readonly string _privateExplicitNoUnderscoreField = null!;
		internal string PrivateExplicitNoUnderscoreField => _privateExplicitNoUnderscoreField;
	}
}
