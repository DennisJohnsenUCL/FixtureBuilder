namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal sealed class CastToTests
    {
        private interface IFirstValue
        {
            string First { get; }
        }

        private interface ISecondValue
        {
            string Second { get; }
        }

        private class TwoInterfacesClass : IFirstValue, ISecondValue
        {
            private readonly string _first = null!;
            private readonly string _second = null!;

            public string Own { get; set; } = null!;
            string IFirstValue.First => _first;
            string ISecondValue.Second => _second;
        }

        #region Single cast

        [Test]
        public void BuildsInstanceOfOriginalType()
        {
            var fixture = Fixture.WithBogus<TwoInterfacesClass>().CastTo<IFirstValue>();

            var result = fixture.Build();

            Assert.That(result, Is.TypeOf<TwoInterfacesClass>());
        }

        [Test]
        public void ConfigurationBeforeCast_IsApplied()
        {
            var fixture = Fixture.WithBogus<TwoInterfacesClass>().With(x => x.Own, "own").CastTo<IFirstValue>();

            var result = (TwoInterfacesClass)fixture.Build();

            Assert.That(result.Own, Is.EqualTo("own"));
        }

        [Test]
        public void ConfigurationAfterCast_IsApplied()
        {
            var fixture = Fixture.WithBogus<TwoInterfacesClass>().CastTo<IFirstValue>().WithBackingField(x => x.First, "first");

            var result = fixture.Build();

            Assert.That(result.First, Is.EqualTo("first"));
        }

        [Test]
        public void FakerConfigurationBeforeAndAfterCast_UsesFixtureFaker()
        {
            var fixture = Fixture.WithBogus<TwoInterfacesClass>("de")
                .With(x => x.Own, faker => faker.Locale)
                .CastTo<IFirstValue>()
                .WithBackingField(x => x.First, faker => faker.Locale);

            var result = fixture.Build();

            var own = ((TwoInterfacesClass)result).Own;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(own, Is.EqualTo("de"));
                Assert.That(result.First, Is.EqualTo("de"));
            }
        }

        [Test]
        public void UnrelatedType_Build_ThrowsInvalidCastException()
        {
            var fixture = Fixture.WithBogus<TwoInterfacesClass>().CastTo<string>();

            Assert.Throws<InvalidCastException>(() => fixture.Build());
        }

        #endregion

        #region Several instances

        [Test]
        public void Count_ReturnsDistinctInstances()
        {
            var fixture = Fixture.WithBogus<TwoInterfacesClass>().CastTo<IFirstValue>();

            var result = fixture.Build(3);

            Assert.That(result, Is.Unique);
        }

        [Test]
        public void Count_AppliesConfigurationBeforeAndAfterCastToEachInstance()
        {
            var fixture = Fixture.WithBogus<TwoInterfacesClass>()
                .With(x => x.Own, "own")
                .CastTo<IFirstValue>()
                .WithBackingField(x => x.First, "first");

            var result = fixture.Build(2);

            var owns = result.Select(instance => ((TwoInterfacesClass)instance).Own);
            var firsts = result.Select(instance => instance.First);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(owns, Is.EqualTo(["own", "own"]));
                Assert.That(firsts, Is.EqualTo(["first", "first"]));
            }
        }

        [Test]
        public void Count_FakerConfigurationBeforeCast_IsInvokedForEachBuild()
        {
            var calls = 0;
            var fixture = Fixture.WithBogus<TwoInterfacesClass>().With(x => x.Own, _ => $"own {++calls}").CastTo<IFirstValue>();

            var result = fixture.Build(2);

            var owns = result.Select(instance => ((TwoInterfacesClass)instance).Own);
            Assert.That(owns, Is.EqualTo(["own 1", "own 2"]));
        }

        [Test]
        public void Count_FakerConfigurationAfterCast_IsInvokedForEachBuild()
        {
            var calls = 0;
            var fixture = Fixture.WithBogus<TwoInterfacesClass>().CastTo<IFirstValue>().WithBackingField(x => x.First, _ => $"first {++calls}");

            var result = fixture.Build(2);

            var firsts = result.Select(instance => instance.First);
            Assert.That(firsts, Is.EqualTo(["first 1", "first 2"]));
        }

        [Test]
        public void Instance_CountAboveOne_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.WithBogus(new TwoInterfacesClass()).CastTo<IFirstValue>();

            Assert.Throws<InvalidOperationException>(() => fixture.Build(2));
        }

        [Test]
        public void Instance_UnrelatedType_CountAboveOne_ThrowsInvalidCastException()
        {
            var fixture = Fixture.WithBogus(new TwoInterfacesClass()).CastTo<string>();

            Assert.Throws<InvalidCastException>(() => fixture.Build(2));
        }

        #endregion

        #region Cast twice

        [Test]
        public void CastTwice_AppliesConfigurationFromEveryStage()
        {
            var fixture = Fixture.WithBogus<TwoInterfacesClass>()
                .With(x => x.Own, "own")
                .CastTo<IFirstValue>()
                .WithBackingField(x => x.First, "first")
                .CastTo<ISecondValue>()
                .WithBackingField(x => x.Second, "second");

            var result = fixture.Build();

            var instance = (TwoInterfacesClass)result;
            var first = ((IFirstValue)instance).First;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(instance.Own, Is.EqualTo("own"));
                Assert.That(first, Is.EqualTo("first"));
                Assert.That(result.Second, Is.EqualTo("second"));
            }
        }

        [Test]
        public void CastTwice_Count_FakerConfigurationOfEveryStageIsInvokedForEachBuild()
        {
            var ownCalls = 0;
            var firstCalls = 0;
            var fixture = Fixture.WithBogus<TwoInterfacesClass>()
                .With(x => x.Own, _ => $"own {++ownCalls}")
                .CastTo<IFirstValue>()
                .WithBackingField(x => x.First, _ => $"first {++firstCalls}")
                .CastTo<ISecondValue>();

            var result = fixture.Build(2);

            var owns = result.Select(instance => ((TwoInterfacesClass)instance).Own);
            var firsts = result.Select(instance => ((IFirstValue)instance).First);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Unique);
                Assert.That(owns, Is.EqualTo(["own 1", "own 2"]));
                Assert.That(firsts, Is.EqualTo(["first 1", "first 2"]));
            }
        }

        #endregion
    }
}
