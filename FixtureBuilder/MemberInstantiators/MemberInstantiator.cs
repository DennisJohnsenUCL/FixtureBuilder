using FixtureBuilder.ConstructingProviders;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.UninitializedProviders;

namespace FixtureBuilder.MemberInstantiators
{
    internal class MemberInstantiator<T> : IConstructor<T>
    {
        private readonly IFixtureContext _context;

        internal MemberInstantiator(IFixtureContext context)
        {
            _context = context;
        }

        public T UseAutoConstructor()
        {
            return (T)_context.AutoResolve(new FixtureRequest(typeof(T)), _context);
        }

        public T UseConstructor(params object?[] arguments)
        {
            return (T)new ConstructingProvider().ResolveWithArguments(new FixtureRequest(typeof(T)), arguments);
        }

        public T CreateUninitialized()
            => CreateUninitialized(InitializeMembers.None);

        public T CreateUninitialized(InitializeMembers initializeMembers)
        {
            var instance = _context.ResolveUninitialized(new FixtureRequest(typeof(T)), initializeMembers, _context)
                ?? throw new InvalidOperationException("");
            return (T)instance;
        }
    }
}
