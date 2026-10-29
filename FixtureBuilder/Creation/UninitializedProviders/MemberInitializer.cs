using System.Reflection;
using System.Runtime.CompilerServices;
using FixtureBuilder.Configuration;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Creation.UninitializedProviders
{
    /// <summary>
    /// Iterates over an object's data members (properties and fields) and initializes any
    /// that hold their default value by resolving values through an
    /// <see cref="IUninitializedProvider"/>. Members are discovered via reflection
    /// using <see cref="Extensions.TypeExtensions.GetDataMembers"/> and filtered through an
    /// <see cref="IDataMemberSkipFilter"/> before initialization. Members the skip filter rejects,
    /// and properties whose getter or setter throws, are left unchanged; the remaining members are still processed.
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
            var currentType = instance.GetType();
            var dataMembers = currentType.GetDataMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).ToList();
            currentType = currentType.BaseType;
            while (currentType != null && currentType != typeof(object))
            {
                dataMembers.AddRange(currentType.GetDataMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                    .Where(m => m.IsFieldInfo ? m.Field.IsPrivate : m.Property.GetAccessors(true).All(a => a.IsPrivate)));
                currentType = currentType.BaseType;
            }

            foreach (var dataMember in dataMembers)
            {
                if (_skipFilter.ShouldSkip(dataMember, initializeMembers)) continue;

                object? value;
                try
                {
                    value = dataMember.GetValue(instance);
                }
                catch (TargetInvocationException) { continue; }

                var type = dataMember.DataMemberType;
                var defaultValue = type.IsValueType && Nullable.GetUnderlyingType(type) is null
                    ? RuntimeHelpers.GetUninitializedObject(type)
                    : null;

                if (!RuntimeHelpers.Equals(value, defaultValue)) continue;

                var linked = context.UnwrapAndLink(type, rootType);
                var request = new FixtureRequest(linked, dataMember.MemberInfo, rootType);

                value = _uninitializedProvider.ResolveUninitialized(request, initializeMembers, context, recursiveResolveContext.Branch());

                if (value is NoResult) continue;

                if (dataMember.IsPropertyInfo
                    && !dataMember.Property.CanWrite
                    && dataMember.DeclaringType!.TryGetField(FieldHelper.GetAutoBackingFieldName(dataMember.Property), out var backingField))
                {
                    backingField.SetValue(instance, value);
                }
                else
                {
                    try
                    {
                        dataMember.SetValue(instance, value);
                    }
                    catch (TargetInvocationException) { }
                }
            }
        }
    }
}
