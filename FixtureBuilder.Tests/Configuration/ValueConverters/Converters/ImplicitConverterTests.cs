#pragma warning disable IDE0060

using FixtureBuilder.Configuration.ValueConverters.Converters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.Converters
{
    internal sealed class ImplicitConverterTests
    {
        private static Mock<IFixtureContext> CreateContext(bool allowImplicit)
        {
            var options = new FixtureOptions { AllowImplicitConversion = allowImplicit };
            var context = new Mock<IFixtureContext>();
            context.Setup(c => c.GetBaseOptions()).Returns(options);
            return context;
        }

        private class SourceDefined
        {
            public int Value;
            public static implicit operator int(SourceDefined s) => s.Value;
        }

        private struct TargetDefined
        {
            public int Value;
            public static implicit operator TargetDefined(int v) => new() { Value = v };
        }

        [Test]
        public void Constructor_Constructs()
        {
            Assert.DoesNotThrow(() => new ImplicitConverter());
        }

        [Test]
        public void Convert_ImplicitOnSourceType_Converts()
        {
            var target = typeof(int);
            var value = new SourceDefined { Value = 42 };
            var context = CreateContext(true).Object;

            var converter = new ImplicitConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void Convert_ImplicitOnTargetType_Converts()
        {
            var target = typeof(TargetDefined);
            var value = 42;
            var context = CreateContext(true).Object;

            var converter = new ImplicitConverter();

            var result = converter.Convert(target, value, context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<TargetDefined>());
                Assert.That(((TargetDefined)result!).Value, Is.EqualTo(42));
            }
        }

        [Test]
        public void Convert_NoImplicitOperatorExists_ReturnsNoResult()
        {
            var target = typeof(string);
            var value = 42;
            var context = CreateContext(true).Object;

            var converter = new ImplicitConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_AllowImplicitConversionDisabled_ReturnsNoResult()
        {
            var target = typeof(int);
            var value = new SourceDefined { Value = 42 };
            var context = CreateContext(false).Object;

            var converter = new ImplicitConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        private class Wrapper<T>
        {
            public T Value = default!;
            public static implicit operator Wrapper<T>(T value) => new() { Value = value };
        }

        [Test]
        public void Convert_GenericTypeWithImplicit_Converts()
        {
            var target = typeof(Wrapper<int>);
            var value = 42;
            var context = CreateContext(true).Object;

            var converter = new ImplicitConverter();

            var result = converter.Convert(target, value, context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<Wrapper<int>>());
                Assert.That(((Wrapper<int>)result!).Value, Is.EqualTo(42));
            }
        }

        private class AcceptsBase
        {
            public static implicit operator AcceptsBase(Base b) => new();
        }

        private class Base { }
        private class Derived : Base { }

        [Test]
        public void Convert_ImplicitAcceptsBaseType_ValueIsDerived_Converts()
        {
            var target = typeof(AcceptsBase);
            var value = new Derived();
            var context = CreateContext(true).Object;

            var converter = new ImplicitConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<AcceptsBase>());
        }

        private class ExplicitOnly
        {
            public int Value;
            public static explicit operator int(ExplicitOnly e) => e.Value;
        }

        [Test]
        public void Convert_ExplicitConversionExists_ButNoImplicit_ReturnsNoResult()
        {
            var target = typeof(int);
            var value = new ExplicitOnly { Value = 42 };
            var context = CreateContext(true).Object;

            var converter = new ImplicitConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
