using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration
{
    internal class MemberInstantiator<T> : IConstructor<T>
    {
        private readonly Type _rootType;
        private readonly IFixtureContext _context;

        internal MemberInstantiator(Type rootType, IFixtureContext context)
        {
            _rootType = rootType;
            _context = context;
        }

        public T UseAutoConstructor()
        {
            var type = _context.UnwrapAndLink(typeof(T), _rootType);
            var request = new TypeRequest(type, _rootType);

            return (T)_context.AutoConstructingProvider.AutoResolve(request, _context);
        }

        public T UseConstructor(params object?[] arguments)
        {
            var type = typeof(T).Unwrap();

            return (T)_context.ConstructingProvider.ResolveWithArguments(type, arguments);
        }

        public T CreateUninitialized()
            => CreateUninitialized(InitializeMembers.None);

        public T CreateUninitialized(InitializeMembers initializeMembers)
        {
            var type = _context.UnwrapAndLink(typeof(T), _rootType);
            var request = new TypeRequest(type, _rootType);

            var instance = _context.UninitializedProvider.ResolveUninitialized(request, initializeMembers, _context);

            if (instance is NoResult)
                throw new InvalidOperationException($"Failed to instantiate {type.Name} uninitialized for root type {_rootType.Name}.");

            return (T)instance;
        }

        public T UseCustomInstantiator(Func<T> instantiator)
        {
            var instance = instantiator() ?? throw new InvalidOperationException(
                $"Custom instantiator for {typeof(T).Name} returned null. Custom instantiators must always return a valid instance.");

            return instance;
        }
    }
}
