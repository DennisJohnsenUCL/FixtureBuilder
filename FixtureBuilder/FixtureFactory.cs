using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder
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

        private static IFixtureContext InitializeContext(FixtureOptions? options = null)
        {
            return FixtureContextFactory.CreateEagerContext(options);
        }
    }
}
