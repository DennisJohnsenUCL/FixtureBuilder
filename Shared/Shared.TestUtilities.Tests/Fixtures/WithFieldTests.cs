using Shared.TestUtilities.Fixtures;
using System.Reflection;

namespace Shared.TestUtilities.Tests.Fixtures
{
	internal sealed class WithFieldTests
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
			var fixture = FixtureBuilder.New<TestValue>().BypassConstructor().WithField(t => t.Text, _text).Build();

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void RecordProperties_SetsProperties()
		{
			var fixture = FixtureBuilder.New<TestValue>().BypassConstructor().WithField(t => t.Text, _text).WithField(t => t.Number, _number).Build();

			using (Assert.EnterMultipleScope())
			{
				Assert.That(fixture.Text, Is.EqualTo(_text));
				Assert.That(fixture.Number, Is.EqualTo(_number));
			}
		}

		[Test]
		public void NotARecordProperty_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => FixtureBuilder.New<TestValue>().BypassConstructor().WithField(t => t.GetHashCode(), _number).Build());
		}

		[Test]
		public void NoRecordPropertyBackingField_ThrowsException()
		{
			Assert.Throws<InvalidOperationException>(() => FixtureBuilder.New<TestValue>().BypassConstructor().WithField(t => t.Text.Length, _number).Build());
		}

		[Test]
		public void ClassProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithField(t => t.Text, _text).Build();

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void ClassProperties_SetsProperties()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithField(t => t.Text, _text).WithField(t => t.Number, _number).Build();

			using (Assert.EnterMultipleScope())
			{
				Assert.That(fixture.Text, Is.EqualTo(_text));
				Assert.That(fixture.Number, Is.EqualTo(_number));
			}
		}

		[Test]
		public void NotAClassProperty_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => FixtureBuilder.New<TestClass>().BypassConstructor().WithField(t => t.GetHashCode(), _number).Build());
		}

		[Test]
		public void NoClassPropertyBackingField_ThrowsException()
		{
			Assert.Throws<InvalidOperationException>(() => FixtureBuilder.New<TestClass>().BypassConstructor().WithField(t => t.Text.Length, _number).Build());
		}

		[Test]
		public void ExplicitBackingField_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithField(t => t.PrivateExplicitField, _text).Build();

			Assert.That(fixture.PrivateExplicitField, Is.EqualTo(_text));
		}

		[Test]
		public void ExplicitBackingFieldNoUnderscore_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithField(t => t.PrivateExplicitNoUnderscoreField, _text).Build();

			Assert.That(fixture.PrivateExplicitNoUnderscoreField, Is.EqualTo(_text));
		}

		[Test]
		public void DerivedProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<DerivedTestClass>().BypassConstructor().WithField(t => t.Text, _text).Build();

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void OverriddenProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<DerivedTestClass>().BypassConstructor().WithField(t => t.Number, _number).Build();

			Assert.That(fixture.Number, Is.EqualTo(_number));
		}

		[Test]
		public void ImplicitInterfaceImplementation_SetsProperty()
		{
			var fixture = FixtureBuilder.New<InterfaceTestClass>().BypassConstructor().WithField(t => t.ImplicitProperty, _text).Build();

			Assert.That(fixture.ImplicitProperty, Is.EqualTo(_text));
		}

		[Test]
		public void TwiceDerivedClass_PropertyInDerivedClass_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TwiceDerivedClass>().BypassConstructor().WithField(p => p.Number, _number).Build();

			Assert.That(fixture.Number, Is.EqualTo(_number));
		}

		[Test]
		public void GenericClass_SetsProperty()
		{
			var fixture = FixtureBuilder.New<GenericClass<string>>().BypassConstructor().WithField(g => g.Value, _text).Build();

			Assert.That(fixture.Value, Is.EqualTo(_text));
		}

		[Test]
		public void NestedProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithField(t => t.NestedClass.Value, _text).Build();

			Assert.That(fixture.NestedClass.Value, Is.EqualTo(_text));
		}

		[Test]
		public void DerivedNestedProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<DerivedTestClass>().BypassConstructor().WithField(t => t.NestedClass.Value, _text).Build();

			Assert.That(fixture.NestedClass.Value, Is.EqualTo(_text));
		}

		[Test]
		public void DeeperNestedProperty_SetsProperty()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithField(t => t.NestedClass.DeeperNestedClass.Value, _number).Build();

			Assert.That(fixture.NestedClass.DeeperNestedClass.Value, Is.EqualTo(_number));
		}

		[Test]
		public void SkipConstructionMethods_ConstructsFixture()
		{
			var fixture = FixtureBuilder.New<TestClass>().WithField(t => t.Text, _text).Build();

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void ClassPrivateField_SetsField()
		{
			var fieldName = "_privateExplicitField";

			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithField(fieldName, _text).Build();

			Assert.That(fixture.PrivateExplicitField, Is.EqualTo(_text));
		}

		[Test]
		public void IncorrectFieldName_ThrowsException()
		{
			var fieldName = "_notAField";

			Assert.Throws<InvalidOperationException>(() => FixtureBuilder.New<TestClass>().BypassConstructor().WithField(fieldName, _text).Build());
		}

		[Test]
		public void IncorrectFieldType_ThrowsException()
		{
			var fieldName = "_privateExplicitField";

			Assert.Throws<ArgumentException>(() => FixtureBuilder.New<TestClass>().BypassConstructor().WithField(fieldName, _number).Build());
		}

		[Test]
		public void InheritedProtectedField_SetsField()
		{
			var fieldName = "_inheritedField";

			var derivedTestClass = FixtureBuilder.New<DerivedTestClass>().BypassConstructor().WithField(fieldName, _text).Build();

			Assert.That(derivedTestClass.InheritedFieldGetter, Is.EqualTo(_text));
		}

		[Test]
		public void FieldNameGiven_DeeperNestedProperty_SetsProperty()
		{
			var fieldName = "_privateField";

			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithField(fieldName, t => t.NestedClass.DeeperNestedClass.PrivateFieldGetter, _text).Build();

			Assert.That(fixture.NestedClass.DeeperNestedClass.PrivateFieldGetter, Is.EqualTo(_text));
		}

		[Test]
		public void FieldNameGiven_SkipConstructionMethods_ConstructsFixture()
		{
			var fieldName = "_privateExplicitField";

			var fixture = FixtureBuilder.New<TestClass>().WithField(fieldName, _text).Build();

			Assert.That(fixture.PrivateExplicitField, Is.EqualTo(_text));
		}

		[Test]
		public void ClassWithMembers_InstantiatesClassMembers()
		{
			var fixture = FixtureBuilder.New<TestClass>().WithField(t => t.Text, _text);

			var field = (TestClass)fixture.GetType().GetField("_fixture", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(fixture)!;

			Assert.That(field.NestedClass, Is.Not.Null);
		}

		[Test]
		public void CollectionTypeField_OneParameter_SetsField()
		{
			var fieldName = "_stringListField";

			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithField(fieldName, _text).Build();

			Assert.That(fixture.StringListProp[0], Is.EqualTo(_text));
		}

		[Test]
		public void CollectionTypeField_CollectionParameter_SetsField()
		{
			var fieldName = "_stringListField";

			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithField(fieldName, [_text]).Build();

			Assert.That(fixture.StringListProp[0], Is.EqualTo(_text));
		}

		[Test]
		public void CollectionTypeField_CollectionParameters_SetsField()
		{
			var fieldName = "_stringListField";
			var secondEntry = "More test";

			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithField(fieldName, [_text, secondEntry]).Build();

			using (Assert.EnterMultipleScope())
			{
				Assert.That(fixture.StringListProp[0], Is.EqualTo(_text));
				Assert.That(fixture.StringListProp[1], Is.EqualTo(secondEntry));
			}
		}

		[Test]
		public void CollectionTypeFieldWithProp_CollectionParameters_SetsField()
		{
			var fieldName = "_stringListField";

			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor().WithField(fieldName, t => t.StringListProp, [_text]).Build();

			Assert.That(fixture.StringListProp[0], Is.EqualTo(_text));
		}
	}
}
