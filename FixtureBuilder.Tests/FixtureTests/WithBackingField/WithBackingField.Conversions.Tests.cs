using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureTests.WithBackingField
{
    internal sealed class WithBackingFieldConversionsTests
    {
        #region Null values

        [Test]
        public void NullableValueTypeField_NullValue_SetsNull()
        {
            var fixture = TestHelper.MakeFixture<PropertyHolder<int?>>();
            fixture.With(x => x.Value, 5);

            var result = fixture.WithBackingField(x => x.Value, null).Build();

            Assert.That(result.Value, Is.Null);
        }

        private class NonNullableFieldClass
        {
            private int _value;
            public int? Value { get => _value; set => _value = (int)value!; }
        }

        [Test]
        public void NonNullableValueTypeField_NullValue_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<NonNullableFieldClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingField(x => x.Value, null));
        }

        #endregion

        #region Implicit conversions

        private class ImplicitConversionClass
        {
            private readonly ImplicitTargetStruct _value;
            public ImplicitSourceStruct Value => new() { Number = _value.Number };
        }

        [Test]
        public void ImplicitConversionAllowed_SetsConvertedValue()
        {
            var options = new FixtureOptions { AllowImplicitConversion = true };
            var factory = new FixtureFactory(options);
            var fixture = TestHelper.MakeFixture<ImplicitConversionClass>(factory);

            var result = fixture.WithBackingField(x => x.Value, new ImplicitSourceStruct { Number = 42 }).Build();

            Assert.That(result.Value.Number, Is.EqualTo(42));
        }

        [Test]
        public void ImplicitConversionNotAllowed_ThrowsInvalidOperationException()
        {
            var fixture = TestHelper.MakeFixture<ImplicitConversionClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.WithBackingField(x => x.Value, new ImplicitSourceStruct { Number = 42 }));
        }

        private class ImplicitConversionListClass
        {
            private readonly List<ImplicitTargetStruct> _values = null!;
            public List<ImplicitSourceStruct> Values => [.. _values.Select(target => new ImplicitSourceStruct { Number = target.Number })];
        }

        [Test]
        public void ImplicitConversionAllowed_ListElements_SetsConvertedElements()
        {
            var options = new FixtureOptions { AllowImplicitConversion = true };
            var factory = new FixtureFactory(options);
            var fixture = TestHelper.MakeFixture<ImplicitConversionListClass>(factory);
            List<ImplicitSourceStruct> values = [new() { Number = 1 }, new() { Number = 2 }, new() { Number = 3 }];

            var result = fixture.WithBackingField(x => x.Values, values).Build();

            var numbers = result.Values.Select(value => value.Number);
            Assert.That(numbers, Is.EqualTo([1, 2, 3]));
        }

        private class ImplicitConversionArrayClass
        {
            private readonly ImplicitTargetStruct[] _values = null!;
            public ImplicitSourceStruct[] Values => [.. _values.Select(target => new ImplicitSourceStruct { Number = target.Number })];
        }

        [Test]
        public void ImplicitConversionAllowed_ArrayElements_SetsConvertedElements()
        {
            var options = new FixtureOptions { AllowImplicitConversion = true };
            var factory = new FixtureFactory(options);
            var fixture = TestHelper.MakeFixture<ImplicitConversionArrayClass>(factory);
            ImplicitSourceStruct[] values = [new() { Number = 1 }, new() { Number = 2 }, new() { Number = 3 }];

            var result = fixture.WithBackingField(x => x.Values, values).Build();

            var numbers = result.Values.Select(value => value.Number);
            Assert.That(numbers, Is.EqualTo([1, 2, 3]));
        }

        private class ImplicitConversionDictionaryClass
        {
            private readonly Dictionary<string, ImplicitTargetStruct> _values = null!;
            public Dictionary<string, ImplicitSourceStruct> Values => _values.ToDictionary(pair => pair.Key, pair => new ImplicitSourceStruct { Number = pair.Value.Number });
        }

        [Test]
        public void ImplicitConversionAllowed_DictionaryValues_SetsConvertedValues()
        {
            var options = new FixtureOptions { AllowImplicitConversion = true };
            var factory = new FixtureFactory(options);
            var fixture = TestHelper.MakeFixture<ImplicitConversionDictionaryClass>(factory);
            var values = new Dictionary<string, ImplicitSourceStruct>
            {
                ["one"] = new() { Number = 1 },
                ["two"] = new() { Number = 2 }
            };

            var result = fixture.WithBackingField(x => x.Values, values).Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Values["one"].Number, Is.EqualTo(1));
                Assert.That(result.Values["two"].Number, Is.EqualTo(2));
            }
        }

        #endregion

        #region Field type differs from property type

        [Test]
        public void WiderBackingField_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<WiderBackingFieldClass>();

            var result = fixture.WithBackingField(x => x.Value, 123).Build();

            Assert.That(result.Value, Is.EqualTo(123));
        }

        [Test]
        public void WiderBackingField_ConverterForFieldType_SetsConvertedValue()
        {
            var factory = new FixtureFactory().AddConverter(new FixedLongConverter(456));
            var fixture = TestHelper.MakeFixture<WiderBackingFieldClass>(factory);

            var result = fixture.WithBackingField(x => x.Value, 123).Build();

            Assert.That(result.Value, Is.EqualTo(456));
        }

        private class StringResultConverter : ICustomConverter
        {
            public object? Convert(Type target, object value) => "converted";
        }

        [Test]
        public void WiderBackingField_ConverterReturnsWrongType_ThrowsInvalidCastException()
        {
            var factory = new FixtureFactory().AddConverter(new StringResultConverter());
            var fixture = TestHelper.MakeFixture<WiderBackingFieldClass>(factory);

            Assert.Throws<InvalidCastException>(() => fixture.WithBackingField(x => x.Value, 123));
        }

        private class NarrowerBackingFieldClass
        {
            private readonly int _value;
            public long Value => _value;
        }

        [Test]
        public void NarrowerBackingField_ThrowsInvalidOperationExceptionWithArgumentExceptionInner()
        {
            var fixture = TestHelper.MakeFixture<NarrowerBackingFieldClass>();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.WithBackingField(x => x.Value, 123L));

            Assert.That(ex.InnerException, Is.InstanceOf<ArgumentException>());
        }

        #endregion
    }
}
