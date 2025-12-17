using System.Reflection;

namespace FixtureBuilder.Tests
{
    internal class Helpers
    {
        internal static T GetFixture<T>(IFixtureConfigurator<T> fixture) where T : class
        {
            return (T)fixture.GetType().GetField("_fixture", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(fixture)!;
        }

        internal static T GetFixture<T>(IFixtureConstructor<T> fixture) where T : class
        {
            return (T)fixture.GetType().GetField("_fixture", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(fixture)!;
        }
    }

    internal class TestClass
    {
        public string Text { get; set; } = null!;
        public virtual int Number { get; protected set; }
        protected string _inheritedField = null!;
        public string InheritedFieldGetter => _inheritedField;
        public NestedClass NestedClass { get; set; } = null!;
    }

    internal class DerivedTestClass : TestClass
    {
        public override int Number { get; protected set; }
    }

    internal class TwiceDerivedClass : TestClass { }

    internal interface ITestInterface
    {
        string ImplicitProperty { get; set; }
    }

    internal class InterfaceTestClass : ITestInterface
    {
        public string ImplicitProperty { get; set; } = null!;
    }

    internal class GenericClass<T> where T : class
    {
        public T Value { get; set; } = null!;
    }

    internal class NestedClass
    {
        public string Value { get; set; } = null!;
        public DeeperNestedClass DeeperNestedClass { get; set; } = null!;
    }

    internal class DeeperNestedClass
    {
        public int Value { get; set; }

        private readonly string _privateField = null!;
        public string Text => _privateField;
    }
}
