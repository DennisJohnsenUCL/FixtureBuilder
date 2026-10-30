#pragma warning disable CS0649
#pragma warning disable IDE0044 // Add readonly modifier

using FixtureBuilder.Core;
using Moq;

namespace FixtureBuilder.Tests.FixtureTests.WithBackingField
{
    internal sealed class WithBackingFieldTests
    {
        private readonly static string _text = "Test string";
        private readonly static int _number = 123;

        class NormalClass
        {
            public string Text { get; set; } = null!;
            public int Number { get; set; }
        }

        [Test]
        public void Property_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<NormalClass>();

            fixture.WithBackingField(t => t.Text, _text);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.Text, Is.EqualTo(_text));
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
            private readonly string _privateExplicitField = null!;
            public string PrivateExplicitField => _privateExplicitField;
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
        public void CastToInterface_ImplicitImplementation_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<InterfaceTestClass>().CastTo<ITestInterface>();

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
        public void DeeperDerivedNestedProperty_Overridden_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<NestedPropertyClass>();

            fixture.WithBackingField(t => t.NestedClass.DerivedNestedClass.Value, _number);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.NestedClass.DerivedNestedClass.Value, Is.EqualTo(_number));
        }

        [Test]
        public void DeeperDerivedNestedProperty_NotOverridden_SetsProperty()
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

        [Test]
        public void InstantiatesFixture()
        {
            var fixture = TestHelper.MakeFixture<NormalClass>();

            using (Assert.EnterMultipleScope())
            {
                Assert.DoesNotThrow(() => fixture.WithBackingField(x => x.Text, "Hello"));
                Assert.That(TestHelper.GetFixture(fixture), Is.Not.Null);
            }
        }

        class FieldIntermediateClass
        {
            public NormalClass NormalClass = null!;
        }

        [Test]
        public void FieldIntermediate_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<FieldIntermediateClass>();

            fixture.WithBackingField(t => t.NormalClass.Text, _text);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.NormalClass.Text, Is.EqualTo(_text));
        }

        class ImplicitConversionClass
        {
            private readonly TargetType _value;
            public SourceType Value => new() { Inner = _value.Inner };
        }

        struct SourceType
        {
            public int Inner;
        }

        struct TargetType
        {
            public int Inner;
            public static implicit operator TargetType(SourceType s) => new() { Inner = s.Inner };
        }

        [Test]
        public void WithBackingField_ImplicitConversion_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<ImplicitConversionClass>();
            TestHelper.SetOption(fixture, o => o.AllowImplicitConversion = true);

            var source = new SourceType { Inner = 42 };
            fixture.WithBackingField(t => t.Value, source);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Value.Inner, Is.EqualTo(42));
        }

        class ImplicitConversionListClass
        {
            private readonly List<TargetType> _values = null!;
            public List<SourceType> Values => [.. _values.Select(t => new SourceType { Inner = t.Inner })];
        }
        [Test]
        public void WithBackingField_ImplicitConversion_ListElements_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<ImplicitConversionListClass>();
            TestHelper.SetOption(fixture, o => o.AllowImplicitConversion = true);

            var source = new List<SourceType>
            {
                new() { Inner = 1 },
                new() { Inner = 2 },
                new() { Inner = 3 }
            };

            fixture.WithBackingField(t => t.Values, source);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Values.Select(v => v.Inner), Is.EqualTo([1, 2, 3]));
        }

        class ImplicitConversionDictionaryClass
        {
            private readonly Dictionary<string, TargetType> _values = null!;
            public Dictionary<string, SourceType> Values => _values.ToDictionary(k => k.Key, v => new SourceType { Inner = v.Value.Inner });
        }

        [Test]
        public void WithBackingField_ImplicitConversion_DictionaryElements_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<ImplicitConversionDictionaryClass>();
            TestHelper.SetOption(fixture, o => o.AllowImplicitConversion = true);

            var source = new Dictionary<string, SourceType>
            {
                { "one", new() { Inner = 1 } },
                { "two", new() { Inner = 2 } },
                { "three", new() { Inner = 3 } }
            };

            fixture.WithBackingField(t => t.Values, source);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Values.Select(v => v.Value.Inner), Is.EqualTo([1, 2, 3]));
        }

        class WideningFieldClass
        {
            private readonly long _number;
            public int Number => (int)_number;
        }
        [Test]
        public void WideningFieldType_NoConverter_SetsField()
        {
            var fixture = TestHelper.MakeFixture<WideningFieldClass>();

            fixture.WithBackingField(t => t.Number, _number);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Number, Is.EqualTo(_number));
        }

        [Test]
        public void WideningFieldType_ConverterHandlesValue_SetsConvertedValue()
        {
            var converted = 456L;
            var converter = new Mock<ICustomConverter>();
            converter.Setup(c => c.Convert(typeof(long), _number)).Returns(converted);
            var fixture = new FixtureFactory()
                .AddConverter(converter.Object)
                .New(new WideningFieldClass());

            fixture.WithBackingField(t => t.Number, _number);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Number, Is.EqualTo(converted));
        }

        class NarrowingFieldClass
        {
            private readonly int _number;
            public long Number => _number;
        }
        [Test]
        public void NarrowingFieldType_ThrowsException()
        {
            var fixture = TestHelper.MakeFixture<NarrowingFieldClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingField(t => t.Number, 123L));
        }

        [Test]
        public void NarrowingFieldType_ExceptionHasSetValueExceptionAsInner()
        {
            var fixture = TestHelper.MakeFixture<NarrowingFieldClass>();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.WithBackingField(t => t.Number, 123L));

            Assert.That(ex?.InnerException, Is.InstanceOf<ArgumentException>());
        }

        struct Point
        {
            private int _z;
            public readonly int Z => _z;
        }
        class StructPropertyIntermediateClass
        {
            public Point Center { get; set; }
        }
        [Test]
        public void StructPropertyIntermediate_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<StructPropertyIntermediateClass>();

            fixture.WithBackingField(t => t.Center.Z, _number);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Center.Z, Is.EqualTo(_number));
        }
    }
}
