namespace Shared.TestUtilities.Tests
{
	internal class NewTests
	{
		[Test]
		public void PreBuiltFixture_SetsAndBuildsFixture()
		{
			var text = "Test";
			var number = 123;

			var preBuiltFixture = new TestClass() { Text = text };

			var fixture = FixtureBuilder.New(preBuiltFixture).WithField(p => p.Number, number).Build();

			Assert.Multiple(() =>
			{
				Assert.That(fixture.Text, Is.EqualTo(text));
				Assert.That(fixture.Number, Is.EqualTo(number));
			});
		}
	}
}
