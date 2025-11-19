using Shared.TestUtilities.Fixtures;
using System.Reflection;

namespace Shared.TestUtilities.Tests.Fixtures
{
	internal sealed class UseConstructorTests
	{
		[Test]
		public void NoParameters_UsesDefaultConstructor()
		{
			var fixture = FixtureBuilder.New<DefaultConstructor>().UseConstructor().Build();

			Assert.That(!string.IsNullOrWhiteSpace(fixture.Value));
		}

		[Test]
		public void Parameters_UsesCorrectConstructor()
		{
			var text = "Test value";

			var fixture = FixtureBuilder.New<NoDefaultConstructor>().UseConstructor(text).Build();

			Assert.That(fixture.Value, Is.EqualTo(text));
		}

		[Test]
		public void NoParameters_NoDefaultConstructor_ThrowsException()
		{
			var fixture = FixtureBuilder.New<NoDefaultConstructor>();

			Assert.Throws<MissingMethodException>(() => fixture.UseConstructor());
		}

		[Test]
		public void Parameters_NoMatchingConstructor_ThrowsException()
		{
			var fixture = FixtureBuilder.New<NoDefaultConstructor>();

			Assert.Throws<MissingMethodException>(() => fixture.UseConstructor("Test value", 123));
		}

		[Test]
		public void GenericClass_CanConstruct()
		{
			IFixtureConfigurator<GenericClass<string>> fixture = null!;
			Assert.DoesNotThrow(() => fixture = FixtureBuilder.New<GenericClass<string>>().UseConstructor());
		}

		[Test]
		public void ClassWithMembers_InstantiatesClassMembers()
		{
			var fixture = FixtureBuilder.New<TestClass>().UseConstructor();

			var field = (TestClass)fixture.GetType().GetField("_fixture", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(fixture)!;

			Assert.That(field.NestedClass, Is.Not.Null);
		}
	}
}
