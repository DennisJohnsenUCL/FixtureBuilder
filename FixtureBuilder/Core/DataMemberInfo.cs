using System.Diagnostics;
using System.Reflection;

/// <summary>
/// Provides a unified view over either a <see cref="PropertyInfo"/> or a <see cref="FieldInfo"/>,
/// allowing consuming code to treat properties and fields interchangeably as "data members."
/// Use <see cref="DataMemberInfo.MemberInfo"/> where the wrapped reflection member itself is needed.
/// </summary>
/// <remarks>
/// Each instance wraps exactly one of the two backing types. Check <see cref="IsPropertyInfo"/>
/// or <see cref="IsFieldInfo"/> before using the typed accessors, as the wrong one
/// (e.g. <see cref="Property"/> on a field-backed instance) returns <see langword="null"/>.
/// All other members delegate transparently to whichever backing member is present.
/// </remarks>
internal class DataMemberInfo
{
    private readonly PropertyInfo? _property;

    /// <summary>
    /// Gets the underlying <see cref="PropertyInfo"/>.
    /// </summary>
    public PropertyInfo Property => _property!;

    private readonly FieldInfo? _field;

    /// <summary>
    /// Gets the underlying <see cref="FieldInfo"/>.
    /// </summary>
    public FieldInfo Field => _field!;

    /// <summary>
    /// Gets the underlying <see cref="PropertyInfo"/> or <see cref="FieldInfo"/>, whichever this instance wraps.
    /// </summary>
    public MemberInfo MemberInfo => IsPropertyInfo ? _property! : _field!;

    /// <summary>Gets <see langword="true"/> when this instance wraps a <see cref="PropertyInfo"/>.</summary>
    public bool IsPropertyInfo => _property != null;

    /// <summary>Gets <see langword="true"/> when this instance wraps a <see cref="FieldInfo"/>.</summary>
    public bool IsFieldInfo => _field != null;

    /// <summary>
    /// Initializes a new <see cref="DataMemberInfo"/> backed by the specified property.
    /// </summary>
    /// <param name="propertyInfo">The property to wrap.</param>
    public DataMemberInfo(PropertyInfo propertyInfo) => _property = propertyInfo;

    /// <summary>
    /// Initializes a new <see cref="DataMemberInfo"/> backed by the specified field.
    /// </summary>
    /// <param name="fieldInfo">The field to wrap.</param>
    public DataMemberInfo(FieldInfo fieldInfo) => _field = fieldInfo;

    /// <summary>
    /// Creates a <see cref="DataMemberInfo"/> backed by the specified property or field.
    /// </summary>
    /// <param name="memberInfo">The <see cref="PropertyInfo"/> or <see cref="FieldInfo"/> to wrap.</param>
    /// <returns>A <see cref="DataMemberInfo"/> wrapping <paramref name="memberInfo"/>.</returns>
    public static DataMemberInfo FromMemberInfo(MemberInfo memberInfo)
    {
        return memberInfo switch
        {
            PropertyInfo pi => new(pi),
            FieldInfo fi => new(fi),
            _ => throw new UnreachableException()
        };
    }

    /// <summary>
    /// Gets the <see cref="Type"/> of the data member — <see cref="PropertyInfo.PropertyType"/>
    /// for properties, <see cref="FieldInfo.FieldType"/> for fields.
    /// </summary>
    public Type DataMemberType => IsPropertyInfo ? _property!.PropertyType : _field!.FieldType;

    /// <summary>
    /// Returns the value of this data member on the given object instance.
    /// </summary>
    /// <param name="obj">The object whose member value will be read.</param>
    /// <returns>The member's current value.</returns>
    public object? GetValue(object? obj) => IsPropertyInfo ? _property!.GetValue(obj) : _field!.GetValue(obj);

    /// <summary>
    /// Sets the value of this data member on the given object instance.
    /// </summary>
    /// <param name="obj">The object whose member value will be written.</param>
    /// <param name="value">The value to assign.</param>
    public void SetValue(object? obj, object? value) { if (IsPropertyInfo) { _property!.SetValue(obj, value); } else { _field!.SetValue(obj, value); } }

    /// <summary>
    /// Gets the name of the data member.
    /// </summary>
    public string Name => MemberInfo.Name;
}
