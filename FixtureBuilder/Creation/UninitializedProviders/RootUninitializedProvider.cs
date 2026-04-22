using System.Runtime.CompilerServices;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Creation.UninitializedProviders
{
    /// <summary>
    /// Creates uninitialized instances of requested types using
    /// <see cref="RuntimeHelpers.GetUninitializedObject"/> and optionally initializes their
    /// members via an <see cref="IMemberInitializer"/>. Throws an
    /// <see cref="InvalidOperationException"/> wrapping the original exception if the type
    /// cannot be created uninitialized.
    /// </summary>
    internal class RootUninitializedProvider : IUninitializedProvider
    {
        private readonly IMemberInitializer _memberInitializer;

        public RootUninitializedProvider(IMemberInitializer memberInitializer)
        {
            ArgumentNullException.ThrowIfNull(memberInitializer);
            _memberInitializer = memberInitializer;
        }

        public object? ResolveUninitialized(FixtureRequest request, InitializeMembers initializeMembers, IFixtureContext context, RecursiveResolveContext? recursiveResolveContext = null)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(context);

            recursiveResolveContext ??= new();
            if (!recursiveResolveContext.Add(request.Type))
            {
                if (!context.Options.AllowResolveCircularDependencies)
                    throw new InvalidOperationException($"Circular dependency detected for {request.Type.Name} in CreateUninitialized.");

                return recursiveResolveContext.AddShell(request.Type);
            }


            if (request.Type.IsInterface || request.Type.IsAbstract) return null;

            object? instance = new NoResult();
            try
            {
                instance = RuntimeHelpers.GetUninitializedObject(request.Type);
            }
            catch (Exception) { }

            if (instance is not NoResult && instance != null && initializeMembers != InitializeMembers.None)
            {
                _memberInitializer.InitializeMembers(instance, initializeMembers, request.RootType, context, recursiveResolveContext.Branch());
                recursiveResolveContext.Copy(request.Type, instance!);
            }

            return instance;
        }
    }
}
