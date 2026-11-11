using FixtureBuilder.Core;

namespace FixtureBuilder.Tests
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

    internal struct PropertyHolderStruct<T>
    {
        public T Value { get; set; }
    }

    internal struct ReadOnlyPropertyHolderStruct<T>
    {
        private T _value;
        public readonly T Value => _value;
    }

    #endregion

    #region Type kinds

    internal class EmptyClass;

    internal struct PointStruct
    {
        public int X { get; set; }
    }

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

    internal class ClassParameterClass(StringConstructorClass dependency)
    {
        public StringConstructorClass Dependency { get; } = dependency;
    }

    internal sealed class PrivateConstructorClass
    {
        public string Value { get; }

        private PrivateConstructorClass() => Value = "constructor";
    }

    internal abstract class AbstractClass;

    #endregion

    #region Type links

    internal interface IMarkerInterface;

    internal class MarkerImplementationClass : IMarkerInterface;

    internal class OtherMarkerImplementationClass : IMarkerInterface;

    internal class InterfaceParameterClass(IMarkerInterface dependency)
    {
        public IMarkerInterface Dependency { get; } = dependency;
    }

    internal interface IGenericInterface<T>;

    #endregion

    #region Members

    internal class WriteOnlyPropertyClass
    {
        public PropertyHolder<string> Value { set { } }
    }

    #endregion

    #region Conversions

    internal class WiderBackingFieldClass
    {
        private readonly long _value;
        public int Value => (int)_value;
    }

    internal class FixedLongConverter(long result) : ICustomConverter
    {
        public object? Convert(Type target, object value) => target == typeof(long) ? result : NoResult.Instance;
    }

    internal struct ImplicitSourceStruct
    {
        public int Number;
    }

    internal struct ImplicitTargetStruct
    {
        public int Number;

        public static implicit operator ImplicitTargetStruct(ImplicitSourceStruct source) => new() { Number = source.Number };
    }

    #endregion
}

#region Namespaces

// Namespace starts with "System" but is neither System nor inside it.
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace SystemUnderTest
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    internal class Invoice
    {
        public string CreatedBy { get; } = "constructor";
        public string Number { get; set; } = null!;
    }
}

// No namespace.
internal class GlobalNamespaceClass
{
    public string Value { get; set; } = null!;
}

#endregion
