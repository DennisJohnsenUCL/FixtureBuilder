using System.Reflection;
using System.Runtime.CompilerServices;

namespace FixtureBuilder.UninitializedProviders
{
    /// <summary>
    /// Determines whether a <see cref="DataMemberInfo"/> should be skipped during fixture
    /// member initialization. Members are skipped if they belong to system namespaces, are static,
    /// are inaccessible properties (read-only, write-only, or indexed), are compiler-generated
    /// backing fields, or — when <see cref="InitializeMembers.NonNullables"/> is specified —
    /// are nullable (either nullable value types or nullable reference types).
    /// </summary>
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
            var nullContext = new NullabilityInfoContext();

            var nullInfo = dataMember.IsPropertyInfo
                ? nullContext.Create(dataMember.Property)
                : nullContext.Create(dataMember.Field);

            return nullInfo.ReadState == NullabilityState.Nullable;
        }
    }
}
