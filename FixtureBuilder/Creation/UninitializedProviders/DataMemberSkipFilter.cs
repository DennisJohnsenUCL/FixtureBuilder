using System.Reflection;
using System.Runtime.CompilerServices;
using FixtureBuilder.Configuration;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Creation.UninitializedProviders
{
    /// <summary>
    /// Determines whether a <see cref="DataMemberInfo"/> should be skipped during fixture
    /// member initialization. Members are skipped if they belong to <c>System</c> or <c>Microsoft</c> namespaces, are static,
    /// are inaccessible properties (write-only, indexed, of a ref struct type such as <see cref="Span{T}"/>,
    /// or read-only without an auto-property backing field), are compiler-generated
    /// backing fields, or — when <see cref="InitializeMembers.NonNullables"/> is specified —
    /// are nullable (either nullable value types or nullable reference types).
    /// </summary>
    internal class DataMemberSkipFilter : IDataMemberSkipFilter
    {
        private static readonly string[] _skippedNamespacePrefixes = ["System", "Microsoft"];

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
            return ns != null && _skippedNamespacePrefixes.Any(prefix => ns == prefix || ns.StartsWith(prefix + "."));
        }

        private static bool IsInaccessibleProperty(DataMemberInfo dataMember)
        {
            if (!dataMember.IsPropertyInfo) return false;
            var property = dataMember.Property;

            var isInaccessible = false;

            if (!property.CanRead) isInaccessible = true;
            if (property.GetIndexParameters().Length > 0) isInaccessible = true;
            if (property.PropertyType.IsByRefLike) isInaccessible = true;

            if (!property.CanWrite
                && !dataMember.DeclaringType!.TryGetField(FieldHelper.GetAutoBackingFieldName(property), out _))
            {
                isInaccessible = true;
            }

            return isInaccessible;
        }

        private static bool IsCompilerGeneratedField(DataMemberInfo dataMember)
        {
            return dataMember.IsFieldInfo
                && dataMember.IsDefined(typeof(CompilerGeneratedAttribute), false);
        }

        private static bool IsNullable(DataMemberInfo dataMember)
        {
            var nullContext = new NullabilityInfoContext();

            var nullInfo = dataMember.IsPropertyInfo
                ? nullContext.Create(dataMember.Property)
                : nullContext.Create(dataMember.Field);

            return nullInfo.ReadState == NullabilityState.Nullable;
        }
    }
}
