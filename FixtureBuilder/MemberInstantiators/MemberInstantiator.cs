using FixtureBuilder.ConstructingProviders;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.UninitializedProviders;

namespace FixtureBuilder.MemberInstantiators
{
    internal class MemberInstantiator<T> : IConstructor<T>
    {
        private readonly FixtureRequest _request;
        private readonly IFixtureContext _context;

        internal MemberInstantiator(FixtureRequest request, IFixtureContext context)
        {
            _request = request;
            _context = context;
        }

        public T UseAutoConstructor()
        {
            return (T)_context.AutoResolve(_request, _context);
        }

        public T UseConstructor(params object?[] arguments)
        {
            return (T)new ConstructingProvider().ResolveWithArguments(_request, arguments);
        }

        public T CreateUninitialized()
            => CreateUninitialized(InitializeMembers.None);

        public T CreateUninitialized(InitializeMembers initializeMembers)
        {
            var instance = _context.ResolveUninitialized(_request, initializeMembers, _context)
                ?? throw new InvalidOperationException("Failed to intantiate {typeof(T).Name} uninitialized.");
            return (T)instance;
        }
    }
}
