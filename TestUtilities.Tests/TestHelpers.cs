namespace TestUtilities.Tests
{
	internal record TestValue(string Text, int Number);

	internal class TestClass
	{
		public string Text { get; set; } = null!;
		public virtual int Number { get; protected set; }
		public string PropWithoutSetter { get; } = null!;
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
		string ImplicitProperty { get; set; }
		int ExplicitValueProperty { get; set; }
		string ExplicitRefProperty { get; set; }
	}

	internal class InterfaceTestClass : ITestInterface
	{
		public string ImplicitProperty { get; set; } = null!;
		int ITestInterface.ExplicitValueProperty { get; set; }
		string ITestInterface.ExplicitRefProperty { get; set; } = null!;
	}

	internal class NestedClass(string value);

	internal class NoDefaultConstructor
	{
		public string Value { get; set; }

		public NoDefaultConstructor(string value) => Value = value;
	}

	internal class DefaultConstructor
	{
		public string Value { get; set; }

		public DefaultConstructor() => Value = "Something";
	}

	internal class GenericClass<T> where T : class
	{
		public T Value { get; set; } = null!;
	}
}
