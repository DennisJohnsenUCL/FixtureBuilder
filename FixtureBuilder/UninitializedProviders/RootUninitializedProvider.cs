using System.Runtime.CompilerServices;
using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.UninitializedProviders
{
    /// <summary>
    /// Creates uninitialized instances of requested types using
    /// <see cref="RuntimeHelpers.GetUninitializedObject"/> and optionally initializes their
    /// members via an <see cref="IMemberInitializer"/>. Throws an
    /// <see cref="InvalidOperationException"/> wrapping the original exception if the type
    /// cannot be created uninitialized.
    /// </summary>
    internal class RootUninitializedProvider : IFixtureUninitializedProvider
    {
        private readonly IMemberInitializer _memberInitializer;

        public RootUninitializedProvider(IMemberInitializer memberInitializer)
        {
            ArgumentNullException.ThrowIfNull(memberInitializer);
            _memberInitializer = memberInitializer;
        }

        public object? ResolveUninitialized(FixtureRequest request, InitializeMembers initializeMembers, IFixtureContext context)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(context);

            if (request.Type.IsInterface || request.Type.IsAbstract) return null;

            object? instance = null;
            try
            {
                instance = RuntimeHelpers.GetUninitializedObject(request.Type);
            }
            catch (Exception) { }

            if (instance is not null && initializeMembers != InitializeMembers.None)
                _memberInitializer.InitializeMembers(instance, initializeMembers, context);

            return instance;
        }
    }
}
