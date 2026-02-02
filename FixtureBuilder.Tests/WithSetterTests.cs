#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1822 // Mark members as static

namespace FixtureBuilder.Tests
{
    internal class WithSetterTests
    {
        private readonly static string _text = "Test string";
        private readonly static int _number = 123;

        [Test]
        public void SetterInFixture_SetsProperty()
        {
            var fixture = Fixture.New<TestClass>().BypassConstructor().WithSetter(t => t.Text, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
        }

        [Test]
        public void SetterInRecord_SetsProperty()
        {
            var fixture = Fixture.New<TestRecord>().BypassConstructor().WithSetter(t => t.Text, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
        }

        [Test]
        public void DerivedSetter_SetsProperty()
        {
            var fixture = Fixture.New<DerivedTestClass>().BypassConstructor().WithSetter(t => t.Text, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
        }

        [Test]
        public void OverriddenSetter_SetsProperty()
        {
            var fixture = Fixture.New<DerivedTestClass>().BypassConstructor().WithSetter(t => t.Number, _number);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Number, Is.EqualTo(_number));
        }

        [Test]
        public void ImplicitInterfaceImplementation_SetsProperty()
        {
            var fixture = Fixture.New<InterfaceTestClass>().BypassConstructor().WithSetter(t => t.ImplicitProperty, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ImplicitProperty, Is.EqualTo(_text));
        }

        class PropWithoutSetterClass
        {
            public string Text => "Test";
        }
        [Test]
        public void PropWithoutSetter_ThrowsException()
        {
            var fixture = Fixture.New<PropWithoutSetterClass>().BypassConstructor();
            var field = Helpers.GetFixture(fixture);

            Assert.Throws<InvalidOperationException>(() => fixture.WithSetter(f => f.Text, "Test"));
        }


        [Test]
        public void GenericClass_SetsProperty()
        {
            var fixture = Fixture.New<GenericClass<string>>().BypassConstructor().WithSetter(g => g.Value, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Value, Is.EqualTo(_text));
        }

        [Test]
        public void NestedProperty_SetsProperty()
        {
            var fixture = Fixture.New<TestClass>().BypassConstructor().WithSetter(t => t.NestedClass.Value, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.NestedClass.Value, Is.EqualTo(_text));
        }

        [Test]
        public void DeeperNestedProperty_SetsProperty()
        {
            var fixture = Fixture.New<TestClass>().BypassConstructor().WithSetter(t => t.NestedClass.DeeperNestedClass.Value, _number);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.NestedClass.DeeperNestedClass.Value, Is.EqualTo(_number));
        }

        [Test]
        public void DerivedNestedProperty_SetsProperty()
        {
            var fixture = Fixture.New<DerivedTestClass>().BypassConstructor().WithSetter(t => t.NestedClass.Value, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.NestedClass.Value, Is.EqualTo(_text));
        }

        class PrivateSetterClass
        {
            public string Text { get; private set; } = null!;
        }
        [Test]
        public void PrivateSetter_CanSetValue()
        {
            var fixture = Fixture.New<PrivateSetterClass>().BypassConstructor().WithSetter(t => t.Text, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
        }

        class ListPropertyClass
        {
            public List<string> StringList { get; set; } = null!;
        }
        [Test]
        public void CollectionTypeProperty_OneParameter_SetsProperty()
        {
            var fixture = Fixture.New<ListPropertyClass>().BypassConstructor().WithSetter(t => t.StringList, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.StringList[0], Is.EqualTo(_text));
        }

        [Test]
        public void CollectionTypeProperty_TwoParameters_SetsProperty()
        {
            var secondEntry = "More test";
            var fixture = Fixture.New<ListPropertyClass>().BypassConstructor().WithSetter(t => t.StringList, [_text, secondEntry]);
            var field = Helpers.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.StringList[0], Is.EqualTo(_text));
                Assert.That(field.StringList[1], Is.EqualTo(secondEntry));
            }
        }

        [Test]
        public void ConstructionNotChosen_InstantiatesNonNullables()
        {
            var fixture = Fixture.New<ClassWithNullable>().WithSetter(c => c.Text, "test");
            var field = Helpers.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.NullableClass, Is.Null);
                Assert.That(field.NonNullableClass, Is.Not.Null);
            }
        }

        [Test]
        public void NoMemberAccess_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<TestClass>().WithSetter(c => c, new TestClass()));
        }
    }
}
