using System.Reflection;
using FixtureBuilder.Extensions;

/// <summary>
/// Provides a unified view over either a <see cref="PropertyInfo"/> or a <see cref="FieldInfo"/>,
/// allowing consuming code to treat properties and fields interchangeably as "data members."
/// Inherits from <see cref="MemberInfo"/> so it can be used wherever reflection metadata is expected.
/// </summary>
/// <remarks>
/// Each instance wraps exactly one of the two backing types. Accessing the wrong typed
/// accessor (e.g. <see cref="Property"/> on a field-backed instance) throws
/// <see cref="InvalidOperationException"/>. All other members delegate transparently
/// to whichever backing member is present.
/// </remarks>
internal class DataMemberInfo : MemberInfo
{
    private readonly PropertyInfo? _property;

    /// <summary>
    /// Gets the underlying <see cref="PropertyInfo"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">This instance wraps a field, not a property.</exception>
    public PropertyInfo Property => _property ?? throw new InvalidOperationException("This DataMember is a Field.");

    private readonly FieldInfo? _field;

    /// <summary>
    /// Gets the underlying <see cref="FieldInfo"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">This instance wraps a property, not a field.</exception>
    public FieldInfo Field => _field ?? throw new InvalidOperationException("This DataMember is a Property.");

    /// <summary>Gets <see langword="true"/> when this instance wraps a <see cref="PropertyInfo"/>.</summary>
    public bool IsPropertyInfo => _property != null;

    /// <summary>Gets <see langword="true"/> when this instance wraps a <see cref="FieldInfo"/>.</summary>
    public bool IsFieldInfo => _field != null;

    /// <summary>
    /// Initialises a new <see cref="DataMemberInfo"/> backed by the specified property.
    /// </summary>
    /// <param name="propertyInfo">The property to wrap.</param>
    public DataMemberInfo(PropertyInfo propertyInfo) => _property = propertyInfo;

    /// <summary>
    /// Initialises a new <see cref="DataMemberInfo"/> backed by the specified field.
    /// </summary>
    /// <param name="fieldInfo">The field to wrap.</param>
    public DataMemberInfo(FieldInfo fieldInfo) => _field = fieldInfo;

    public static DataMemberInfo FromMemberInfo(MemberInfo memberInfo)
    {
        if (memberInfo is PropertyInfo pi) return new(pi);
        if (memberInfo is FieldInfo fi) return new(fi);
        throw new InvalidOperationException("MemberInfo must be a PropertyInfo or FieldInfo to be a valid DataMemberInfo.");
    }

    public bool TryIsPropertyInfo(out PropertyInfo propertyInfo)
    {
        propertyInfo = _property!;
        return IsPropertyInfo;
    }

    /// <summary>
    /// Gets the <see cref="Type"/> of the data member — <see cref="PropertyInfo.PropertyType"/>
    /// for properties, <see cref="FieldInfo.FieldType"/> for fields.
    /// </summary>
    public Type DataMemberType => IsPropertyInfo ? _property!.PropertyType : _field!.FieldType;

    /// <inheritdoc/>
    public override Type? DeclaringType => IsPropertyInfo ? _property!.DeclaringType : _field!.DeclaringType;

    /// <summary>
    /// Returns the value of this data member on the given object instance.
    /// </summary>
    /// <param name="obj">The object whose member value will be read, or <see langword="null"/> for static members.</param>
    /// <returns>The member's current value.</returns>
    public object? GetValue(object? obj) => IsPropertyInfo ? _property!.GetValue(obj) : _field!.GetValue(obj);

    /// <summary>
    /// Sets the value of this data member on the given object instance.
    /// </summary>
    /// <param name="obj">The object whose member value will be written, or <see langword="null"/> for static members.</param>
    /// <param name="value">The value to assign.</param>
    public void SetValue(object? obj, object? value) { if (IsPropertyInfo) { _property!.SetValue(obj, value); } else { _field!.SetValue(obj, value); } }

    /// <inheritdoc/>
    public override IEnumerable<CustomAttributeData> CustomAttributes => IsPropertyInfo ? _property!.CustomAttributes : _field!.CustomAttributes;

    /// <inheritdoc/>
    public override bool IsDefined(Type attributeType, bool inherit) => IsPropertyInfo ? _property!.IsDefined(attributeType, inherit) : _field!.IsDefined(attributeType, inherit);

    /// <inheritdoc/>
    public override Module Module => IsPropertyInfo ? _property!.Module : _field!.Module;

    /// <summary>
    /// Gets a value indicating whether the underlying member is static.
    /// </summary>
    public bool IsStatic => IsPropertyInfo ? _property!.IsStatic(true) : _field!.IsStatic;

    /// <inheritdoc/>
    public override string Name => IsPropertyInfo ? _property!.Name : _field!.Name;

    /// <inheritdoc/>
    public override Type? ReflectedType => IsPropertyInfo ? _property!.ReflectedType : _field!.ReflectedType;

    /// <inheritdoc/>
    public override MemberTypes MemberType => IsPropertyInfo ? _property!.MemberType : _field!.MemberType;

    /// <inheritdoc/>
    public override object[] GetCustomAttributes(bool inherit) => IsPropertyInfo ? _property!.GetCustomAttributes(inherit) : _field!.GetCustomAttributes(inherit);

    /// <inheritdoc/>
    public override object[] GetCustomAttributes(Type attributeType, bool inherit) => IsPropertyInfo ? _property!.GetCustomAttributes(attributeType, inherit) : _field!.GetCustomAttributes(attributeType, inherit);
}
