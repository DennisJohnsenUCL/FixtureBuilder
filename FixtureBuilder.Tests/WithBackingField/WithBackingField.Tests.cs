#pragma warning disable CA1822 // Mark members as static

namespace FixtureBuilder.Tests.WithBackingField
{
    internal sealed class WithBackingFieldTests
    {
        private readonly static string _text = "Test string";
        private readonly static int _number = 123;

        [Test]
        public void RecordProperty_SetsProperty()
        {
            var fixture = Fixture.New<TestRecord>().BypassConstructor().WithBackingField(t => t.Text, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
        }

        [Test]
        public void RecordProperties_SetsProperties()
        {
            var fixture = Fixture.New<TestRecord>().BypassConstructor().WithBackingField(t => t.Text, _text).WithBackingField(t => t.Number, _number);
            var field = Helpers.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.Text, Is.EqualTo(_text));
                Assert.That(field.Number, Is.EqualTo(_number));
            }
        }

        [Test]
        public void NotARecordProperty_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<TestRecord>().BypassConstructor().WithBackingField(t => t.GetHashCode(), _number));
        }

        [Test]
        public void NoRecordPropertyBackingField_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<TestRecord>().BypassConstructor().WithBackingField(t => t.Text.Length, _number));
        }

        class NormalClass
        {
            public string Text { get; set; } = null!;
            public int Number { get; set; }
        }
        [Test]
        public void ClassProperty_SetsProperty()
        {
            var fixture = Fixture.New<TestClass>().BypassConstructor().WithBackingField(t => t.Text, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
        }

        [Test]
        public void ClassProperties_SetsProperties()
        {
            var fixture = Fixture.New<TestClass>().BypassConstructor().WithBackingField(t => t.Text, _text).WithBackingField(t => t.Number, _number);
            var field = Helpers.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.Text, Is.EqualTo(_text));
                Assert.That(field.Number, Is.EqualTo(_number));
            }
        }

        [Test]
        public void NotAClassProperty_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<TestClass>().BypassConstructor().WithBackingField(t => t.GetHashCode(), _number));
        }

        [Test]
        public void NoClassPropertyBackingField_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<TestClass>().BypassConstructor().WithBackingField(t => t.Text.Length, _number));
        }

        [Test]
        public void ExplicitBackingField_SetsProperty()
        {
            var fixture = Fixture.New<ExplicitBackingFieldClass>().BypassConstructor().WithBackingField(t => t.Text, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
        }

        class ExplicitBackingFieldNoUnderscoreClass
        {
            private readonly string privateExplicitField = null!;
            public string PrivateExplicitField => privateExplicitField;
        }
        [Test]
        public void ExplicitBackingFieldNoUnderscore_SetsProperty()
        {
            var fixture = Fixture.New<ExplicitBackingFieldNoUnderscoreClass>().BypassConstructor().WithBackingField(t => t.PrivateExplicitField, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.PrivateExplicitField, Is.EqualTo(_text));
        }

        [Test]
        public void DerivedProperty_SetsProperty()
        {
            var fixture = Fixture.New<DerivedTestClass>().BypassConstructor().WithBackingField(t => t.Text, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
        }

        [Test]
        public void OverriddenProperty_SetsProperty()
        {
            var fixture = Fixture.New<DerivedTestClass>().BypassConstructor().WithBackingField(t => t.Number, _number);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Number, Is.EqualTo(_number));
        }

        [Test]
        public void ImplicitInterfaceImplementation_SetsProperty()
        {
            var fixture = Fixture.New<InterfaceTestClass>().BypassConstructor().WithBackingField(t => t.ImplicitProperty, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ImplicitProperty, Is.EqualTo(_text));
        }

        [Test]
        public void TwiceDerivedClass_PropertyInDerivedClass_SetsProperty()
        {
            var fixture = Fixture.New<TwiceDerivedClass>().BypassConstructor().WithBackingField(p => p.Number, _number);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Number, Is.EqualTo(_number));
        }

        [Test]
        public void GenericClass_SetsProperty()
        {
            var fixture = Fixture.New<GenericClass<string>>().BypassConstructor().WithBackingField(g => g.Value, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Value, Is.EqualTo(_text));
        }

        class NestedPropertyClass
        {
            public NestedClass NestedClass { get; set; } = null!;
        }
        [Test]
        public void NestedProperty_SetsProperty()
        {
            var fixture = Fixture.New<NestedPropertyClass>().BypassConstructor().WithBackingField(t => t.NestedClass.Value, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.NestedClass.Value, Is.EqualTo(_text));
        }

        [Test]
        public void DerivedNestedProperty_SetsProperty()
        {
            var fixture = Fixture.New<DerivedTestClass>().BypassConstructor().WithBackingField(t => t.NestedClass.Value, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.NestedClass.Value, Is.EqualTo(_text));
        }

        [Test]
        public void DeeperNestedProperty_SetsProperty()
        {
            var fixture = Fixture.New<NestedPropertyClass>().BypassConstructor().WithBackingField(t => t.NestedClass.DeeperNestedClass.Value, _number);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.NestedClass.DeeperNestedClass.Value, Is.EqualTo(_number));
        }

        [Test]
        public void DeeperDerivedNestedProperty_Overriden_SetsProperty()
        {
            var fixture = Fixture.New<NestedPropertyClass>().BypassConstructor().WithBackingField(t => t.NestedClass.DerivedNestedClass.Value, _number);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.NestedClass.DerivedNestedClass.Value, Is.EqualTo(_number));
        }

        [Test]
        public void DeeperDerivedNestedProperty_NotOverriden_SetsProperty()
        {
            var fixture = Fixture.New<NestedPropertyClass>().BypassConstructor().WithBackingField(t => t.NestedClass.DerivedNestedClass.String, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.NestedClass.DerivedNestedClass.String, Is.EqualTo(_text));
        }

        [Test]
        public void FieldNameGiven_DeeperNestedProperty_SetsProperty()
        {
            var fieldName = "_privateField";

            var fixture = Fixture.New<NestedPropertyClass>().BypassConstructor().WithBackingField(fieldName, t => t.NestedClass.DeeperNestedClass.Text, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.NestedClass.DeeperNestedClass.Text, Is.EqualTo(_text));
        }

        class ListPropertyClass
        {
            public List<string> StringList { get; } = null!;
        }
        [Test]
        public void CollectionTypeFieldWithProp_CollectionParameters_SetsField()
        {
            var fixture = Fixture.New<ListPropertyClass>().BypassConstructor().WithBackingField(t => t.StringList, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.StringList[0], Is.EqualTo(_text));
        }

        class CollectionDifferentTypeClass
        {
            private readonly List<string> _stringList = null!;
            public IReadOnlyList<string> StringList => _stringList;
        }
        [Test]
        public void BackingFieldDifferentCollectionType_SetsField()
        {
            var fixture = Fixture.New<CollectionDifferentTypeClass>().WithBackingField(t => t.StringList, [_text]).Build();

            Assert.That(fixture.StringList.Single(), Is.EqualTo(_text));
        }

        [Test]
        public void Property_ConstructionNotChosen_InstantiatesNonNullables()
        {
            var fixture = Fixture.New<ClassWithNullable>().WithBackingField(c => c.Text, "test");
            var field = Helpers.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.NullableClass, Is.Null);
                Assert.That(field.NonNullableClass, Is.Not.Null);
            }
        }

        class ComputedPropertyClass
        {
            public int Computed => SomeMethod();
            private static int SomeMethod() => 2;
        }
        [Test]
        public void ComputedProperty_ThrowsException()
        {
            var fixture = Fixture.New<ComputedPropertyClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingField(c => c.Computed, 5));
        }

        class NullableValueListClass
        {
            private readonly List<int?> _list = null!;
            public IEnumerable<int> List => _list.Cast<int>();
        }
        [Test]
        public void NullableValueListField_NonNullableValueListValue_SetsField()
        {
            var fieldName = "_list";
            var list = new List<int>() { _number };

            var fixture = Fixture.New<NullableValueListClass>().BypassConstructor().WithBackingField(fieldName, c => c.List, list);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.List.Single(), Is.EqualTo(_number));
        }

        class NonNullableValueListClass
        {
            private readonly List<int> _list = null!;
            public IEnumerable<int?> List => _list.Cast<int?>();
        }
        [Test]
        [Ignore("It is undecided whether this should be allowed. Potentially with allowConversion bool.")]
        public void NonNullableValueListField_NullableValueListValue_SetsField()
        {
            var fieldName = "_list";
            var list = new List<int?>() { _number };

            var fixture = Fixture.New<NonNullableValueListClass>().BypassConstructor().WithBackingField(fieldName, c => c.List, list);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.List.First(), Is.EqualTo(_number));
        }

        [Test]
        [Ignore("It is undecided whether this should be allowed. Potentially with allowConversion bool.")]
        public void NonNullableValueListField_NullableValueListValue_WithNull_SetsField()
        {
            var fieldName = "_list";
            var list = new List<int?>() { _number, null };

            var fixture = Fixture.New<NonNullableValueListClass>().BypassConstructor().WithBackingField(fieldName, c => c.List, list);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.List.First(), Is.EqualTo(_number));
        }

        class NullableReferenceListClass
        {
            private readonly List<string?> _list = null!;
            public IEnumerable<string> List => _list.Cast<string>();
        }
        [Test]
        public void NullableReferenceListField_NonNullableReferenceListValue_SetsField()
        {
            var fieldName = "_list";
            var list = new List<string>() { _text };

            var fixture = Fixture.New<NullableReferenceListClass>().BypassConstructor().WithBackingField(fieldName, c => c.List, list);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.List.Single(), Is.EqualTo(_text));
        }

        class NonNullableReferenceListClass
        {
            private readonly List<string> _list = null!;
            public IEnumerable<string?> List => _list.Cast<string?>();
        }
        [Test]
        public void NonNullableReferenceListField_NullableReferenceListValue_SetsField()
        {
            var fieldName = "_list";
            var list = new List<string?>() { _text };

            var fixture = Fixture.New<NonNullableReferenceListClass>().BypassConstructor().WithBackingField(fieldName, c => c.List, list);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.List.First(), Is.EqualTo(_text));
        }

        [Test]
        public void NonNullableReferenceListField_NullableReferenceListValue_WithNull_SetsField()
        {
            var fieldName = "_list";
            var list = new List<string?>() { _text, null };

            var fixture = Fixture.New<NonNullableReferenceListClass>().BypassConstructor().WithBackingField(fieldName, c => c.List, list);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.List.First(), Is.EqualTo(_text));
        }

        [Test]
        public void NoMemberAccess_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<TestClass>().WithBackingField(c => c, new TestClass()));
        }
    }
}
