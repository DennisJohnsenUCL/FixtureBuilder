using System.Reflection;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Helpers
{
    internal class DataMemberInfo : MemberInfo
    {
        private readonly PropertyInfo? _property;
        public PropertyInfo Property => _property ?? throw new InvalidOperationException("This DataMember is a Field.");
        private readonly FieldInfo? _field;
        public FieldInfo Field => _field ?? throw new InvalidOperationException("This DataMember is a Property.");

        public bool IsPropertyInfo => _property != null;
        public bool IsFieldInfo => _field != null;

        public DataMemberInfo(PropertyInfo propertyInfo) => _property = propertyInfo;
        public DataMemberInfo(FieldInfo fieldInfo) => _field = fieldInfo;

        public Type DataMemberType => IsPropertyInfo ? _property!.PropertyType : _field!.FieldType;
        public override Type? DeclaringType => IsPropertyInfo ? _property!.DeclaringType : _field!.DeclaringType;
        public object? GetValue(object? obj) => IsPropertyInfo ? _property!.GetValue(obj) : _field!.GetValue(obj);
        public void SetValue(object? obj, object? value) { if (IsPropertyInfo) { _property!.SetValue(obj, value); } else { _field!.SetValue(obj, value); } }
        public override IEnumerable<CustomAttributeData> CustomAttributes => IsPropertyInfo ? _property!.CustomAttributes : _field!.CustomAttributes;
        public override bool IsDefined(Type attributeType, bool inherit) => IsPropertyInfo ? _property!.IsDefined(attributeType, inherit) : _field!.IsDefined(attributeType, inherit);
        public override Module Module => IsPropertyInfo ? _property!.Module : _field!.Module;

        public bool IsStatic => IsPropertyInfo ? _property!.IsStatic(true) : _field!.IsStatic;

        public override string Name => IsPropertyInfo ? _property!.Name : _field!.Name;
        public override Type? ReflectedType => IsPropertyInfo ? _property!.ReflectedType : _field!.ReflectedType;
        public override MemberTypes MemberType => IsPropertyInfo ? _property!.MemberType : _field!.MemberType;
        public override object[] GetCustomAttributes(bool inherit) => IsPropertyInfo ? _property!.GetCustomAttributes(inherit) : _field!.GetCustomAttributes(inherit);
        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => IsPropertyInfo ? _property!.GetCustomAttributes(attributeType, inherit) : _field!.GetCustomAttributes(attributeType, inherit);
    }
}
