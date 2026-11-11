namespace FixtureBuilder.Tests.FixtureTests
{
    #region Type kinds

    internal record PositionalRecord(string Value);

    #endregion

    #region Inheritance

    internal class DerivedClass : PropertyHolder<string>;

    internal class DerivedChainClass : PropertyHolder<PropertyHolder<string>>;

    internal class VirtualPropertyClass
    {
        public virtual int Value { get; protected set; }
    }

    internal class OverridingClass : VirtualPropertyClass
    {
        public override int Value { get; protected set; }
    }

    #endregion

    #region Interfaces

    internal interface IValueInterface
    {
        string Value { get; set; }
    }

    internal class ImplicitImplementationClass : IValueInterface
    {
        public string Value { get; set; } = null!;
    }

    #endregion

    #region Backing fields

    internal class UnconventionalBackingFieldClass
    {
        private readonly string _unconventionalName = null!;
        public string Value => _unconventionalName;
    }

    #endregion

    #region Methods

    internal class PublicMethodClass
    {
        public int Calls { get; private set; }

        public void Call() => Calls++;
    }

    #endregion
}
