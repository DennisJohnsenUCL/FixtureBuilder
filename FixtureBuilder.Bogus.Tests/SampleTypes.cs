using Bogus;
using FixtureBuilder.Core;

namespace FixtureBuilder.Bogus.Tests
{
    #region Holders

    internal class PropertyHolder<T>
    {
        public T Value { get; set; } = default!;
    }

    internal class FieldHolder<T>
    {
        public T Value = default!;
    }

    internal class ReadOnlyPropertyHolder<T>
    {
        private readonly T _value = default!;
        public T Value => _value;
    }

    #endregion

    #region Type kinds

    internal class EmptyClass;

    #endregion

    #region Construction

    internal class ConstructorSetsValueClass
    {
        public string Value { get; set; }

        public ConstructorSetsValueClass() => Value = "constructor";
    }

    internal class StringConstructorClass(string value)
    {
        public string Value { get; } = value;
    }

    internal sealed class PrivateConstructorClass
    {
        public string Value { get; }

        private PrivateConstructorClass() => Value = "constructor";
    }

    #endregion

    #region Type links

    internal interface IMarkerInterface;

    internal class MarkerImplementationClass : IMarkerInterface;

    internal class InterfaceParameterClass(IMarkerInterface dependency)
    {
        public IMarkerInterface Dependency { get; } = dependency;
    }

    internal class MarkerInterfaceTypeLink : ICustomTypeLink
    {
        public Type? Link(Type target) => target == typeof(IMarkerInterface) ? typeof(MarkerImplementationClass) : null;
    }

    #endregion

    #region Providers and converters

    internal class FixedStringProvider(string value) : ICustomProvider
    {
        public object? ResolveValue(FixtureRequest request) => request.Type == typeof(string) ? value : NoResult.Instance;
    }

    internal class FakerLocaleProvider : IBogusCustomProvider
    {
        public object? ResolveValue(FixtureRequest request, Faker faker) => request.Type == typeof(string) ? faker.Locale : NoResult.Instance;
    }

    internal class WiderBackingFieldClass
    {
        private readonly long _value;
        public int Value => (int)_value;
    }

    internal class FixedLongConverter(long result) : ICustomConverter
    {
        public object? Convert(Type target, object value) => target == typeof(long) ? result : NoResult.Instance;
    }

    #endregion
}
