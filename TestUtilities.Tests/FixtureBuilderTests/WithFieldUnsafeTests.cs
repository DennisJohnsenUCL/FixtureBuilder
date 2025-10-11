namespace TestUtilities.Tests.FixtureBuilderTests
{
	internal sealed class WithFieldUnsafeTests
	{
		[Test]
		public void ClassPrivateField_SetsField()
		{
			var text = "New string name";
			var fieldName = "_privateExplicitField";

			var fixture = FixtureBuilder.New<TestClassObject>().WithFieldUnsafe(fieldName, text).Build();

			Assert.That(fixture.PrivateExplicitField, Is.EqualTo(text));
		}

		[Test]
		public void IncorrectFieldName_ThrowsException()
		{
			var text = "New string name";
			var fieldName = "_notAField";

			Assert.Throws<InvalidOperationException>(() => FixtureBuilder.New<TestClassObject>().WithFieldUnsafe(fieldName, text).Build());
		}

		[Test]
		public void IncorrectFieldType_ThrowsException()
		{
			var number = 123;
			var fieldName = "_privateExplicitField";

			Assert.Throws<ArgumentException>(() => FixtureBuilder.New<TestClassObject>().WithFieldUnsafe(fieldName, number).Build());
		}
	}
}
