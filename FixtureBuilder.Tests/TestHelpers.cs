#pragma warning disable CA1822 // Mark members as static

using System.Reflection;
using System.Runtime.CompilerServices;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Tests
{
    internal class TestHelper
    {
        internal static IFixtureConfigurator<T> MakeFixture<T>() where T : class
        {
            var instance = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
            return Fixture.New(instance);
        }

        internal static void SetOption<T>(IFixtureConstructor<T> fixture, Action<FixtureOptions> action) where T : class
        {
            ((IFixtureContext)fixture
                .GetType()
                .GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(fixture)!)
            .SetOptions(action);
        }

        internal static IFixtureContext GetContext<T>(IFixtureConstructor<T> fixture) where T : class
        {
            return (IFixtureContext)fixture
                .GetType().GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(fixture)!;
        }

        internal static void SetContext<T>(IFixtureConstructor<T> fixture, IFixtureContext context) where T : class
        {
            fixture
                .GetType()
                .GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(fixture, context);
        }

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
        public NestedClass NoSetterProperty { get; } = null!;

        public NestedClass TestMethod() => new();
    }

    class ExplicitBackingFieldClass
    {
        private readonly string _text = null!;
        public string Text => _text;
    }

    record TestRecord(string Text, int Number);

    internal class DerivedTestClass : TestClass
    {
        public override int Number { get; protected set; }
    }

    class ClassWithNullable
    {
        public NestedClass? NullableClass { get; set; } = null;
        public NestedClass NonNullableClass { get; set; } = null!;
        private string _text = null!;
        public string Text { get => _text; set { _text = value; } }
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
        public DerivedNestedClass DerivedNestedClass { get; set; } = null!;
    }

    internal class DeeperNestedClass
    {
        public virtual int Value { get; set; }
        public string String { get; set; } = null!;

        protected readonly string _privateField = null!;
        public string Text => _privateField;
    }

    internal class DerivedNestedClass : DeeperNestedClass
    {
        public override int Value { get; set; }
    }
}
