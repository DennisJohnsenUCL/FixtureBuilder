using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder
{
    public class FixtureFactory
    {
        private readonly IFixtureContext _context;

        public FixtureFactory()
        {
            _context = InitializeContext();
        }

        public FixtureFactory(FixtureOptions options)
        {
            _context = InitializeContext(options);
        }

        private static IFixtureContext InitializeContext(FixtureOptions? options = null)
        {
            return FixtureContextFactory.CreateEagerContext(options);
        }
    }
}
