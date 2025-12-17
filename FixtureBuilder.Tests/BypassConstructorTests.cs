namespace FixtureBuilder.Tests
{
    internal sealed class BypassConstructorTests
    {
        class NormalClass();
        [Test]
        public void Class_CanConstruct()
        {
            IFixtureConfigurator<NormalClass> fixture = null!;

            Assert.DoesNotThrow(() => fixture = Fixture.New<NormalClass>().BypassConstructor());

            var field = Helpers.GetFixture(fixture);

            Assert.That(field, Is.Not.Null);
        }

        class GenericClass<T>();
        [Test]
        public void GenericClass_CanConstruct()
        {
            IFixtureConfigurator<GenericClass<string>> fixture = null!;
            Assert.DoesNotThrow(() => fixture = Fixture.New<GenericClass<string>>().BypassConstructor());

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
            var fixture = Fixture.New<ClassWithMember>().BypassConstructor();

            var field = Helpers.GetFixture(fixture);

            Assert.That(field.NestedClass, Is.Not.Null);
        }

        class ReadOnlyFieldClass()
        {
            public readonly string ReadOnlyField = null!;
        }
        [Test]
        public void ReadOnlyFieldNotSet_HasDefaultValue()
        {
            var fixture = Fixture.New<ReadOnlyFieldClass>().BypassConstructor();
            var field = Helpers.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.ReadOnlyField, Is.Not.Null);
                Assert.That(field.ReadOnlyField, Is.EqualTo(""));
            }
        }
    }
}
