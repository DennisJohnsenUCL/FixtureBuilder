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

            var instance = RuntimeHelpers.GetUninitializedObject(request.Type)
                ?? throw new Exception(""); //TODO: Add

            if (initializeMembers != InitializeMembers.None)
                _memberInitializer.InitializeMembers(instance, initializeMembers, context);

            return instance;
        }
    }
}
