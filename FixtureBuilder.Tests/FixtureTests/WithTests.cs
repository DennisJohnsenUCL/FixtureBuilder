namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class WithTests
    {
        [Test]
        public void PropertyWithSetter_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<string>>();

            var result = fixture.With(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        private class UnconventionalFieldWithSetterClass
        {
            private string _unconventionalName = null!;
            public string Value { get => _unconventionalName; set => _unconventionalName = value; }
        }

        [Test]
        public void PropertyWithSetter_UnconventionalBackingField_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<UnconventionalFieldWithSetterClass>();

            var result = fixture.With(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void PropertyWithoutSetter_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyPropertyHolder<string>>();

            var result = fixture.With(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void PropertyWithoutSetter_WiderBackingField_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<WiderBackingFieldClass>();

            var result = fixture.With(x => x.Value, 123).Build();

            Assert.That(result.Value, Is.EqualTo(123));
        }

        [Test]
        public void PropertyWithoutSetter_UnconventionalBackingField_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<UnconventionalBackingFieldClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.With(x => x.Value, "value"));
        }

        [Test]
        public void Field_SetsField()
        {
            var fixture = TestHelper.MakeFixture<FieldHolder<string>>();

            var result = fixture.With(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void MethodCall_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<string>>();

            Assert.Throws<InvalidOperationException>(() => fixture.With(x => x.GetHashCode(), 123));
        }

        [Test]
        public void ExpressionEndingInNullableValue_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<PointStruct?>>();

            Assert.Throws<InvalidOperationException>(() => fixture.With(x => x.Value!.Value, new PointStruct()));
        }

        [Test]
        public void NotConstructed_ConstructsImplicitlyAndSetsProperty()
        {
            var fixture = Fixture.New<PropertyHolder<string>>();

            var result = fixture.With(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }
    }
}
