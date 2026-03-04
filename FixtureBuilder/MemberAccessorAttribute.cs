namespace FixtureBuilder
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal class MemberAccessorAttribute : Attribute
    {
        public AccessorTypes AccessorTypes { get; }
        public Type? Type { get; }
        public GenericSources? GenericSources { get; }
        public int? GenericIndex { get; }

        public MemberAccessorAttribute(AccessorTypes accessorTypes, Type type)
        {
            AccessorTypes = accessorTypes;
            Type = type;
        }

        public MemberAccessorAttribute(AccessorTypes accessorTypes, GenericSources genericSource, int genericIndex)
        {
            AccessorTypes = accessorTypes;
            GenericSources = genericSource;
            GenericIndex = genericIndex;
        }
    }

    internal enum AccessorTypes
    {
        Field,
        Method
    }

    internal enum GenericSources
    {
        Class,
        Method
    }
}
