using System.Reflection;
using FixtureBuilder.Configuration;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Creation.UninitializedProviders
{
    /// <summary>
    /// Iterates over an object's data members (properties and fields) and initializes any
    /// that are <see langword="null"/> by resolving values through an
    /// <see cref="IUninitializedProvider"/>. Members are discovered via reflection
    /// using <see cref="Extensions.TypeExtensions.GetDataMembers"/> and filtered through an
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

        public void InitializeMembers(object instance, InitializeMembers initializeMembers, Type rootType, IFixtureContext context, RecursiveResolveContext recursiveResolveContext)
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
                    MemberInfo requestSource = dataMember.IsPropertyInfo ? dataMember.Property : dataMember.Field;
                    var request = new FixtureRequest(dataMember.DataMemberType, requestSource, rootType, dataMember.Name);

                    value = _uninitializedProvider.ResolveUninitialized(request, initializeMembers, context, recursiveResolveContext.Branch());

                    if (value is NoResult)
                    {
                        value = dataMember.DataMemberType.IsValueType
                            ? Activator.CreateInstance(dataMember.DataMemberType)
                            : null;
                    }

                    if (dataMember.IsPropertyInfo
                        && !dataMember.Property.CanWrite
                        && dataMember.DeclaringType!.TryGetField(FieldHelper.GetAutoBackingFieldName(dataMember.Property), out var backingField))
                    {
                        backingField.SetValue(instance, value);
                    }
                    else
                    {
                        dataMember.SetValue(instance, value);
                    }
                }
            }
        }
    }
}
