namespace FixtureBuilder
{
    internal class RecursiveResolveContext
    {
        public HashSet<Type> Types { get; }

        public RecursiveResolveContext()
        {
            Types = [];
        }

        public RecursiveResolveContext(HashSet<Type> types)
        {
            Types = [.. types];
        }

        public bool Add(Type type)
        {
            return Types.Add(type);
        }
    }
}
