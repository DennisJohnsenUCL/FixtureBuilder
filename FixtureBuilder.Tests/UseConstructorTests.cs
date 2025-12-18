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

        [Test]
        public void GenericClass_CanConstruct()
        {
            IFixtureConfigurator<GenericClass<string>> fixture = null!;

            Assert.DoesNotThrow(() => fixture = Fixture.New<GenericClass<string>>().UseConstructor());

            var field = Helpers.GetFixture(fixture);

            Assert.That(field, Is.Not.Null);
        }

        record EmptyRecord;
        [Test]
        public void Record_CanConstruct()
        {
            IFixtureConfigurator<EmptyRecord> fixture = null!;

            Assert.DoesNotThrow(() => fixture = Fixture.New<EmptyRecord>().UseConstructor());

            var field = Helpers.GetFixture(fixture);

            Assert.That(field, Is.Not.Null);
        }

        record StringRecord(string Text);
        [Test]
        public void RecordWithParameter_CanConstruct()
        {
            IFixtureConfigurator<StringRecord> fixture = null!;

            Assert.DoesNotThrow(() => fixture = Fixture.New<StringRecord>().UseConstructor("Test"));

            var field = Helpers.GetFixture(fixture);

            Assert.That(field, Is.Not.Null);
        }

        [Test]
        public void ClassWithMembers_InstantiatesNonNullables()
        {
            var fixture = Fixture.New<ClassWithNullable>().UseConstructor();
            var field = Helpers.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.NullableClass, Is.Null);
                Assert.That(field.NonNullableClass, Is.Not.Null);
            }
        }
    }
}
