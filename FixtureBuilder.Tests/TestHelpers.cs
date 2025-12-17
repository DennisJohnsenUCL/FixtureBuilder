using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
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

    internal record TestValue(string Text, int Number);

    internal class TestClass : INestedInterface
    {
        public string Text { get; set; } = null!;
        public virtual int Number { get; protected set; }
        public string PropWithoutSetter { get; } = null!;
        public string PropWithPrivateSetter { get; private set; } = null!;
        private readonly string _privateExplicitField = null!;
        public string PrivateExplicitField => _privateExplicitField;
        private readonly string privateExplicitNoUnderscoreField = null!;
        public string PrivateExplicitNoUnderscoreField => privateExplicitNoUnderscoreField;
        protected string _inheritedField = null!;
        public string InheritedFieldGetter => _inheritedField;
        public NestedClass? NullableClass { get; set; }
        public NestedClass NestedClass { get; set; } = null!;
        NestedClass INestedInterface.NestedInterfaceClass { get; set; } = null!;
        private string _unrelatedFieldName = null!;
        public string PropWithUnrelatedFieldName { get => _unrelatedFieldName; set => _unrelatedFieldName = value; }
        public string PropNoSetterWithUnrelatedFieldName => _unrelatedFieldName;

        private List<string> _stringListField = null!;
        public List<string> StringListProp { get => _stringListField; set => _stringListField = value; }

        private readonly List<string> _collectionDifferentType = null!;
        public IReadOnlyList<string> CollectionDifferentType => _collectionDifferentType.AsReadOnly();

        private readonly List<int> _collectionDifferentTypeInt = null!;
        public IReadOnlyList<int> CollectionDifferentTypeInt => _collectionDifferentTypeInt.AsReadOnly();

        private readonly HashSet<int> _hashSet = null!;
        public HashSet<int> HashSet => _hashSet;
        private readonly int[] _array = null!;
        public int[] Array => _array;
        private readonly ArrayList _arrayList = null!;
        public ArrayList ArrayList => _arrayList;
        private readonly IReadOnlyList<int> _readOnlyList = null!;
        public IReadOnlyList<int> IReadOnlyList => _readOnlyList;
        private readonly ImmutableList<int> _immutableList = null!;
        public ImmutableList<int> ImmutableList => _immutableList;
        private readonly ReadOnlyCollection<int> _readOnlyCollection = null!;
        public ReadOnlyCollection<int> ReadOnlyCollection => _readOnlyCollection;
    }

    internal class ClassWithOnlyNullable
    {
        public NestedClass? NestedNullableClass { get; set; }
    }

    internal class DerivedTestClass : TestClass
    {
        public override int Number { get; protected set; }
    }

    internal class TwiceDerivedClass : TestClass { }

    internal interface ITestInterface
    {
        string ImplicitProperty { get; set; }
        int ExplicitValueProperty { get; set; }
        string ExplicitRefProperty { get; set; }
    }

    internal class InterfaceTestClass : ITestInterface
    {
        public string ImplicitProperty { get; set; } = null!;
        int ITestInterface.ExplicitValueProperty { get; set; }
        string ITestInterface.ExplicitRefProperty { get; set; } = null!;
    }

    internal class NoDefaultConstructor
    {
        public string Value { get; set; }

        private NoDefaultConstructor(string value) => Value = value;
    }

    internal class DefaultConstructor
    {
        public string Value { get; set; }

        private DefaultConstructor() => Value = "Something";
    }

    internal class GenericClass<T> where T : class
    {
        public T Value { get; set; } = null!;
    }

    internal class NestedClass
    {
        public string Value { get; set; } = null!;
        public DeeperNestedClass DeeperNestedClass { get; set; } = null!;

        public NestedClass() { }
    }

    internal class DeeperNestedClass
    {
        public int Value { get; set; }

        private readonly string _privateField = null!;
        public string Text => _privateField;

        public DeeperNestedClass(int value) { Value = value; }
    }

    internal interface INestedInterface
    {
        NestedClass NestedInterfaceClass { get; set; }
    }

    internal interface INotImplementedInterface;
}
