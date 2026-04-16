using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class FixtureFactory
    {
        private readonly IFixtureContext _context;

        public FixtureOptions Options
        {
            get => _context.Options;
            set => _context.Options = value;
        }

        public FixtureFactory()
        {
            _context = InitializeContext();
        }

        public FixtureFactory(FixtureOptions options)
        {
            _context = InitializeContext(options);
        }

        public IFixtureConstructor<T> New<T>() where T : class
        {
            return new Fixture<T>(_context);
        }

        public IFixtureConstructor<T> New<T>(T instance) where T : class
        {
            ArgumentNullException.ThrowIfNull(instance);
            return new Fixture<T>(instance, _context);
        }

        public void SetOptions(Action<FixtureOptions> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            _context.SetOptions(action);
        }

        public void AddTypeLink(ITypeLink link)
        {
            _context.AddTypeLink(link);
        }

        public void AddProvider(ICustomProvider provider)
        {
            var adaptedProvider = new CustomProviderAdapter(provider);
            _context.AddProvider(adaptedProvider);
        }

        public void AddConverter(ICustomConverter converter)
        {
            var adaptedConverter = new CustomConverterAdapter(converter);
            _context.AddConverter(adaptedConverter);
        }

        private static IFixtureContext InitializeContext(FixtureOptions? options = null)
        {
            return FixtureContextFactory.CreateEagerContext(options);
        }
    }
}
