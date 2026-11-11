using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureTests.UseAutoConstructor
{
    internal sealed class UseAutoConstructorConstructorsTests
    {
        private class SeveralPublicConstructorsClass
        {
            public string UsedConstructor { get; }

            public SeveralPublicConstructorsClass() => UsedConstructor = "no parameters";
            public SeveralPublicConstructorsClass(string text) => UsedConstructor = "one parameter";
            public SeveralPublicConstructorsClass(string text, int number) => UsedConstructor = "two parameters";
        }

        [Test]
        public void SeveralPublicConstructors_UsesSimplest()
        {
            var fixture = Fixture.New<SeveralPublicConstructorsClass>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.UsedConstructor, Is.EqualTo("no parameters"));
        }

        private class SimplerPrivateConstructorClass
        {
            public string UsedConstructor { get; }

            public SimplerPrivateConstructorClass(string text) => UsedConstructor = "public";
            private SimplerPrivateConstructorClass() => UsedConstructor = "private";
        }

        [TestCase(true, true, "private")]
        [TestCase(true, false, "public")]
        [TestCase(false, true, "public")]
        [TestCase(false, false, "public")]
        public void SimplerPrivateConstructor_UsesConstructorChosenByOptions(bool allowPrivateConstructors, bool preferSimplestConstructor, string expectedConstructor)
        {
            var options = new FixtureOptions
            {
                AllowPrivateConstructors = allowPrivateConstructors,
                PreferSimplestConstructor = preferSimplestConstructor
            };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<SimplerPrivateConstructorClass>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.UsedConstructor, Is.EqualTo(expectedConstructor));
        }

        [Test]
        public void OnlyPrivateConstructor_UsesIt()
        {
            var fixture = Fixture.New<PrivateConstructorClass>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void OnlyPrivateConstructor_PrivateConstructorsNotAllowed_ThrowsInvalidOperationException()
        {
            var options = new FixtureOptions { AllowPrivateConstructors = false };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<PrivateConstructorClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());
        }

        private class ThrowingConstructorClass
        {
            public ThrowingConstructorClass() => throw new FormatException();
        }

        [Test]
        public void ThrowingConstructor_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<ThrowingConstructorClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());
        }
    }
}
