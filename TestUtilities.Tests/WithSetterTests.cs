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
			var fixture = FixtureBuilder.New<TestClass>().WithSetter(t => t.Text, _text).Build();

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void DerivedSetter_SetsProperty()
		{
			var fixture = FixtureBuilder.New<DerivedTestClass>().WithSetter(t => t.Text, _text).Build();

			Assert.That(fixture.Text, Is.EqualTo(_text));
		}

		[Test]
		public void OverriddenSetter_SetsProperty()
		{
			var fixture = FixtureBuilder.New<DerivedTestClass>().WithSetter(t => t.Number, _number).Build();

			Assert.That(fixture.Number, Is.EqualTo(_number));
		}
	}
}
