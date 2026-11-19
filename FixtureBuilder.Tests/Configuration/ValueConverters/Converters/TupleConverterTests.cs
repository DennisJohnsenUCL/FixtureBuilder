using System.Runtime.CompilerServices;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Configuration.ValueConverters.Converters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.Converters
{
    internal sealed class TupleConverterTests
    {
        #region Unhandled types

        [Test]
        public void Convert_TargetNotTuple_ReturnsNoResult()
        {
            var converter = new TupleConverter();
            var request = TestHelper.MakeConverterRequest(typeof(long));
            var value = (1, 2);

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotTuple_ReturnsNoResult()
        {
            var converter = new TupleConverter();
            var request = TestHelper.MakeConverterRequest(typeof((long, long)));
            List<int> value = [1, 2];

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        private class CustomTupleClass<T>(T value, string label) : ITuple
        {
            public int Length => 1;
            public object? this[int index] => index == 0 ? value : label;
        }

        [Test]
        public void Convert_TargetCustomTuple_ReturnsNoResult()
        {
            var converter = new TupleConverter();
            var request = TestHelper.MakeConverterRequest(typeof(CustomTupleClass<int>));
            var value = ValueTuple.Create(5);

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_DifferentArity_ReturnsNoResult()
        {
            var converter = new TupleConverter();
            var request = TestHelper.MakeConverterRequest(typeof((object, object, object)));
            var value = (1, "value");

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        #endregion

        #region Elements

        [Test]
        public void Convert_ElementsAssignable_ReturnsTupleOfTargetType()
        {
            var converter = new TupleConverter();
            var request = TestHelper.MakeConverterRequest(typeof((object, object)));
            var value = (1, "value");

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<(object, object)>());
                Assert.That(result, Is.EqualTo(((object)1, (object)"value")));
            }
        }

        [Test]
        public void Convert_ElementNotAssignable_ReturnsTupleWithElementConvertedByContextConverter()
        {
            var converter = new TupleConverter();
            var request = TestHelper.MakeConverterRequest(typeof((long, object)));
            var value = (1, "value");
            var compositeConverter = new Mock<ICompositeConverter>();
            compositeConverter.Setup(x => x.Convert(new ConverterRequest(typeof(long), typeof(object), typeof(long)), 1, It.IsAny<IFixtureContext>())).Returns(42L);
            var context = new Mock<IFixtureContext>();
            context.Setup(x => x.UnwrapAndLink(typeof(long), typeof(object))).Returns(typeof(long));
            context.Setup(x => x.Converter).Returns(compositeConverter.Object);

            var result = converter.Convert(request, value, context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<(long, object)>());
                Assert.That(result, Is.EqualTo((42L, (object)"value")));
            }
        }

        private class RootClass;

        [Test]
        public void Convert_ElementTypeLinkedForRoot_ReturnsTupleWithElementConvertedToLinkedType()
        {
            var converter = new TupleConverter();
            var request = new ConverterRequest(typeof((IMarkerInterface, int)), typeof(RootClass), typeof((IMarkerInterface, int)));
            var source = new ImplicitSourceStruct { Number = 42 };
            var value = (source, 1);
            var converted = new MarkerImplementationClass();
            var compositeConverter = new Mock<ICompositeConverter>();
            compositeConverter.Setup(x => x.Convert(new ConverterRequest(typeof(MarkerImplementationClass), typeof(RootClass), typeof(IMarkerInterface)), source, It.IsAny<IFixtureContext>())).Returns(converted);
            var context = new Mock<IFixtureContext>();
            context.Setup(x => x.UnwrapAndLink(typeof(IMarkerInterface), typeof(RootClass))).Returns(typeof(MarkerImplementationClass));
            context.Setup(x => x.Converter).Returns(compositeConverter.Object);

            var result = converter.Convert(request, value, context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<(IMarkerInterface, int)>());
                Assert.That(result, Is.EqualTo(((IMarkerInterface)converted, 1)));
            }
        }

        [Test]
        public void Convert_ElementNotConvertible_ThrowsInvalidCastException()
        {
            var converter = new TupleConverter();
            var request = TestHelper.MakeConverterRequest(typeof((long, long)));
            var value = (1, 2);
            var compositeConverter = new Mock<ICompositeConverter>();
            compositeConverter.Setup(x => x.Convert(new ConverterRequest(typeof(long), typeof(object), typeof(long)), It.IsAny<object>(), It.IsAny<IFixtureContext>())).Returns(NoResult.Instance);
            var context = new Mock<IFixtureContext>();
            context.Setup(x => x.UnwrapAndLink(typeof(long), typeof(object))).Returns(typeof(long));
            context.Setup(x => x.Converter).Returns(compositeConverter.Object);

            Assert.Throws<InvalidCastException>(() => converter.Convert(request, value, context.Object));
        }

        #endregion

        #region Element nullability

        [Test]
        public void Convert_NullElementToReferenceType_ReturnsTupleWithNullElement()
        {
            var converter = new TupleConverter();
            var request = TestHelper.MakeConverterRequest(typeof((object, object)));
            var value = ((string?)null, "value");

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<(object, object)>());
                Assert.That(result, Is.EqualTo(((object?)null, (object)"value")));
            }
        }

        [Test]
        public void Convert_NullElementToNonNullableValueType_ThrowsInvalidCastException()
        {
            var converter = new TupleConverter();
            var request = TestHelper.MakeConverterRequest(typeof((int, int)));
            var value = ((int?)null, 1);

            Assert.Throws<InvalidCastException>(() => converter.Convert(request, value, Mock.Of<IFixtureContext>()));
        }

        #endregion

        #region Tuple shapes

        [Test]
        public void Convert_ReferenceTupleTarget_ReturnsReferenceTupleOfTargetType()
        {
            var converter = new TupleConverter();
            var request = TestHelper.MakeConverterRequest(typeof(Tuple<object, object>));
            var value = Tuple.Create(1, "value");

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<Tuple<object, object>>());
                Assert.That(result, Is.EqualTo(Tuple.Create<object, object>(1, "value")));
            }
        }

        [Test]
        public void Convert_ReferenceTupleValue_ReturnsTupleOfTargetType()
        {
            var converter = new TupleConverter();
            var request = TestHelper.MakeConverterRequest(typeof((object, object)));
            var value = Tuple.Create(1, "value");

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<(object, object)>());
                Assert.That(result, Is.EqualTo(((object)1, (object)"value")));
            }
        }

        [Test]
        public void Convert_UnwrappedNullableTarget_ReturnsTupleOfUnwrappedType()
        {
            var converter = new TupleConverter();
            var request = new ConverterRequest(typeof((object, object)), typeof(object), typeof((object, object)?));
            var value = (1, "value");

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<(object, object)>());
                Assert.That(result, Is.EqualTo(((object)1, (object)"value")));
            }
        }

        [Test]
        public void Convert_MoreThanSevenElements_ReturnsTupleWithRestElementConverted()
        {
            var converter = new TupleConverter();
            var request = TestHelper.MakeConverterRequest(typeof((int, int, int, int, int, int, int, long)));
            var value = (1, 2, 3, 4, 5, 6, 7, 8);
            var compositeConverter = new Mock<ICompositeConverter>();
            compositeConverter.Setup(x => x.Convert(new ConverterRequest(typeof(long), typeof(object), typeof(long)), 8, It.IsAny<IFixtureContext>())).Returns(42L);
            var context = new Mock<IFixtureContext>();
            context.Setup(x => x.UnwrapAndLink(typeof(long), typeof(object))).Returns(typeof(long));
            context.Setup(x => x.Converter).Returns(compositeConverter.Object);

            var result = converter.Convert(request, value, context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<(int, int, int, int, int, int, int, long)>());
                Assert.That(result, Is.EqualTo((1, 2, 3, 4, 5, 6, 7, 42L)));
            }
        }

        #endregion
    }
}
