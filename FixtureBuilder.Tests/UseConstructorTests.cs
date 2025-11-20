using System.Reflection;

namespace FixtureBuilder.Tests
{
    internal sealed class UseConstructorTests
    {
        [Test]
        public void NoParameters_UsesDefaultConstructor()
        {
            var fixture = Fixture.New<DefaultConstructor>().UseConstructor().Build();

            Assert.That(!string.IsNullOrWhiteSpace(fixture.Value));
        }

        [Test]
        public void Parameters_UsesCorrectConstructor()
        {
            var text = "Test value";

            var fixture = Fixture.New<NoDefaultConstructor>().UseConstructor(text).Build();

            Assert.That(fixture.Value, Is.EqualTo(text));
        }

        [Test]
        public void NoParameters_NoDefaultConstructor_ThrowsException()
        {
            var fixture = Fixture.New<NoDefaultConstructor>();

            Assert.Throws<MissingMethodException>(() => fixture.UseConstructor());
        }

        [Test]
        public void Parameters_NoMatchingConstructor_ThrowsException()
        {
            var fixture = Fixture.New<NoDefaultConstructor>();

            Assert.Throws<MissingMethodException>(() => fixture.UseConstructor("Test value", 123));
        }

        [Test]
        public void GenericClass_CanConstruct()
        {
            IFixtureConfigurator<GenericClass<string>> fixture = null!;
            Assert.DoesNotThrow(() => fixture = Fixture.New<GenericClass<string>>().UseConstructor());
        }

        [Test]
        public void ClassWithMembers_InstantiatesClassMembers()
        {
            var fixture = Fixture.New<TestClass>().UseConstructor();

            var field = (TestClass)fixture.GetType().GetField("_fixture", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(fixture)!;

            Assert.That(field.NestedClass, Is.Not.Null);
        }
    }
}
