using FixtureBuilder.FixtureContexts;
using System.Runtime.CompilerServices;

namespace FixtureBuilder.UninitializedProviders
{
    internal class RootUninitializedProvider : IFixtureUninitializedProvider
    {
        private readonly IMemberInitializer _memberInitializer;

        public RootUninitializedProvider(IMemberInitializer memberInitializer)
        {
            ArgumentNullException.ThrowIfNull(memberInitializer);
            _memberInitializer = memberInitializer;
        }

        public object ResolveUninitialized(FixtureRequest request, InitializeMembers initializeMembers, IFixtureContext context)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(context);

            object instance;
            try
            {
                instance = RuntimeHelpers.GetUninitializedObject(request.Type);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Cannot initialize type {request.Type.Name} uninitialized.", ex);
            }

            if (initializeMembers != InitializeMembers.None)
                _memberInitializer.InitializeMembers(instance, initializeMembers, context);

            return instance;
        }
    }
}
