using System.Runtime.CompilerServices;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Creation.UninitializedProviders
{
    /// <summary>
    /// Creates uninitialized instances of requested types using
    /// <see cref="RuntimeHelpers.GetUninitializedObject"/> and optionally initializes their
    /// members via an <see cref="IMemberInitializer"/>. Returns <see cref="NoResult"/> if the
    /// type is an interface, abstract, or cannot be created uninitialized.
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
                if (!context.OptionsFor(request.RootType).AllowResolveCircularDependencies)
                    throw new InvalidOperationException($"Circular dependency detected for {request.Type.Name} in CreateUninitialized.");

                return recursiveResolveContext.AddShell(request.Type);
            }

            if (request.Type.IsInterface || request.Type.IsAbstract) return new NoResult();

            object? instance = new NoResult();
            try
            {
                instance = RuntimeHelpers.GetUninitializedObject(request.Type);
            }
            catch (Exception) { }

            if (instance is not NoResult && initializeMembers != InitializeMembers.None)
            {
                _memberInitializer.InitializeMembers(instance, initializeMembers, request.RootType, context, recursiveResolveContext);
                recursiveResolveContext.Copy(request.Type, instance);
            }

            return instance;
        }
    }
}
