namespace FixtureBuilder.Tests
{
    internal class NewTests
    {
        [Test]
        public void ReturnsIFixtureConstructor()
        {
            var method = typeof(Fixture).GetMethod("New", []);

            Assert.That(method!.ReturnType.GetGenericTypeDefinition, Is.EqualTo(typeof(IFixtureConstructor<>)));
        }

        [Test]
        public void NoPrebuiltFixture_FixtureIsNull()
        {
            var fixture = Fixture.New<TestClass>();
            var field = Helpers.GetFixture(fixture);

            Assert.That(field, Is.Null);
        }

        class ClassWithString
        {
            public string Text = null!;
        }
        [Test]
        public void PreBuiltFixture_SetsFixture()
        {
            var text = "test string";

            var preBuiltFixture = new ClassWithString() { Text = text };

            var fixture = Fixture.New(preBuiltFixture);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(text));
        }

        [Test]
        public void GenericTypeRecord_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => Fixture.New<TestRecord>());
        }

        [Test]
        public void GenericTypeGenericClass_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => Fixture.New<GenericClass<string>>());
        }

        [Test]
        public void GenericTypeInterface_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<IList<string>>());
        }

        abstract class AbstractClass;
        [Test]
        public void GenericTypeAbstract_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<AbstractClass>());
        }

        [Test]
        public void PrebuiltFixtureNull_ThrowsException()
        {
            TestClass prebuilt = null!;

            Assert.Throws<ArgumentNullException>(() => Fixture.New(prebuilt));
        }

        [Test]
        public void PrebuiltFixture_TypeIsFixture_ThrowsException()
        {
            var prebuilt = Fixture.New<TestClass>();

            Assert.Throws<InvalidOperationException>(() => Fixture.New(prebuilt));
        }

        [Test]
        public void NewFixture_TypeIsFixture_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<Fixture<TestClass>>());
        }
    }
}
