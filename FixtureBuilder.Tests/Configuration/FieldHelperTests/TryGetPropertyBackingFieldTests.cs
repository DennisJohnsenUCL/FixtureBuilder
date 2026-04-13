#pragma warning disable CS0169

using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.FieldHelperTests
{
    internal sealed class TryGetPropertyBackingFieldTests
    {
        private class SimpleTarget
        {
            private readonly int _value;
            public int Value { get; set; }
        }

        private class AutoPropTarget
        {
            public string Name { get; set; } = null!;
        }

        private interface IWithProp
        {
            int Id { get; set; }
        }

        private class ImplementsInterface : IWithProp
        {
            public int Id { get; set; }
        }

        private class DerivedTarget : SimpleTarget { }

        [Test]
        public void TryGetPropertyBackingField_ExplicitFieldName_FindsField()
        {
            var property = typeof(SimpleTarget).GetProperty("Value")!;

            var result = FieldHelper.TryGetPropertyBackingField(typeof(SimpleTarget), property, "_value", out var backingField);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(backingField.Name, Is.EqualTo("_value"));
            }
        }

        [Test]
        public void TryGetPropertyBackingField_NullFieldName_FindsAutoPropertyBackingField()
        {
            var property = typeof(AutoPropTarget).GetProperty("Name")!;

            var result = FieldHelper.TryGetPropertyBackingField(typeof(AutoPropTarget), property, null, out var backingField);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(backingField, Is.Not.Null);
            }
        }

        [Test]
        public void TryGetPropertyBackingField_FieldOnDeclaringType_FallsBackToDeclaringType()
        {
            var property = typeof(SimpleTarget).GetProperty("Value")!;

            var result = FieldHelper.TryGetPropertyBackingField(typeof(DerivedTarget), property, "_value", out var backingField);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(backingField.Name, Is.EqualTo("_value"));
                Assert.That(backingField.DeclaringType, Is.EqualTo(typeof(SimpleTarget)));
            }
        }

        [Test]
        public void TryGetPropertyBackingField_InterfaceProperty_NullFieldName_FindsBackingField()
        {
            var interfaceProp = typeof(IWithProp).GetProperty("Id")!;

            var result = FieldHelper.TryGetPropertyBackingField(typeof(ImplementsInterface), interfaceProp, null, out var backingField);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(backingField, Is.Not.Null);
            }
        }

        [Test]
        public void TryGetPropertyBackingField_ExplicitFieldName_NotFound_ReturnsFalse()
        {
            var property = typeof(SimpleTarget).GetProperty("Value")!;

            var result = FieldHelper.TryGetPropertyBackingField(typeof(SimpleTarget), property, "nonexistent", out var backingField);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(backingField, Is.Null);
            }
        }

        [Test]
        public void TryGetPropertyBackingField_NullFieldName_NoMatchAnywhere_ReturnsFalse()
        {
            var property = typeof(IWithProp).GetProperty("Id")!;

            var result = FieldHelper.TryGetPropertyBackingField(typeof(SimpleTarget), property, null, out var backingField);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(backingField, Is.Null);
            }
        }
    }
}
