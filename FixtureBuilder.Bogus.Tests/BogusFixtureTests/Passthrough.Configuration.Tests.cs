namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal sealed class PassthroughConfigurationTests
    {
        #region Values

        [Test]
        public void With_SetsValue()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<string>>();

            var result = fixture.With(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void WithSetter_SetsValue()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<string>>();

            var result = fixture.WithSetter(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void WithField_SetsField()
        {
            var fixture = Fixture.WithBogus<FieldHolder<string>>();

            var result = fixture.WithField("Value", "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void WithField_MemberExpression_SetsFieldOfMember()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<FieldHolder<string>>>();

            var result = fixture.WithField(x => x.Value, "Value", "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        [Test]
        public void WithBackingField_SetsBackingField()
        {
            var fixture = Fixture.WithBogus<ReadOnlyPropertyHolder<string>>();

            var result = fixture.WithBackingField(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void WithBackingField_FieldName_SetsNamedField()
        {
            var fixture = Fixture.WithBogus<ReadOnlyPropertyHolder<string>>();

            var result = fixture.WithBackingField(x => x.Value, "value", "_value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void WithBackingFieldUntyped_SetsBackingField()
        {
            var fixture = Fixture.WithBogus<ReadOnlyPropertyHolder<string>>();

            var result = fixture.WithBackingFieldUntyped(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void WithBackingFieldUntyped_FieldName_SetsNamedField()
        {
            var fixture = Fixture.WithBogus<ReadOnlyPropertyHolder<string>>();

            var result = fixture.WithBackingFieldUntyped(x => x.Value, "value", "_value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void SeveralConfigurationMethods_AppliesAll()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<PropertyHolder<string>>>();

            var result = fixture.With(x => x.Value, new PropertyHolder<string>()).With(x => x.Value.Value, "value").Build();

            Assert.That(result.Value.Value, Is.EqualTo("value"));
        }

        #endregion

        #region Methods

        [Test]
        public void Invoke_CallsMethod()
        {
            var fixture = Fixture.WithBogus<RecordingMethodClass>();

            var result = fixture.Invoke(x => x.Record("value")).Build();

            Assert.That(result.Recorded, Is.EqualTo("value"));
        }

        [Test]
        public void InvokePrivate_CallsNamedMethod()
        {
            var fixture = Fixture.WithBogus<RecordingMethodClass>();

            var result = fixture.InvokePrivate("Record", "value").Build();

            Assert.That(result.Recorded, Is.EqualTo("value"));
        }

        [Test]
        public void InvokePrivate_MemberExpression_CallsNamedMethodOnMember()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<RecordingMethodClass>>();

            var result = fixture.InvokePrivate(x => x.Value, "Record", "value").Build();

            Assert.That(result.Value.Recorded, Is.EqualTo("value"));
        }

        #endregion
    }
}
