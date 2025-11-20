using System.Reflection;

namespace FixtureBuilder.Tests
{
    internal sealed class BypassConstructorTests
    {
        [Test]
        public void GenericClass_CanConstruct()
        {
            IFixtureConfigurator<GenericClass<string>> fixture = null!;
            Assert.DoesNotThrow(() => fixture = Fixture.New<GenericClass<string>>().BypassConstructor());
        }

        [Test]
        public void ClassWithMembers_InstantiatesClassMembers()
        {
            var fixture = Fixture.New<TestClass>().BypassConstructor();

            var field = (TestClass)fixture.GetType().GetField("_fixture", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(fixture)!;

            Assert.That(field.NestedClass, Is.Not.Null);
        }
    }
}
