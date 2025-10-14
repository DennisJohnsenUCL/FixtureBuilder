namespace TestUtilities.Tests
{
	internal sealed class BypassConstructorTests
	{
		[Test]
		public void GenericClass_CanConstruct()
		{
			IStepTwo<GenericClass<string>> fixture = null!;
			Assert.DoesNotThrow(() => fixture = FixtureBuilder.New<GenericClass<string>>().BypassConstructor());
		}
	}
}
