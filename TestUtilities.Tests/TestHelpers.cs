namespace TestUtilities.Tests
{
	internal record TestValue(string Text, int Number);

	internal class TestClass
	{
		public string Text { get; protected set; } = null!;
		public virtual int Number { get; protected set; }
		private readonly string _privateExplicitField = null!;
		internal string PrivateExplicitField => _privateExplicitField;
		private readonly string _privateExplicitNoUnderscoreField = null!;
		internal string PrivateExplicitNoUnderscoreField => _privateExplicitNoUnderscoreField;
		protected string _inheritedField = null!;
		internal string InheritedFieldGetter => _inheritedField;
	}

	internal class DerivedTestClass : TestClass
	{
		public override int Number { get; protected set; }
	}

	internal class TwiceDerivedClass : TestClass { }

	internal interface ITestInterface
	{
		string ImplicitProperty { get; }
		int ExplicitProperty { get; }
	}

	internal class InterfaceTestClass : ITestInterface
	{
		public string ImplicitProperty { get; } = null!;

		int ITestInterface.ExplicitProperty { get; }
	}
}
