using System.Reflection;
using FixtureBuilder.Extensions;
using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.UninitializedProviders
{
    internal class MemberInitializer : IMemberInitializer
    {
        private readonly IDataMemberSkipFilter _skipFilter;
        private readonly IFixtureUninitializedProvider _uninitializedProvider;

        public MemberInitializer(IDataMemberSkipFilter skipFilter, IFixtureUninitializedProvider uninitializedProvider)
        {
            ArgumentNullException.ThrowIfNull(skipFilter);
            ArgumentNullException.ThrowIfNull(uninitializedProvider);
            _skipFilter = skipFilter;
            _uninitializedProvider = uninitializedProvider;
        }

        public void InitializeMembers(object instance, InitializeMembers initializeMembers, IFixtureContext context)
        {
            var dataMembers = instance.GetType().GetDataMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var dataMember in dataMembers)
            {
                if (_skipFilter.ShouldSkip(dataMember, initializeMembers)) return;

                var value = dataMember.GetValue(instance);

                if (value == null)
                {
                    FixtureRequest? request = null;
                    if (dataMember.IsPropertyInfo) request = new FixtureRequest(dataMember.DataMemberType, dataMember.Name, dataMember.Property);
                    if (dataMember.IsFieldInfo) request = new FixtureRequest(dataMember.DataMemberType, dataMember.Name, dataMember.Field);
                    if (request == null) return;

                    value = _uninitializedProvider.ResolveUninitialized(request, initializeMembers, context);

                    if (value != null) dataMember.SetValue(instance, value);
                }
            }
        }
    }
}
