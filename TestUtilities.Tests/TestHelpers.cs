namespace TestUtilities.Tests
{
	internal record TestValue(string Text, int Number);

	internal class TestClass : INestedInterface
	{
		public string Text { get; set; } = null!;
		public virtual int Number { get; protected set; }
		public string PropWithoutSetter { get; } = null!;
		public string PropWithPrivateSetter { get; private set; } = null!;
		private readonly string _privateExplicitField = null!;
		internal string PrivateExplicitField => _privateExplicitField;
		private readonly string privateExplicitNoUnderscoreField = null!;
		internal string PrivateExplicitNoUnderscoreField => privateExplicitNoUnderscoreField;
		protected string _inheritedField = null!;
		internal string InheritedFieldGetter => _inheritedField;
		internal NestedClass NestedClass { get; set; } = null!;
		NestedClass INestedInterface.NestedInterfaceClass { get; set; } = null!;
		private string _unrelatedFieldName = null!;
		public string PropWithUnrelatedFieldName { get => _unrelatedFieldName; set => _unrelatedFieldName = value; }
		public string PropNoSetterWithUnrelatedFieldName => _unrelatedFieldName;
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

	internal class NoDefaultConstructor
	{
		public string Value { get; set; }

		private NoDefaultConstructor(string value) => Value = value;
	}

	internal class DefaultConstructor
	{
		public string Value { get; set; }

		private DefaultConstructor() => Value = "Something";
	}

	internal class GenericClass<T> where T : class
	{
		public T Value { get; set; } = null!;
	}

	internal class NestedClass
	{
		public string Value { get; set; } = null!;
		public DeeperNestedClass DeeperNestedClass { get; set; } = null!;

		public NestedClass() { }
	}

	internal class DeeperNestedClass
	{
		public int Value { get; set; }

		private readonly string _privateField = null!;
		public string PrivateFieldGetter => _privateField;

		public DeeperNestedClass(int value) { Value = value; }
	}

	internal interface INestedInterface
	{
		NestedClass NestedInterfaceClass { get; set; }
	}
}
