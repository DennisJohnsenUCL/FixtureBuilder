namespace Shared.TestUtilities.Tests
{
	internal sealed class ExtensionTests
	{
		private static string _text = "Test string";

		[Test]
		public void With_Property_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().Build();

			fixture = fixture.With(f => f.Text, _text);

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void With_InterfaceProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().Build();

			fixture = fixture.With<TestClass, INestedInterface, string>(f => f.NestedInterfaceClass.Value, _text);

			Assert.That(((INestedInterface)fixture).NestedInterfaceClass.Value, Is.EqualTo(_text));
		}

		[Test]
		public void WithField_Property_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().Build();

			fixture = fixture.WithField(f => f.Text, _text);

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void WithField_InterfaceProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().Build();

			fixture = fixture.WithField<TestClass, INestedInterface, string>(f => f.NestedInterfaceClass.Value, _text);

			Assert.That(((INestedInterface)fixture).NestedInterfaceClass.Value, Is.EqualTo(_text));
		}

		[Test]
		public void WithField_FieldName_SetsField()
		{
			var fieldName = "_privateExplicitField";

			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithField(fieldName, _text).Build();

			Assert.That(fixture.PrivateExplicitField, Is.EqualTo(_text));
		}

		[Test]
		public void WithField_FieldName_CollectionTypeField_SetsField()
		{
			var fieldName = "_stringListField";
			var secondEntry = "More test";

			var fixture = FixtureBuilder.New<TestClass>().Build();

			fixture = fixture.WithField(fieldName, [_text, secondEntry]);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(fixture.StringListProp[0], Is.EqualTo(_text));
				Assert.That(fixture.StringListProp[1], Is.EqualTo(secondEntry));
			}
		}

		[Test]
		public void WithField_FieldNameAndProperty_SetsProperty()
		{
			var fieldName = "_privateField";

			var fixture = FixtureBuilder.New<TestClass>().Build();

			fixture = fixture.WithField(fieldName, t => t.NestedClass.DeeperNestedClass.PrivateFieldGetter, _text);

			Assert.That(fixture.NestedClass.DeeperNestedClass.PrivateFieldGetter, Is.EqualTo(_text));
		}

		[Test]
		public void WithField_FieldNameAndInterfaceProperty_SetsProperty()
		{
			var fieldName = "_privateField";

			var fixture = FixtureBuilder.New<TestClass>().Build();

			fixture = fixture.WithField<TestClass, INestedInterface, string>(fieldName, t => t.NestedInterfaceClass.DeeperNestedClass.PrivateFieldGetter, _text);

			Assert.That(((INestedInterface)fixture).NestedInterfaceClass.DeeperNestedClass.PrivateFieldGetter, Is.EqualTo(_text));
		}

		[Test]
		public void WithSetter_Property_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().Build();

			fixture = fixture.WithSetter(t => t.Text, _text);

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void WithSetter_InterfaceProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<InterfaceTestClass>().Build();

			fixture = fixture.WithSetter<InterfaceTestClass, ITestInterface, string>(t => t.ExplicitRefProperty, _text);

			Assert.That(((ITestInterface)fixture).ExplicitRefProperty, Is.EqualTo(_text));
		}
	}
}
