using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using FixtureBuilder.Helpers;

namespace FixtureBuilder.UninitializedProviders
{
    internal class DataMemberSkipFilter : IDataMemberSkipFilter
    {
        private static readonly string[] SkippedNamespacePrefixes = ["System", "Microsoft", "RunTime"];

        public bool ShouldSkip(DataMemberInfo dataMember, InitializeMembers initializeMembers)
        {
            if (IsFromSkippedNamespace(dataMember) || dataMember.IsStatic)
                return true;

            if (IsInaccessibleProperty(dataMember))
                return true;

            if (IsCompilerGeneratedField(dataMember))
                return true;

            if (initializeMembers == InitializeMembers.NonNullables && IsNullable(dataMember))
                return true;

            return false;
        }

        private static bool IsFromSkippedNamespace(DataMemberInfo dataMember)
        {
            var ns = dataMember.DeclaringType?.Namespace;
            return ns != null && SkippedNamespacePrefixes.Any(prefix => ns.StartsWith(prefix));
        }

        private static bool IsInaccessibleProperty(DataMemberInfo dataMember)
        {
            return dataMember.IsPropertyInfo
                && (!dataMember.Property.CanRead
                    || !dataMember.Property.CanWrite
                    || dataMember.Property.GetIndexParameters().Length > 0);
        }

        private static bool IsCompilerGeneratedField(DataMemberInfo dataMember)
        {
            return dataMember.IsFieldInfo
                && dataMember.IsDefined(typeof(CompilerGeneratedAttribute), false);
        }

        private static bool IsNullable(DataMemberInfo dataMember)
        {
            return IsNullableValueType(dataMember.DataMemberType)
                || IsNullableReferenceType(dataMember);
        }

        private static bool IsNullableValueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        private static bool IsNullableReferenceType(DataMemberInfo dataMember)
        {
            var nullableAttr = FindCustomAttribute(dataMember.CustomAttributes,
                "System.Runtime.CompilerServices.NullableAttribute");

            if (nullableAttr != null)
                return IsMarkedNullable(nullableAttr);

            return InheritsNullableContext(dataMember);
        }

        private static bool IsMarkedNullable(CustomAttributeData nullableAttr)
        {
            if (nullableAttr.ConstructorArguments.Count != 1)
                return false;

            var arg = nullableAttr.ConstructorArguments[0];

            if (arg.ArgumentType == typeof(byte[])
                && arg.Value is ReadOnlyCollection<CustomAttributeTypedArgument> args
                && args.Count > 0
                && args[0].Value is byte b)
            {
                return b == 2;
            }

            if (arg.ArgumentType == typeof(byte) && arg.Value is byte byteValue)
            {
                return byteValue == 2;
            }

            return false;
        }

        private static bool InheritsNullableContext(DataMemberInfo dataMember)
        {
            var contextAttr =
                FindNullableContextAttribute(dataMember.CustomAttributes)
                ?? FindNullableContextAttribute(dataMember.DeclaringType?.CustomAttributes)
                ?? FindNullableContextAttribute(dataMember.Module.CustomAttributes);

            return contextAttr != null
                && (contextAttr.ConstructorArguments[0].Value as byte?) == 2;
        }

        private static CustomAttributeData? FindNullableContextAttribute(
            IEnumerable<CustomAttributeData>? attributes)
        {
            return attributes == null
                ? null
                : FindCustomAttribute(attributes,
                    "System.Runtime.CompilerServices.NullableContextAttribute");
        }

        private static CustomAttributeData? FindCustomAttribute(
            IEnumerable<CustomAttributeData> attributes, string fullName)
        {
            return attributes.FirstOrDefault(a => a.AttributeType.FullName == fullName);
        }
    }
}
