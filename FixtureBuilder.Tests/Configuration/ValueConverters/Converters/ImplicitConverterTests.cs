using FixtureBuilder.Configuration.ValueConverters.Converters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.Converters
{
    internal sealed class ImplicitConverterTests
    {
        private class ImplicitToIntClass
        {
            public int Number;

            public static implicit operator int(ImplicitToIntClass source) => source.Number;
        }

        [Test]
        public void Convert_OperatorOnSourceType_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = new TypeRequest(typeof(int), typeof(object));
            var value = new ImplicitToIntClass { Number = 42 };
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void Convert_ImplicitConversionNotAllowed_ReturnsNoResult()
        {
            var converter = new ImplicitConverter();
            var request = new TypeRequest(typeof(int), typeof(object));
            var value = new ImplicitToIntClass { Number = 42 };

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        private class DerivedImplicitToIntClass : ImplicitToIntClass;

        [Test]
        public void Convert_OperatorOnBaseOfSourceType_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = new TypeRequest(typeof(int), typeof(object));
            var value = new DerivedImplicitToIntClass { Number = 42 };
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.EqualTo(42));
        }

        private struct ImplicitFromIntStruct
        {
            public int Number;

            public static implicit operator ImplicitFromIntStruct(int number) => new() { Number = number };
        }

        [Test]
        public void Convert_OperatorOnTargetType_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = new TypeRequest(typeof(ImplicitFromIntStruct), typeof(object));
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, 42, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            var converted = (ImplicitFromIntStruct)result!;
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
            var request = new TypeRequest(typeof(ImplicitFromValueClass<int>), typeof(object));
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, 42, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            var converted = (ImplicitFromValueClass<int>)result!;
            Assert.That(converted.Value, Is.EqualTo(42));
        }

        private class BaseClass;

        private class DerivedClass : BaseClass;

        private class ImplicitFromBaseClass
        {
            public static implicit operator ImplicitFromBaseClass(BaseClass value) => new();
        }

        [Test]
        public void Convert_OperatorAcceptsBaseOfValueType_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = new TypeRequest(typeof(ImplicitFromBaseClass), typeof(object));
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, new DerivedClass(), Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.TypeOf<ImplicitFromBaseClass>());
        }

        [Test]
        public void Convert_NoOperator_ReturnsNoResult()
        {
            var converter = new ImplicitConverter();
            var request = new TypeRequest(typeof(string), typeof(object));
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
            var request = new TypeRequest(typeof(int), typeof(object));
            var value = new ExplicitToIntClass { Number = 42 };
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
