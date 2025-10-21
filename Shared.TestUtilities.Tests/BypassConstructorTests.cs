using System.Reflection;

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

		[Test]
		public void ClassWithMembers_InstantiatesClassMembers()
		{
			var fixture = FixtureBuilder.New<TestClass>().BypassConstructor();

			var field = (TestClass)fixture.GetType().GetField("_fixture", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(fixture)!;

			Assert.That(field.NestedClass, Is.Not.Null);
		}
	}
}
