namespace TestUtilities.Tests
{
	internal class WithSetterTests
	{
		private string _text;
		private int _number;

		[SetUp]
		public void Setup()
		{
			_text = "Test";
			_number = 123;
		}

		[Test]
		public void SetterInFixture_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithSetter(t => t.Text, _text).Build();

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void DerivedSetter_SetsProperty()
		{
			var fixture = FixtureBuilder.New<DerivedTestClass>().BypassConstructor().WithSetter(t => t.Text, _text).Build();

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void OverriddenSetter_SetsProperty()
		{
			var fixture = FixtureBuilder.New<DerivedTestClass>().BypassConstructor().WithSetter(t => t.Number, _number).Build();

			Assert.That(fixture.Number, Is.EqualTo(_number));
		}

		[Test]
		public void ImplicitInterfaceImplementation_SetsProperty()
		{
			var fixture = FixtureBuilder.New<InterfaceTestClass>().BypassConstructor().WithSetter(t => t.ImplicitProperty, _text).Build();

			Assert.That(fixture.ImplicitProperty, Is.EqualTo(_text));
		}


		[Test]
		public void ExplicitInterfaceImplementation_SetsProperty()
		{
			var fixture = FixtureBuilder.New<InterfaceTestClass>().BypassConstructor().WithSetter<ITestInterface, string>(t => t.ExplicitRefProperty, _text).Build();

			Assert.That(((ITestInterface)fixture).ExplicitRefProperty, Is.EqualTo(_text));
		}

		[Test]
		public void PropWithoutSetter_ThrowsException()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor();

			Assert.Throws<ArgumentException>(() => fixture.WithSetter(f => f.PropWithoutSetter, "Test"));
		}

		[Test]
		public void GenericClass_SetsProperty()
		{
			var fixture = FixtureBuilder.New<GenericClass<string>>().BypassConstructor().WithSetter(g => g.Value, _text).Build();

			Assert.That(fixture.Value, Is.EqualTo(_text));
		}

		[Test]
		public void NestedProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithSetter(t => t.NestedClass.Value, _text).Build();

			Assert.That(fixture.NestedClass.Value, Is.EqualTo(_text));
		}

		[Test]
		public void DeeperNestedProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithSetter(t => t.NestedClass.DeeperNestedClass.Value, _number).Build();

			Assert.That(fixture.NestedClass.DeeperNestedClass.Value, Is.EqualTo(_number));
		}

		[Test]
		public void DerivedNestedProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<DerivedTestClass>().BypassConstructor().WithSetter(t => t.NestedClass.Value, _text).Build();

			Assert.That(fixture.NestedClass.Value, Is.EqualTo(_text));
		}

		[Test]
		public void NestedInterfaceProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithSetter<INestedInterface, string>(t => t.NestedInterfaceClass.Value, _text).Build();

			Assert.That(((INestedInterface)fixture).NestedInterfaceClass.Value, Is.EqualTo(_text));
		}

		[Test]
		public void DeeperNestedInterfaceProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithSetter<INestedInterface, int>(t => t.NestedInterfaceClass.DeeperNestedClass.Value, _number).Build();

			Assert.That(((INestedInterface)fixture).NestedInterfaceClass.DeeperNestedClass.Value, Is.EqualTo(_number));
		}
	}
}
