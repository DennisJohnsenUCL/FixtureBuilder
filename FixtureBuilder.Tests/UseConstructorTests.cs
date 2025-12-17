#pragma warning disable IDE0290 // Use primary constructor

namespace FixtureBuilder.Tests
{
    internal sealed class UseConstructorTests
    {
        class PrivateConstructorClass
        {
            private PrivateConstructorClass() { }
        }
        [Test]
        public void PrivateConstructor_UsesConstructor()
        {
            IFixtureConfigurator<PrivateConstructorClass> fixture = null!;
            Assert.DoesNotThrow(() => fixture = Fixture.New<PrivateConstructorClass>().UseConstructor());

            Assert.That(fixture, Is.Not.Null);
        }

        class DefaultConstructorClass
        {
            public string Value { get; set; }
            public DefaultConstructorClass() => Value = "Something";
        }
        [Test]
        public void NoParameters_UsesDefaultConstructor()
        {
            var fixture = Fixture.New<DefaultConstructorClass>().UseConstructor();
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Value, Is.EqualTo("Something"));
        }

        class NoDefaultConstructor
        {
            public string Value { get; set; }
            public NoDefaultConstructor(string value) => Value = value;
        }
        [Test]
        public void Parameters_UsesCorrectConstructor()
        {
            var text = "Test value";

            var fixture = Fixture.New<NoDefaultConstructor>().UseConstructor(text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Value, Is.EqualTo(text));
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

        class GenericClass<T>();
        [Test]
        public void GenericClass_CanConstruct()
        {
            IFixtureConfigurator<GenericClass<string>> fixture = null!;

            Assert.DoesNotThrow(() => fixture = Fixture.New<GenericClass<string>>().UseConstructor());

            var field = Helpers.GetFixture(fixture);

            Assert.That(field, Is.Not.Null);
        }

        class ClassWithMember
        {
            public NestedClass NestedClass = null!;
        }
        [Test]
        public void ClassWithMembers_InstantiatesClassMembers()
        {
            var fixture = Fixture.New<ClassWithMember>().UseConstructor();

            var field = Helpers.GetFixture(fixture);

            Assert.That(field.NestedClass, Is.Not.Null);
        }
    }
}
