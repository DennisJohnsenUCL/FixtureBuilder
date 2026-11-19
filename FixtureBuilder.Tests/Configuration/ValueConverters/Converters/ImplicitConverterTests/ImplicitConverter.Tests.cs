using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Configuration.ValueConverters.Converters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.Converters.ImplicitConverterTests
{
    internal sealed class ImplicitConverterTests
    {
        #region Options

        [Test]
        public void Convert_ImplicitConversionNotAllowed_ReturnsNoResult()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(int));
            var value = new ImplicitToIntClass { Number = 42 };

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == new FixtureOptions()));

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        #endregion

        #region Target type

        private class DeclaredClass;

        private class ImplicitToNullableIntClass
        {
            public int Number;

            public static implicit operator int?(ImplicitToNullableIntClass source) => source.Number;
        }

        [Test]
        public void Convert_UnwrappedNullableType_OperatorToNullable_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = new ConverterRequest(typeof(int), typeof(object), typeof(int?));
            var value = new ImplicitToNullableIntClass { Number = 42 };
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.EqualTo(42));
        }

        private class ImplicitLinkedClass : DeclaredClass
        {
            public static implicit operator ImplicitLinkedClass(ImplicitSourceStruct source) => new();
        }

        [Test]
        public void Convert_LinkedType_OperatorOnLinkedType_ReturnsLinkedValue()
        {
            var converter = new ImplicitConverter();
            var request = new ConverterRequest(typeof(ImplicitLinkedClass), typeof(object), typeof(DeclaredClass));
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, new ImplicitSourceStruct(), Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.TypeOf<ImplicitLinkedClass>());
        }

        private class LinkedClass : DeclaredClass;

        private class ImplicitToDeclaredClass
        {
            public static implicit operator DeclaredClass(ImplicitToDeclaredClass source) => new();
        }

        [Test]
        public void Convert_LinkedType_OperatorOnlyToOriginalType_ReturnsNoResult()
        {
            var converter = new ImplicitConverter();
            var request = new ConverterRequest(typeof(LinkedClass), typeof(object), typeof(DeclaredClass));
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, new ImplicitToDeclaredClass(), Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        private class ImplicitMarkerClass : IMarkerInterface
        {
            public static implicit operator ImplicitMarkerClass(ImplicitSourceStruct source) => new();
        }

        [Test]
        public void Convert_InterfaceLinkedToClass_ReturnsLinkedValue()
        {
            var converter = new ImplicitConverter();
            var request = new ConverterRequest(typeof(ImplicitMarkerClass), typeof(object), typeof(IMarkerInterface));
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, new ImplicitSourceStruct(), Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.TypeOf<ImplicitMarkerClass>());
        }

        private class ImplicitToMarkerClass
        {
            public static implicit operator MarkerImplementationClass(ImplicitToMarkerClass source) => new();
        }

        [Test]
        public void Convert_UnlinkedInterface_ReturnsNoResult()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(IMarkerInterface));
            var options = new FixtureOptions { AllowImplicitConversion = true };

            var result = converter.Convert(request, new ImplicitToMarkerClass(), Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options));

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        #endregion

        #region Repeated conversions

        [Test]
        public void Convert_SameTypesAgain_ConvertsNewValue()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(int));
            var options = new FixtureOptions { AllowImplicitConversion = true };
            var context = Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options);
            converter.Convert(request, new ImplicitToIntClass { Number = 1 }, context);

            var result = converter.Convert(request, new ImplicitToIntClass { Number = 2 }, context);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void Convert_SameTypesAfterNoConversion_ReturnsNoResult()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(string));
            var options = new FixtureOptions { AllowImplicitConversion = true };
            var context = Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options);
            converter.Convert(request, 42, context);

            var result = converter.Convert(request, 42, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        #endregion

        #region Operator exceptions

        private class PositiveOnlyImplicitClass
        {
            public int Number;

            public static implicit operator int(PositiveOnlyImplicitClass source)
                => source.Number > 0 ? source.Number : throw new InvalidOperationException("Number must be positive.");
        }

        [Test]
        public void Convert_OperatorThrows_ThrowsInvalidOperationException()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(int));
            var value = new PositiveOnlyImplicitClass { Number = 0 };
            var options = new FixtureOptions { AllowImplicitConversion = true };
            var context = Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options);

            Assert.Throws<InvalidOperationException>(() => converter.Convert(request, value, context));
        }

        [Test]
        public void Convert_OperatorThrewForEarlierValue_ReturnsConvertedValue()
        {
            var converter = new ImplicitConverter();
            var request = TestHelper.MakeConverterRequest(typeof(int));
            var options = new FixtureOptions { AllowImplicitConversion = true };
            var context = Mock.Of<IFixtureContext>(x => x.OptionsFor(request.RootType) == options);
            Assert.Throws<InvalidOperationException>(() => converter.Convert(request, new PositiveOnlyImplicitClass { Number = 0 }, context));

            var result = converter.Convert(request, new PositiveOnlyImplicitClass { Number = 42 }, context);

            Assert.That(result, Is.EqualTo(42));
        }

        #endregion
    }
}
