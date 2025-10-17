namespace Shared.TestUtilities.Tests
{
	internal sealed class BypassConstructorTests
	{
		[Test]
		public void GenericClass_CanConstruct()
		{
			IFixtureConfigurator<GenericClass<string>> fixture = null!;
			Assert.DoesNotThrow(() => fixture = FixtureBuilder.New<GenericClass<string>>().BypassConstructor());
		}
	}
}
