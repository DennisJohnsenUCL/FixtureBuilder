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
			var fixture = FixtureBuilder.New<TestValue>().With(t => t.Text, _text).Build();

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void RecordProperties_SetsProperties()
		{
			var fixture = FixtureBuilder.New<TestValue>().With(t => t.Text, _text).With(t => t.Number, _number).Build();

			Assert.Multiple(() =>
			{
				Assert.That(fixture.Text, Is.EqualTo(_text));
				Assert.That(fixture.Number, Is.EqualTo(_number));
			});
		}

		[Test]
		public void NotARecordProperty_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => FixtureBuilder.New<TestValue>().With(t => t.GetHashCode(), _number).Build());
		}

		[Test]
		public void NoRecordPropertyBackingField_ThrowsException()
		{
			Assert.Throws<InvalidOperationException>(() => FixtureBuilder.New<TestValue>().With(t => t.Text.Length, _number).Build());
		}

		[Test]
		public void ClassProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().With(t => t.Text, _text).Build();

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void ClassProperties_SetsProperties()
		{
			var fixture = FixtureBuilder.New<TestClass>().With(t => t.Text, _text).With(t => t.Number, _number).Build();

			Assert.Multiple(() =>
			{
				Assert.That(fixture.Text, Is.EqualTo(_text));
				Assert.That(fixture.Number, Is.EqualTo(_number));
			});
		}

		[Test]
		public void NotAClassProperty_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => FixtureBuilder.New<TestClass>().With(t => t.GetHashCode(), _number).Build());
		}

		[Test]
		public void NoClassPropertyBackingField_ThrowsException()
		{
			Assert.Throws<InvalidOperationException>(() => FixtureBuilder.New<TestClass>().With(t => t.Text.Length, _number).Build());
		}

		[Test]
		public void ExplicitBackingField_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().With(t => t.PrivateExplicitField, _text).Build();

			Assert.That(fixture.PrivateExplicitField, Is.EqualTo(_text));
		}

		[Test]
		public void ExplicitBackingFieldNoUnderscore_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().With(t => t.PrivateExplicitNoUnderscoreField, _text).Build();

			Assert.That(fixture.PrivateExplicitNoUnderscoreField, Is.EqualTo(_text));
		}

		[Test]
		public void DerivedProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<DerivedTestClass>().With(t => t.Text, _text).Build();

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void OverriddenProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<DerivedTestClass>().With(t => t.Number, _number).Build();

			Assert.That(fixture.Number, Is.EqualTo(_number));
		}

		[Test]
		public void ImplicitInterfaceImplementation_SetsProperty()
		{
			var fixture = FixtureBuilder.New<InterfaceTestClass>().With(t => t.ImplicitProperty, _text).Build();

			Assert.That(fixture.ImplicitProperty, Is.EqualTo(_text));
		}

		[Test]
		public void ExplicitInterfaceImplementation_SetsProperty()
		{
			var fixture = FixtureBuilder.New<InterfaceTestClass>().With(t => ((ITestInterface)t).ExplicitProperty, _number).Build();

			Assert.That(((ITestInterface)fixture).ExplicitProperty, Is.EqualTo(_number));
		}
	}
}
