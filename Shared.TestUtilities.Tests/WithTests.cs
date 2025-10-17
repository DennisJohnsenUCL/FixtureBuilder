namespace Shared.TestUtilities.Tests
{
	internal sealed class WithTests
	{
		[Test]
		public void PropWithSetter_SetsProperty()
		{
			var text = "Test string";

			var fixture = FixtureBuilder.New<TestClass>().With(t => t.Text, text).Build();

			Assert.That(fixture.Text, Is.EqualTo(text));
		}

		[Test]
		public void PropWithSetterAndUnrecognizedField_SetsProperty()
		{
			var text = "Test string";

			var fixture = FixtureBuilder.New<TestClass>().With(t => t.PropWithUnrelatedFieldName, text).Build();

			Assert.That(fixture.PropWithUnrelatedFieldName, Is.EqualTo(text));
		}

		[Test]
		public void PropWithoutSetter_SetsProperty()
		{
			var text = "Test string";

			var fixture = FixtureBuilder.New<TestClass>().With(t => t.PropWithoutSetter, text).Build();

			Assert.That(fixture.PropWithoutSetter, Is.EqualTo(text));
		}

		[Test]
		public void PropWithoutSetterAndUnrecognizedField_ThrowsException()
		{
			var text = "Test string";

			Assert.Throws<InvalidOperationException>(() => FixtureBuilder.New<TestClass>().With(t => t.PropNoSetterWithUnrelatedFieldName, text).Build());
		}
	}
}
