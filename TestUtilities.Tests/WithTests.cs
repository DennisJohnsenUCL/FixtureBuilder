namespace TestUtilities.Tests
{
	internal sealed class WithTests
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
		public void RecordProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestValue>().BypassConstructor().With(t => t.Text, _text).Build();

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void RecordProperties_SetsProperties()
		{
			var fixture = FixtureBuilder.New<TestValue>().BypassConstructor().With(t => t.Text, _text).With(t => t.Number, _number).Build();

			Assert.Multiple(() =>
			{
				Assert.That(fixture.Text, Is.EqualTo(_text));
				Assert.That(fixture.Number, Is.EqualTo(_number));
			});
		}

		[Test]
		public void NotARecordProperty_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => FixtureBuilder.New<TestValue>().BypassConstructor().With(t => t.GetHashCode(), _number).Build());
		}

		[Test]
		public void NoRecordPropertyBackingField_ThrowsException()
		{
			Assert.Throws<MissingMethodException>(() => FixtureBuilder.New<TestValue>().BypassConstructor().With(t => t.Text.Length, _number).Build());
		}

		[Test]
		public void ClassProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().With(t => t.Text, _text).Build();

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void ClassProperties_SetsProperties()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().With(t => t.Text, _text).With(t => t.Number, _number).Build();

			Assert.Multiple(() =>
			{
				Assert.That(fixture.Text, Is.EqualTo(_text));
				Assert.That(fixture.Number, Is.EqualTo(_number));
			});
		}

		[Test]
		public void NotAClassProperty_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => FixtureBuilder.New<TestClass>().BypassConstructor().With(t => t.GetHashCode(), _number).Build());
		}

		[Test]
		public void NoClassPropertyBackingField_ThrowsException()
		{
			Assert.Throws<MissingMethodException>(() => FixtureBuilder.New<TestClass>().BypassConstructor().With(t => t.Text.Length, _number).Build());
		}

		[Test]
		public void ExplicitBackingField_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().With(t => t.PrivateExplicitField, _text).Build();

			Assert.That(fixture.PrivateExplicitField, Is.EqualTo(_text));
		}

		[Test]
		public void ExplicitBackingFieldNoUnderscore_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().With(t => t.PrivateExplicitNoUnderscoreField, _text).Build();

			Assert.That(fixture.PrivateExplicitNoUnderscoreField, Is.EqualTo(_text));
		}

		[Test]
		public void DerivedProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<DerivedTestClass>().BypassConstructor().With(t => t.Text, _text).Build();

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void OverriddenProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<DerivedTestClass>().BypassConstructor().With(t => t.Number, _number).Build();

			Assert.That(fixture.Number, Is.EqualTo(_number));
		}

		[Test]
		public void ImplicitInterfaceImplementation_SetsProperty()
		{
			var fixture = FixtureBuilder.New<InterfaceTestClass>().BypassConstructor().With(t => t.ImplicitProperty, _text).Build();

			Assert.That(fixture.ImplicitProperty, Is.EqualTo(_text));
		}

		[Test]
		public void ExplicitValueInterfaceImplementation_SetsProperty()
		{
			var fixture = FixtureBuilder.New<InterfaceTestClass>().BypassConstructor().With<ITestInterface, int>(t => t.ExplicitValueProperty, _number).Build();

			Assert.That(((ITestInterface)fixture).ExplicitValueProperty, Is.EqualTo(_number));
		}

		[Test]
		public void ExplicitRefInterfaceImplementation_SetsProperty()
		{
			var fixture = FixtureBuilder.New<InterfaceTestClass>().BypassConstructor().With<ITestInterface, string>(t => t.ExplicitRefProperty, _text).Build();

			Assert.That(((ITestInterface)fixture).ExplicitRefProperty, Is.EqualTo(_text));
		}

		[Test]
		public void TwiceDerivedClass_PropertyInDerivedClass_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TwiceDerivedClass>().BypassConstructor().With(p => p.Number, _number).Build();

			Assert.That(fixture.Number, Is.EqualTo(_number));
		}

		[Test]
		public void GenericClass_SetsProperty()
		{
			var fixture = FixtureBuilder.New<GenericClass<string>>().BypassConstructor().With(g => g.Value, _text).Build();

			Assert.That(fixture.Value, Is.EqualTo(_text));
		}

		[Test]
		public void NestedProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().With(t => t.NestedClass.Value, _text).Build();

			Assert.That(fixture.NestedClass.Value, Is.EqualTo(_text));
		}

		[Test]
		public void DerivedNestedProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<DerivedTestClass>().BypassConstructor().With(t => t.NestedClass.Value, _text).Build();

			Assert.That(fixture.NestedClass.Value, Is.EqualTo(_text));
		}

		[Test]
		public void NestedInterfaceProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().With<INestedInterface, string>(t => t.NestedInterfaceClass.Value, _text).Build();

			Assert.That(((INestedInterface)fixture).NestedInterfaceClass.Value, Is.EqualTo(_text));
		}

		[Test]
		public void DeeperNestedProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().With(t => t.NestedClass.DeeperNestedClass.Value, _number).Build();

			Assert.That(fixture.NestedClass.DeeperNestedClass.Value, Is.EqualTo(_number));
		}

		[Test]
		public void DeeperNestedInterfaceProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().With<INestedInterface, int>(t => t.NestedInterfaceClass.DeeperNestedClass.Value, _number).Build();

			Assert.That(((INestedInterface)fixture).NestedInterfaceClass.DeeperNestedClass.Value, Is.EqualTo(_number));
		}
	}
}
