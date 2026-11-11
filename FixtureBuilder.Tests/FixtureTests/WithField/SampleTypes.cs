namespace FixtureBuilder.Tests.FixtureTests.WithField
{
    internal class ProtectedFieldBaseClass
    {
        protected readonly string _value = null!;
        public string Value => _value;
    }

    internal class DerivedFromProtectedFieldClass : ProtectedFieldBaseClass;

    internal class ChildOfPrivateFieldClass : ReadOnlyPropertyHolder<string>;

    internal class GrandchildOfPrivateFieldClass : ChildOfPrivateFieldClass;
}
