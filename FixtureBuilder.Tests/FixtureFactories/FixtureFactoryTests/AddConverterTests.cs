using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class AddConverterTests
    {
        [Test]
        public void AddedConverter_ConvertsValueForBackingField()
        {
            var factory = new FixtureFactory().AddConverter(new FixedLongConverter(456));
            var fixture = TestHelper.MakeFixture<WiderBackingFieldClass>(factory);

            var result = fixture.WithBackingField(x => x.Value, 123).Build();

            Assert.That(result.Value, Is.EqualTo(456));
        }

        [Test]
        public void LaterConverter_WinsOverEarlierConverter()
        {
            var factory = new FixtureFactory()
                .AddConverter(new FixedLongConverter(1))
                .AddConverter(new FixedLongConverter(2));
            var fixture = TestHelper.MakeFixture<WiderBackingFieldClass>(factory);

            var result = fixture.WithBackingField(x => x.Value, 123).Build();

            Assert.That(result.Value, Is.EqualTo(2));
        }

        private class NullConverter : ICustomConverter
        {
            public object? Convert(Type target, object value) => null;
        }

        [Test]
        public void ConverterReturnsNullForNonNullableValueTypeField_ThrowsInvalidCastException()
        {
            var factory = new FixtureFactory().AddConverter(new NullConverter());
            var fixture = TestHelper.MakeFixture<WiderBackingFieldClass>(factory);

            Assert.Throws<InvalidCastException>(() => fixture.WithBackingField(x => x.Value, 123));
        }

        [Test]
        public void NullConverter_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            var ex = Assert.Throws<ArgumentNullException>(() => factory.AddConverter(null!));

            Assert.That(ex.ParamName, Is.EqualTo("converter"));
        }
    }
}
