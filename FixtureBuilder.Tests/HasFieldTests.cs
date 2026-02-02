namespace FixtureBuilder.Tests
{
    internal sealed class HasFieldTests
    {
        class FieldClass
        {
            public string _existingField = null!;
        }
        class DerivedFieldClass : FieldClass;

        [Test]
        public void FieldDoesNotExist_ReturnsFalse()
        {
            var fixture = Fixture.New<FieldClass>().BypassConstructor();

            var exists = fixture.HasField("_notExistingField");

            Assert.That(exists, Is.False);
        }

        [Test]
        public void FieldExists_ReturnsTrue()
        {
            var fixture = Fixture.New<FieldClass>().BypassConstructor();

            var exists = fixture.HasField("_existingField");

            Assert.That(exists, Is.True);
        }

        [Test]
        public void DerivedType_FieldExists_ReturnsTrue()
        {
            var fixture = Fixture.New<DerivedFieldClass>().BypassConstructor();

            var exists = fixture.HasField("_existingField");

            Assert.That(exists, Is.True);
        }

        [Test]
        public void WithExpression_FieldDoesNotExist_ReturnsFalse()
        {
            var fixture = Fixture.New<TestClass>().BypassConstructor();

            var exists = fixture.HasField("_notExistingField", c => c.NestedClass.DeeperNestedClass);

            Assert.That(exists, Is.False);
        }

        [Test]
        public void WithExpression_FieldExists_ReturnsTrue()
        {
            var fixture = Fixture.New<TestClass>().BypassConstructor();

            var exists = fixture.HasField("_privateField", c => c.NestedClass.DeeperNestedClass);

            Assert.That(exists, Is.True);
        }

        [Test]
        public void WithExpression_DerivedType_FieldExists_ReturnsTrue()
        {
            var fixture = Fixture.New<TestClass>().BypassConstructor();

            var exists = fixture.HasField("_privateField", c => c.NestedClass.DerivedNestedClass);

            Assert.That(exists, Is.True);
        }

        [Test]
        public void NoMemberAccess_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<TestClass>().HasField("_inheritedField", c => c));
        }
    }
}
