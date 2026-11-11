namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal sealed class BuildTests
    {
        #region Without instance

        [Test]
        public void ConstructsFixtureImplicitly()
        {
            var fixture = Fixture.WithBogus<ConstructorSetsValueClass>();

            var result = fixture.Build();

            Assert.That(result.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void Count_ReturnsThatManyInstances()
        {
            var fixture = Fixture.WithBogus<EmptyClass>();

            var result = fixture.Build(3);

            Assert.That(result, Has.Exactly(3).Items);
        }

        [Test]
        public void Count_ReturnsDistinctInstances()
        {
            var fixture = Fixture.WithBogus<EmptyClass>();

            var result = fixture.Build(3);

            Assert.That(result, Is.Unique);
        }

        [Test]
        public void Count_EnumeratedTwice_ReturnsSameInstances()
        {
            var fixture = Fixture.WithBogus<EmptyClass>();
            var result = fixture.Build(2);

            var first = result.ToList();
            var second = result.ToList();

            Assert.That(second, Is.EqualTo(first));
        }

        [Test]
        public void Count_AppliesConfigurationToEachInstance()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<string>>().With(x => x.Value, "value");

            var result = fixture.Build(2);

            var values = result.Select(holder => holder.Value);
            Assert.That(values, Is.EqualTo(["value", "value"]));
        }

        [Test]
        public void ZeroCount_ThrowsArgumentOutOfRangeException()
        {
            var fixture = Fixture.WithBogus<EmptyClass>();

            Assert.Throws<ArgumentOutOfRangeException>(() => fixture.Build(0));
        }

        [Test]
        public void NegativeCount_ThrowsArgumentOutOfRangeException()
        {
            var fixture = Fixture.WithBogus<EmptyClass>();

            Assert.Throws<ArgumentOutOfRangeException>(() => fixture.Build(-1));
        }

        #endregion

        #region With instance

        [Test]
        public void Instance_CountOne_ReturnsInstance()
        {
            var instance = new EmptyClass();
            var fixture = Fixture.WithBogus(instance);

            var result = fixture.Build(1);

            Assert.That(result, Is.EqualTo([instance]));
        }

        [Test]
        public void Instance_CountAboveOne_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.WithBogus(new EmptyClass());

            Assert.Throws<InvalidOperationException>(() => fixture.Build(2));
        }

        [Test]
        public void Instance_ZeroCount_ThrowsArgumentOutOfRangeException()
        {
            var fixture = Fixture.WithBogus(new EmptyClass());

            Assert.Throws<ArgumentOutOfRangeException>(() => fixture.Build(0));
        }

        #endregion
    }
}
