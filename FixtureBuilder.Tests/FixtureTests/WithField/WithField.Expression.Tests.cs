namespace FixtureBuilder.Tests.FixtureTests.WithField
{
    internal class WithFieldExpressionTests
    {
        private const string _text = "Test string";

        class BaseClass
        {
            public NestedClass NestedClass { get; set; } = null!;
        }

        class NestedClass
        {
            public TwiceNestedClass TwiceNestedClass { get; set; } = null!;
        }

        class TwiceNestedClass : InheritedClass
        {
            private readonly string _field = null!;
            public string GetField => _field;
        }

        class InheritedClass
        {
            protected readonly string _inheritedField = null!;
            public string GetInheritedField => _inheritedField;
            private readonly string _privateInheritedField = null!;
            public string GetPrivateInheritedField => _privateInheritedField;
        }

        [Test]
        public void NestedProperty_SetsField()
        {
            var fieldName = "_field";
            var fixture = TestHelper.MakeFixture<BaseClass>();

            fixture.WithField(c => c.NestedClass.TwiceNestedClass, fieldName, _text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NestedClass.TwiceNestedClass.GetField, Is.EqualTo(_text));
        }

        [Test]
        public void InheritedField_SetsField()
        {
            var fieldName = "_inheritedField";
            var fixture = TestHelper.MakeFixture<BaseClass>();

            fixture.WithField(c => c.NestedClass.TwiceNestedClass, fieldName, _text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NestedClass.TwiceNestedClass.GetInheritedField, Is.EqualTo(_text));
        }

        [Test]
        public void InheritedPrivateField_SetsField()
        {
            var fieldName = "_privateInheritedField";
            var fixture = TestHelper.MakeFixture<BaseClass>();

            fixture.WithField(c => c.NestedClass.TwiceNestedClass, fieldName, _text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NestedClass.TwiceNestedClass.GetPrivateInheritedField, Is.EqualTo(_text));
        }

        [Test]
        public void ComputedProperty_ThrowsException()
        {
            var fieldName = "_field";
            var fixture = TestHelper.MakeFixture<BaseClass>();

            Assert.Throws<InvalidOperationException>(
                () => fixture.WithField(c => c.NestedClass.TwiceNestedClass.GetField.Length, fieldName, _text));
        }

        [Test]
        public void MethodAccessor_ThrowsException()
        {
            var fieldName = "_field";
            var fixture = TestHelper.MakeFixture<BaseClass>();

            Assert.Throws<InvalidOperationException>(
                () => fixture.WithField(c => c.NestedClass.TwiceNestedClass.GetHashCode(), fieldName, _text));
        }

        [Test]
        public void WithField_InstantiatesFixture()
        {
            var fieldName = "_field";

            var fixture = TestHelper.MakeFixture<BaseClass>();

            using (Assert.EnterMultipleScope())
            {
                Assert.DoesNotThrow(() => fixture.WithField(c => c.NestedClass.TwiceNestedClass, fieldName, "Hello"));
                Assert.That(TestHelper.GetFixture(fixture), Is.Not.Null);
            }
        }

        class BaseClassWithFieldIntermediate
        {
            public NestedClassWithField NestedClass = null!;
        }

        class NestedClassWithField
        {
            private readonly string _field = null!;
            public string GetField => _field;
        }

        [Test]
        public void FieldTerminator_SetsField()
        {
            var fieldName = "_field";
            var fixture = TestHelper.MakeFixture<BaseClassWithFieldIntermediate>();

            fixture.WithField(c => c.NestedClass, fieldName, _text);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.NestedClass.GetField, Is.EqualTo(_text));
        }

        class DeclaredAsBaseHolder
        {
            public InheritedClass Member { get; set; } = null!;
        }

        [Test]
        public void MemberHoldingDerivedInstance_SetsFieldDeclaredOnRuntimeType()
        {
            var fieldName = "_field";
            var fixture = TestHelper.MakeFixture<DeclaredAsBaseHolder>()
                .With(c => c.Member, new TwiceNestedClass());

            fixture.WithField(c => c.Member, fieldName, _text);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(((TwiceNestedClass)result.Member).GetField, Is.EqualTo(_text));
        }
    }
}
