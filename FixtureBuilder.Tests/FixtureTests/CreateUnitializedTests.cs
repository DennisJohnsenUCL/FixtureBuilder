namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class CreateUninitializedTests
    {
        class NormalClass();
        [Test]
        public void Class_CanConstruct()
        {
            IFixtureConfigurator<NormalClass> fixture = null!;

            Assert.DoesNotThrow(() => fixture = Fixture.New<NormalClass>().CreateUninitialized());

            var field = Helpers.GetFixture(fixture);

            Assert.That(field, Is.Not.Null);
        }

        [Test]
        public void Record_CanConstruct()
        {
            IFixtureConfigurator<TestRecord> fixture = null!;

            Assert.DoesNotThrow(() => fixture = Fixture.New<TestRecord>().CreateUninitialized());

            var field = Helpers.GetFixture(fixture);

            Assert.That(field, Is.Not.Null);
        }

        [Test]
        public void GenericClass_CanConstruct()
        {
            IFixtureConfigurator<GenericClass<string>> fixture = null!;
            Assert.DoesNotThrow(() => fixture = Fixture.New<GenericClass<string>>().CreateUninitialized());

            var field = Helpers.GetFixture(fixture);

            Assert.That(field, Is.Not.Null);
        }

        class ReadOnlyFieldClass()
        {
            public readonly string ReadOnlyField = null!;
        }
        [Test]
        public void ReadOnlyFieldNotSet_HasDefaultValue()
        {
            var fixture = Fixture.New<ReadOnlyFieldClass>().CreateUninitialized();
            var field = Helpers.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.ReadOnlyField, Is.Not.Null);
                Assert.That(field.ReadOnlyField, Is.EqualTo(""));
            }
        }

        [Test]
        public void ClassWithMembers_InstantiatesNonNullables()
        {
            var fixture = Fixture.New<ClassWithNullable>().CreateUninitialized();
            var field = Helpers.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.NullableClass, Is.Null);
                Assert.That(field.NonNullableClass, Is.Not.Null);
            }
        }
    }
}
