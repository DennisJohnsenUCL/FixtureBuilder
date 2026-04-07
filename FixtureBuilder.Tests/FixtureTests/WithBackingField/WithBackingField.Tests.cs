#pragma warning disable CA1822 // Mark members as static

namespace FixtureBuilder.Tests.FixtureTests.WithBackingField
{
    internal sealed class WithBackingFieldTests
    {
        private readonly static string _text = "Test string";
        private readonly static int _number = 123;

        [Test]
        public void RecordProperty_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<TestRecord>();

            fixture.WithBackingField(t => t.Text, _text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.Text, Is.EqualTo(_text));
        }

        [Test]
        public void RecordProperties_SetsProperties()
        {
            var fixture = TestHelper.MakeFixture<TestRecord>();

            fixture.WithBackingField(t => t.Text, _text);
            fixture.WithBackingField(t => t.Number, _number);

            var field = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.Text, Is.EqualTo(_text));
                Assert.That(field.Number, Is.EqualTo(_number));
            }
        }

        [Test]
        public void NotARecordProperty_ThrowsException()
        {
            var fixture = TestHelper.MakeFixture<TestRecord>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingField(t => t.GetHashCode(), _number));
        }

        [Test]
        public void NoRecordPropertyBackingField_ThrowsException()
        {
            var fixture = TestHelper.MakeFixture<TestRecord>();

            fixture.With(t => t.Text, "test");

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingField(t => t.Text.Length, _number));
        }

        class NormalClass
        {
            public string Text { get; set; } = null!;
            public int Number { get; set; }
        }
        [Test]
        public void ClassProperty_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<NormalClass>();

            fixture.WithBackingField(t => t.Text, _text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.Text, Is.EqualTo(_text));
        }

        [Test]
        public void ClassProperties_SetsProperties()
        {
            var fixture = TestHelper.MakeFixture<NormalClass>();

            fixture.WithBackingField(t => t.Text, _text);
            fixture.WithBackingField(t => t.Number, _number);

            var field = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.Text, Is.EqualTo(_text));
                Assert.That(field.Number, Is.EqualTo(_number));
            }
        }

        [Test]
        public void NotAClassProperty_ThrowsException()
        {
            var fixture = TestHelper.MakeFixture<TestClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingField(t => t.GetHashCode(), _number));
        }

        [Test]
        public void NoClassPropertyBackingField_ThrowsException()
        {
            var fixture = TestHelper.MakeFixture<TestClass>();

            fixture.With(t => t.Text, "test");

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingField(t => t.Text.Length, _number));
        }

        [Test]
        public void ExplicitBackingField_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<ExplicitBackingFieldClass>();

            fixture.WithBackingField(t => t.Text, _text);

            var field = TestHelper.GetFixture(fixture);
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
            var fixture = TestHelper.MakeFixture<ExplicitBackingFieldNoUnderscoreClass>();

            fixture.WithBackingField(t => t.PrivateExplicitField, _text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.PrivateExplicitField, Is.EqualTo(_text));
        }

        [Test]
        public void DerivedProperty_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<DerivedTestClass>();

            fixture.WithBackingField(t => t.Text, _text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.Text, Is.EqualTo(_text));
        }

        [Test]
        public void OverriddenProperty_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<DerivedTestClass>();

            fixture.WithBackingField(t => t.Number, _number);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.Number, Is.EqualTo(_number));
        }

        [Test]
        public void ImplicitInterfaceImplementation_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<InterfaceTestClass>();

            fixture.WithBackingField(t => t.ImplicitProperty, _text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.ImplicitProperty, Is.EqualTo(_text));
        }

        [Test]
        public void TwiceDerivedClass_PropertyInDerivedClass_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<TwiceDerivedClass>();

            fixture.WithBackingField(p => p.Number, _number);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.Number, Is.EqualTo(_number));
        }

        [Test]
        public void GenericClass_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<GenericClass<string>>();

            fixture.WithBackingField(g => g.Value, _text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.Value, Is.EqualTo(_text));
        }

        class NestedPropertyClass
        {
            public NestedClass NestedClass { get; set; } = null!;
        }
        [Test]
        public void NestedProperty_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<NestedPropertyClass>();

            fixture.WithBackingField(t => t.NestedClass.Value, _text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NestedClass.Value, Is.EqualTo(_text));
        }

        [Test]
        public void DerivedNestedProperty_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<DerivedTestClass>();

            fixture.WithBackingField(t => t.NestedClass.Value, _text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NestedClass.Value, Is.EqualTo(_text));
        }

        [Test]
        public void DeeperNestedProperty_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<NestedPropertyClass>();

            fixture.WithBackingField(t => t.NestedClass.DeeperNestedClass.Value, _number);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NestedClass.DeeperNestedClass.Value, Is.EqualTo(_number));
        }

        [Test]
        public void DeeperDerivedNestedProperty_Overriden_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<NestedPropertyClass>();

            fixture.WithBackingField(t => t.NestedClass.DerivedNestedClass.Value, _number);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NestedClass.DerivedNestedClass.Value, Is.EqualTo(_number));
        }

        [Test]
        public void DeeperDerivedNestedProperty_NotOverriden_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<NestedPropertyClass>();

            fixture.WithBackingField(t => t.NestedClass.DerivedNestedClass.String, _text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NestedClass.DerivedNestedClass.String, Is.EqualTo(_text));
        }

        [Test]
        public void FieldNameGiven_DeeperNestedProperty_SetsProperty()
        {
            var fieldName = "_privateField";
            var fixture = TestHelper.MakeFixture<NestedPropertyClass>();

            fixture.WithBackingField(t => t.NestedClass.DeeperNestedClass.Text, _text, fieldName);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NestedClass.DeeperNestedClass.Text, Is.EqualTo(_text));
        }

        class ListPropertyClass
        {
            public List<string> StringList { get; } = null!;
        }
        [Test]
        public void CollectionTypeFieldWithProp_CollectionParameters_SetsField()
        {
            var fixture = TestHelper.MakeFixture<ListPropertyClass>();

            fixture.WithBackingField(t => t.StringList, [_text]);

            var field = TestHelper.GetFixture(fixture);
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
            var fixture = TestHelper.MakeFixture<CollectionDifferentTypeClass>();

            fixture.WithBackingField(t => t.StringList, [_text]);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.StringList.Single(), Is.EqualTo(_text));
        }

        class ComputedPropertyClass
        {
            public int Computed => SomeMethod();
            private static int SomeMethod() => 2;
        }
        [Test]
        public void ComputedProperty_ThrowsException()
        {
            var fixture = TestHelper.MakeFixture<ComputedPropertyClass>();

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
            var fixture = TestHelper.MakeFixture<NullableValueListClass>();

            fixture.WithBackingField(c => c.List, list, fieldName);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.List.Single(), Is.EqualTo(_number));
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
            var fixture = TestHelper.MakeFixture<NullableReferenceListClass>();

            fixture.WithBackingField(c => c.List, list, fieldName);

            var field = TestHelper.GetFixture(fixture);
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
            var fixture = TestHelper.MakeFixture<NonNullableReferenceListClass>();

            fixture.WithBackingField(c => c.List, list, fieldName);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.List.First(), Is.EqualTo(_text));
        }

        [Test]
        public void NonNullableReferenceListField_NullableReferenceListValue_WithNull_SetsField()
        {
            var fieldName = "_list";
            var list = new List<string?>() { _text, null };
            var fixture = TestHelper.MakeFixture<NonNullableReferenceListClass>();

            fixture.WithBackingField(c => c.List, list, fieldName);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.List.First(), Is.EqualTo(_text));
        }

        class ValueMembersClass
        {
            private int? _nullable;
            public int? Nullable { get => _nullable; set { _nullable = value; } }

            private int _nonNullable;
            public int? NonNullable { get => _nonNullable; set { _nonNullable = (int)value!; } }
        }
        [Test]
        public void NullableValueField_NonNullableValueProperty_SetsField()
        {
            var fixture = TestHelper.MakeFixture<ValueMembersClass>();

            fixture = fixture.WithBackingField(x => x.Nullable, _number);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Nullable, Is.EqualTo(_number));
        }

        [Test]
        public void NullableValueField_NullValue_SetsField()
        {
            var fixture = TestHelper.MakeFixture<ValueMembersClass>();

            fixture = fixture.WithBackingField(x => x.Nullable, null);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Nullable, Is.Null);
        }

        [Test]
        public void NonNullableValueField_NullValue_ThrowsException()
        {
            var fixture = TestHelper.MakeFixture<ValueMembersClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingField(x => x.NonNullable, null));
        }
    }
}
