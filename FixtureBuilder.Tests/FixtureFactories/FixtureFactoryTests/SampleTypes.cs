using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    #region Construction

    internal class StringAndIntConstructorClass(string text, int number)
    {
        public string Text { get; } = text;
        public int Number { get; } = number;
    }

    #endregion

    #region Providers

    internal class FixedStringProvider(string value) : ICustomProvider
    {
        public object? ResolveValue(FixtureRequest request) => request.Type == typeof(string) ? value : NoResult.Instance;
    }

    #endregion

    #region Type links

    internal class MarkerInterfaceTypeLink : ICustomTypeLink
    {
        public Type? Link(Type target) => target == typeof(IMarkerInterface) ? typeof(MarkerImplementationClass) : null;
    }

    #endregion
}
