using System.Reflection;

namespace FixtureBuilder
{
    internal class DataMemberInfo
    {
        private readonly PropertyInfo? _property;
        public PropertyInfo Property => _property ?? throw new InvalidOperationException("");
        private readonly FieldInfo? _field;
        public FieldInfo Field => _field ?? throw new InvalidOperationException("");

        public bool IsPropertyInfo => _property != null;
        public bool IsFieldInfo => _field != null;

        public DataMemberInfo(PropertyInfo propertyInfo) => _property = propertyInfo;
        public DataMemberInfo(FieldInfo fieldInfo) => _field = fieldInfo;

        public Type DataMemberType => IsPropertyInfo ? _property!.PropertyType : _field!.FieldType;
        public Type? DeclaringType => IsPropertyInfo ? _property!.DeclaringType : _field!.DeclaringType;
        public object? GetValue(object? obj) => IsPropertyInfo ? _property!.GetValue(obj) : _field!.GetValue(obj);
        public void SetValue(object? obj, object? value) { if (IsPropertyInfo) { _property!.SetValue(obj, value); } else { _field!.SetValue(obj, value); } }
        public IEnumerable<CustomAttributeData> CustomAttributes => IsPropertyInfo ? _property!.CustomAttributes : _field!.CustomAttributes;

        public bool CanReadProperty => IsPropertyInfo ? _property!.CanRead : throw new InvalidOperationException("");
        public bool CanWriteProperty => IsPropertyInfo ? _property!.CanWrite : throw new InvalidOperationException("");
        public ParameterInfo[] GetPropertyIndexParameters() => IsPropertyInfo ? _property!.GetIndexParameters() : throw new InvalidOperationException("");

        public bool IsStaticField => IsFieldInfo ? _field!.IsStatic : throw new InvalidOperationException("");
    }
}
