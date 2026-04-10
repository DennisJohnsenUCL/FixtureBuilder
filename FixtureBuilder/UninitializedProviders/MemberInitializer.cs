using System.Reflection;
using FixtureBuilder.Extensions;
using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.UninitializedProviders
{
    /// <summary>
    /// Iterates over an object's data members (properties and fields) and initializes any
    /// that are <see langword="null"/> by resolving values through an
    /// <see cref="IUninitializedProvider"/>. Members are discovered via reflection
    /// using <see cref="DataMemberInfoExtensions.GetDataMembers"/> and filtered through an
    /// <see cref="IDataMemberSkipFilter"/> before initialization. If the skip filter indicates
    /// a member should be skipped, processing stops immediately for all remaining members.
    /// </summary>
    internal class MemberInitializer : IMemberInitializer
    {
        private readonly IDataMemberSkipFilter _skipFilter;
        private readonly IUninitializedProvider _uninitializedProvider;

        public MemberInitializer(IDataMemberSkipFilter skipFilter, IUninitializedProvider uninitializedProvider)
        {
            ArgumentNullException.ThrowIfNull(skipFilter);
            ArgumentNullException.ThrowIfNull(uninitializedProvider);
            _skipFilter = skipFilter;
            _uninitializedProvider = uninitializedProvider;
        }

        public void InitializeMembers(object instance, InitializeMembers initializeMembers, IFixtureContext context, RecursiveResolveContext recursiveResolveContext)
        {
            var dataMembers = instance.GetType().GetDataMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var dataMember in dataMembers)
            {
                if (_skipFilter.ShouldSkip(dataMember, initializeMembers)) continue;

                var value = dataMember.GetValue(instance);

                var defaultValue = dataMember.DataMemberType.IsValueType
                    ? Activator.CreateInstance(dataMember.DataMemberType)
                    : null;

                if (Equals(value, defaultValue))
                {
                    FixtureRequest? request = null;
                    if (dataMember.IsPropertyInfo) request = new FixtureRequest(dataMember.DataMemberType, dataMember.Name, dataMember.Property);
                    if (dataMember.IsFieldInfo) request = new FixtureRequest(dataMember.DataMemberType, dataMember.Name, dataMember.Field);
                    if (request == null) return;

                    value = _uninitializedProvider.ResolveUninitialized(request, initializeMembers, context, recursiveResolveContext);

                    if (value is NoResult)
                    {
                        value = dataMember.DataMemberType.IsValueType
                            ? Activator.CreateInstance(dataMember.DataMemberType)
                            : null;
                    }
                    dataMember.SetValue(instance, value);
                }
            }
        }
    }
}
