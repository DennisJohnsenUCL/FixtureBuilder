namespace FixtureBuilder.Tests.FixtureTests.WithField
{
    internal sealed class WithFieldTests
    {
        [Test]
        public void PrivateField_SetsField()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<string>>();

            var result = fixture.WithField("_value", "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void NullValue_SetsNull()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<string>>();
            fixture.WithField("_value", "value");

            var result = fixture.WithField<string>("_value", null!).Build();

            Assert.That(result.Value, Is.Null);
        }

        [Test]
        public void UnknownField_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<string>>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithField("_unknown", "value"));
        }

        [Test]
        public void ValueOfWrongType_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<string>>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithField("_value", 123));
        }

        [Test]
        public void NullFieldName_ThrowsArgumentNullException()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<string>>();

            var ex = Assert.Throws<ArgumentNullException>(() => fixture.WithField(null!, "value"));

            Assert.That(ex.ParamName, Is.EqualTo("fieldName"));
        }

        [Test]
        public void InheritedProtectedField_SetsField()
        {
            var fixture = TestHelper.MakeFixture<DerivedFromProtectedFieldClass>();

            var result = fixture.WithField("_value", "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void PrivateFieldInheritedTwoLevelsUp_SetsField()
        {
            var fixture = TestHelper.MakeFixture<GrandchildOfPrivateFieldClass>();

            var result = fixture.WithField("_value", "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        private record PrivateFieldRecord
        {
            private readonly string _value = null!;
            public string Value => _value;
        }

        [Test]
        public void PrivateFieldOfRecord_SetsField()
        {
            var fixture = TestHelper.MakeFixture<PrivateFieldRecord>();

            var result = fixture.WithField("_value", "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void ListField_ListValue_SetsField()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<List<string>>>();

            var result = fixture.WithField<List<string>>("_value", ["first", "second"]).Build();

            Assert.That(result.Value, Is.EqualTo(["first", "second"]));
        }

        [Test]
        public void ListField_ElementValue_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<List<string>>>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithField("_value", "value"));
        }

        [Test]
        public void NotConstructed_ConstructsImplicitlyAndSetsField()
        {
            var fixture = Fixture.New<ReadOnlyPropertyHolder<string>>();

            var result = fixture.WithField("_value", "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }
    }
}
