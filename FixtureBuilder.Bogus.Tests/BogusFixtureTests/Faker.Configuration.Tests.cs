namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal sealed class FakerConfigurationTests
    {
        #region Values

        [Test]
        public void With_Faker_SetsValueFromFixtureFaker()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<string>>("de");

            var result = fixture.With(x => x.Value, faker => faker.Locale).Build();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        [Test]
        public void With_Faker_IsInvokedForEachBuild()
        {
            var calls = 0;
            var fixture = Fixture.WithBogus<PropertyHolder<string>>().With(x => x.Value, _ => $"value {++calls}");

            var result = fixture.Build(2);

            var values = result.Select(holder => holder.Value);
            Assert.That(values, Is.EqualTo(["value 1", "value 2"]));
        }

        [Test]
        public void WithSetter_Faker_SetsValueFromFixtureFaker()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<string>>("de");

            var result = fixture.WithSetter(x => x.Value, faker => faker.Locale).Build();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        [Test]
        public void WithField_Faker_SetsFieldFromFixtureFaker()
        {
            var fixture = Fixture.WithBogus<FieldHolder<string>>("de");

            var result = fixture.WithField("Value", faker => faker.Locale).Build();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        [Test]
        public void WithField_FakerAndMemberExpression_SetsFieldOfMemberFromFixtureFaker()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<FieldHolder<string>>>("de");

            var result = fixture.WithField(x => x.Value, "Value", faker => faker.Locale).Build();

            Assert.That(result.Value.Value, Is.EqualTo("de"));
        }

        [Test]
        public void WithBackingField_Faker_SetsBackingFieldFromFixtureFaker()
        {
            var fixture = Fixture.WithBogus<ReadOnlyPropertyHolder<string>>("de");

            var result = fixture.WithBackingField(x => x.Value, faker => faker.Locale).Build();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        [Test]
        public void WithBackingField_FakerAndFieldName_SetsNamedFieldFromFixtureFaker()
        {
            var fixture = Fixture.WithBogus<ReadOnlyPropertyHolder<string>>("de");

            var result = fixture.WithBackingField(x => x.Value, faker => faker.Locale, "_value").Build();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        [Test]
        public void WithBackingFieldUntyped_Faker_SetsBackingFieldFromFixtureFaker()
        {
            var fixture = Fixture.WithBogus<ReadOnlyPropertyHolder<string>>("de");

            var result = fixture.WithBackingFieldUntyped(x => x.Value, faker => faker.Locale).Build();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        [Test]
        public void WithBackingFieldUntyped_FakerAndFieldName_SetsNamedFieldFromFixtureFaker()
        {
            var fixture = Fixture.WithBogus<ReadOnlyPropertyHolder<string>>("de");

            var result = fixture.WithBackingFieldUntyped(x => x.Value, faker => faker.Locale, "_value").Build();

            Assert.That(result.Value, Is.EqualTo("de"));
        }

        [Test]
        public void FakerAndPassthroughMethods_AppliesBoth()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<PropertyHolder<string>>>("de");

            var result = fixture.With(x => x.Value, new PropertyHolder<string>()).With(x => x.Value.Value, faker => faker.Locale).Build();

            Assert.That(result.Value.Value, Is.EqualTo("de"));
        }

        #endregion

        #region Methods

        [Test]
        public void Invoke_Faker_CallsMethodWithFixtureFaker()
        {
            var fixture = Fixture.WithBogus<RecordingMethodClass>("de");

            var result = fixture.Invoke(faker => x => x.Record(faker.Locale)).Build();

            Assert.That(result.Recorded, Is.EqualTo("de"));
        }

        [Test]
        public void InvokePrivate_Faker_PassesArgumentsFromFixtureFaker()
        {
            var fixture = Fixture.WithBogus<RecordingMethodClass>("de");

            var result = fixture.InvokePrivate("Record", faker => [faker.Locale]).Build();

            Assert.That(result.Recorded, Is.EqualTo("de"));
        }

        [Test]
        public void InvokePrivate_FakerAndMemberExpression_PassesArgumentsFromFixtureFakerToMember()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<RecordingMethodClass>>("de");

            var result = fixture.InvokePrivate(x => x.Value, "Record", faker => [faker.Locale]).Build();

            Assert.That(result.Value.Recorded, Is.EqualTo("de"));
        }

        #endregion
    }
}
