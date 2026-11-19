using FixtureBuilder.Configuration.ValueConverters.Converters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.Converters.ImplicitConverterTests
{
    internal sealed class ImplicitConverterConversionsTests
    {
        private class BaseClass;

        private class DerivedClass : BaseClass;

        #region Implicit operators

        [Test]
        public void Convert_OperatorOnSourceType_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(int));
            var value = new ImplicitToIntClass { Number = 42 };
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.EqualTo(42));
        }

        private class DerivedImplicitToIntClass : ImplicitToIntClass;

        [Test]
        public void Convert_OperatorOnBaseOfSourceType_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(int));
            var value = new DerivedImplicitToIntClass { Number = 42 };
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void Convert_OperatorOnTargetType_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(ImplicitTargetStruct));
            var value = new ImplicitSourceStruct { Number = 42 };
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            var converted = (ImplicitTargetStruct)result!;
            Assert.That(converted.Number, Is.EqualTo(42));
        }

        private class ImplicitFromValueClass<T>
        {
            public T Value = default!;

            public static implicit operator ImplicitFromValueClass<T>(T value) => new() { Value = value };
        }

        [Test]
        public void Convert_OperatorOnGenericTargetType_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(ImplicitFromValueClass<int>));
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, 42, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            var converted = (ImplicitFromValueClass<int>)result!;
            Assert.That(converted.Value, Is.EqualTo(42));
        }

        #endregion

        #region Standard conversions

        [Test]
        public void Convert_WiderNumericType_ReturnsWidenedValue()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(long));
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, 42, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.TypeOf<long>().And.EqualTo(42L));
        }

        private class ImplicitFromBaseClass
        {
            public static implicit operator ImplicitFromBaseClass(BaseClass value) => new();
        }

        [Test]
        public void Convert_OperatorAcceptsBaseOfValueType_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(ImplicitFromBaseClass));
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, new DerivedClass(), Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.TypeOf<ImplicitFromBaseClass>());
        }

        private class ImplicitToDerivedClass
        {
            public static implicit operator DerivedClass(ImplicitToDerivedClass source) => new();
        }

        [Test]
        public void Convert_OperatorToDerivedOfTargetType_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(BaseClass));
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, new ImplicitToDerivedClass(), Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.TypeOf<DerivedClass>());
        }

        private struct ImplicitFromLongStruct
        {
            public long Number;

            public static implicit operator ImplicitFromLongStruct(long number) => new() { Number = number };
        }

        [Test]
        public void Convert_OperatorAcceptsWiderNumericType_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(ImplicitFromLongStruct));
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, 42, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            var converted = (ImplicitFromLongStruct)result!;
            Assert.That(converted.Number, Is.EqualTo(42));
        }

        [Test]
        public void Convert_OperatorToNarrowerNumericType_ReturnsWidenedValue()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(long));
            var value = new ImplicitToIntClass { Number = 42 };
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.TypeOf<long>().And.EqualTo(42L));
        }

        [Test]
        public void Convert_OperatorToUnderlyingOfNullableType_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(int?));
            var value = new ImplicitToIntClass { Number = 42 };
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.EqualTo(42));
        }

        #endregion

        #region Arrays

        private class ImplicitFromArrayClass
        {
            public int[] Values = [];

            public static implicit operator ImplicitFromArrayClass(int[] values) => new() { Values = values };
        }

        [Test]
        public void Convert_ArrayValue_OperatorFromArrayType_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(ImplicitFromArrayClass));
            int[] value = [1, 2];
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            var converted = (ImplicitFromArrayClass)result!;
            Assert.That(converted.Values, Is.EqualTo([1, 2]));
        }

        private class ImplicitFromBaseArrayClass
        {
            public BaseClass[] Values = [];

            public static implicit operator ImplicitFromBaseArrayClass(BaseClass[] values) => new() { Values = values };
        }

        [Test]
        public void Convert_ArrayValue_OperatorFromBaseElementArray_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(ImplicitFromBaseArrayClass));
            DerivedClass[] value = [new()];
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            var converted = (ImplicitFromBaseArrayClass)result!;
            Assert.That(converted.Values, Is.SameAs(value));
        }

        private class ImplicitFromJaggedArrayClass
        {
            public int[][] Values = [];

            public static implicit operator ImplicitFromJaggedArrayClass(int[][] values) => new() { Values = values };
        }

        [Test]
        public void Convert_JaggedArrayValue_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(ImplicitFromJaggedArrayClass));
            int[][] value = [[1, 2], [3]];
            int[][] expected = [[1, 2], [3]];
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            var converted = (ImplicitFromJaggedArrayClass)result!;
            Assert.That(converted.Values, Is.EqualTo(expected));
        }

        #endregion

        #region No conversion

        [Test]
        public void Convert_NoOperator_ReturnsNoResult()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(string));
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, 42, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        private class ExplicitToIntClass
        {
            public int Number;

            public static explicit operator int(ExplicitToIntClass source) => source.Number;
        }

        [Test]
        public void Convert_OnlyExplicitOperator_ReturnsNoResult()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(int));
            var value = new ExplicitToIntClass { Number = 42 };
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        private class AmbiguousTargetClass
        {
            public static implicit operator AmbiguousTargetClass(AmbiguousSourceClass source) => new();
        }

        private class AmbiguousSourceClass
        {
            public static implicit operator AmbiguousTargetClass(AmbiguousSourceClass source) => new();
        }

        [Test]
        public void Convert_AmbiguousOperators_ReturnsNoResult()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(AmbiguousTargetClass));
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, new AmbiguousSourceClass(), Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        #endregion
    }
}
