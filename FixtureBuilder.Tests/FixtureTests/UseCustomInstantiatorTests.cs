namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class UseCustomInstantiatorTests
    {
        [Test]
        public void BuildsInstantiatorInstance()
        {
            var instance = new PropertyHolder<string>();
            var fixture = Fixture.New<PropertyHolder<string>>();

            var result = fixture.UseCustomInstantiator(() => instance).Build();

            Assert.That(result, Is.SameAs(instance));
        }

        [Test]
        public void BuildCalledTwice_ReturnsSameInstance()
        {
            var fixture = Fixture.New<PropertyHolder<string>>();
            fixture.UseCustomInstantiator(() => new PropertyHolder<string>());

            var first = fixture.Build();
            var second = fixture.Build();

            Assert.That(second, Is.SameAs(first));
        }

        private class TwoPropertiesClass
        {
            public string First { get; set; } = null!;
            public string Second { get; set; } = null!;
        }

        [Test]
        public void FollowedByConfiguration_KeepsInstantiatorValuesAndAppliesConfiguration()
        {
            var fixture = Fixture.New<TwoPropertiesClass>();

            var result = fixture
                .UseCustomInstantiator(() => new TwoPropertiesClass { First = "instantiator" })
                .With(x => x.Second, "configured")
                .Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.First, Is.EqualTo("instantiator"));
                Assert.That(result.Second, Is.EqualTo("configured"));
            }
        }

        [Test]
        public void FollowedByConfiguration_ConfigurationOverridesInstantiatorValue()
        {
            var fixture = Fixture.New<PropertyHolder<string>>();

            var result = fixture
                .UseCustomInstantiator(() => new PropertyHolder<string> { Value = "instantiator" })
                .With(x => x.Value, "configured")
                .Build();

            Assert.That(result.Value, Is.EqualTo("configured"));
        }

        [Test]
        public void InstantiatorReturnsNull_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<PropertyHolder<string>>();

            Assert.Throws<InvalidOperationException>(() => fixture.UseCustomInstantiator(() => null!));
        }

        [Test]
        public void NullInstantiator_ThrowsArgumentNullException()
        {
            var fixture = Fixture.New<PropertyHolder<string>>();

            var ex = Assert.Throws<ArgumentNullException>(() => fixture.UseCustomInstantiator(null!));

            Assert.That(ex.ParamName, Is.EqualTo("instantiator"));
        }

        [Test]
        public void AlreadyConstructed_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<PropertyHolder<string>>();
            fixture.UseCustomInstantiator(() => new PropertyHolder<string>());

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseCustomInstantiator(() => new PropertyHolder<string>()));

            Assert.That(ex.Message, Does.Contain("already constructed"));
        }
    }
}
