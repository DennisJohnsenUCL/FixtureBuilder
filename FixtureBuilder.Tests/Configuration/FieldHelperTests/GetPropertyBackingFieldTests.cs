using System.Reflection;
using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.FieldHelperTests
{
    internal sealed class GetPropertyBackingFieldTests
    {
        private class NamedFieldAndAutoPropertyClass
        {
            private readonly int _value;
            public int Value { get; set; }
        }

        #region Field names

        [Test]
        public void FieldName_FindsNamedField()
        {
            var property = typeof(NamedFieldAndAutoPropertyClass).GetProperty(nameof(NamedFieldAndAutoPropertyClass.Value))!;

            var result = FieldHelper.GetPropertyBackingField(typeof(NamedFieldAndAutoPropertyClass), property, "_value");

            Assert.That(result?.Name, Is.EqualTo("_value"));
        }

        [Test]
        public void NoFieldName_AutoProperty_FindsAutoPropertyBackingField()
        {
            var property = typeof(PropertyHolder<string>).GetProperty(nameof(PropertyHolder<>.Value))!;

            var result = FieldHelper.GetPropertyBackingField(typeof(PropertyHolder<string>), property, null);

            Assert.That(result?.Name, Is.EqualTo("<Value>k__BackingField"));
        }

        [Test]
        public void UnknownFieldName_ReturnsNull()
        {
            var property = typeof(NamedFieldAndAutoPropertyClass).GetProperty(nameof(NamedFieldAndAutoPropertyClass.Value))!;

            var result = FieldHelper.GetPropertyBackingField(typeof(NamedFieldAndAutoPropertyClass), property, "_unknown");

            Assert.That(result, Is.Null);
        }

        private class InternalPropertyClass
        {
            internal string Name { get; set; } = null!;
        }

        [Test]
        public void NoFieldName_InternalAutoProperty_FindsAutoPropertyBackingField()
        {
            var property = typeof(InternalPropertyClass).GetProperty(nameof(InternalPropertyClass.Name), BindingFlags.Instance | BindingFlags.NonPublic)!;

            var result = FieldHelper.GetPropertyBackingField(typeof(InternalPropertyClass), property, null);

            Assert.That(result?.Name, Is.EqualTo("<Name>k__BackingField"));
        }

        #endregion

        #region Inheritance

        private class NameClass
        {
            public string Name { get; set; } = null!;
        }

        private class VirtualNumberClass
        {
            public virtual int Number { get; set; }
        }

        private class DerivedFromNamedFieldClass : NamedFieldAndAutoPropertyClass;

        [Test]
        public void FieldNameOfFieldDeclaredOnBaseType_FindsBaseTypeField()
        {
            var property = typeof(NamedFieldAndAutoPropertyClass).GetProperty(nameof(NamedFieldAndAutoPropertyClass.Value))!;

            var result = FieldHelper.GetPropertyBackingField(typeof(DerivedFromNamedFieldClass), property, "_value");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result?.Name, Is.EqualTo("_value"));
                Assert.That(result?.DeclaringType, Is.EqualTo(typeof(NamedFieldAndAutoPropertyClass)));
            }
        }

        private class OverridingNumberClass : VirtualNumberClass
        {
            public override int Number { get; set; }
        }

        private class DerivedFromOverridingNumberClass : OverridingNumberClass;

        // Expression trees reference the original virtual declaration, so the base property is what arrives here.
        [TestCase(typeof(OverridingNumberClass))]
        [TestCase(typeof(DerivedFromOverridingNumberClass))]
        public void OverriddenAutoProperty_FindsOverridingField(Type parentType)
        {
            var property = typeof(VirtualNumberClass).GetProperty(nameof(VirtualNumberClass.Number))!;

            var result = FieldHelper.GetPropertyBackingField(parentType, property, null);

            Assert.That(result?.DeclaringType, Is.EqualTo(typeof(OverridingNumberClass)));
        }

        private class GetterOnlyOverrideClass : VirtualNumberClass
        {
            public override int Number => base.Number;
        }

        [Test]
        public void GetterOnlyOverride_FindsBaseField()
        {
            var property = typeof(VirtualNumberClass).GetProperty(nameof(VirtualNumberClass.Number))!;

            var result = FieldHelper.GetPropertyBackingField(typeof(GetterOnlyOverrideClass), property, null);

            Assert.That(result?.DeclaringType, Is.EqualTo(typeof(VirtualNumberClass)));
        }

        private class PrivateFieldBaseClass
        {
            private readonly int _number = 1;
            protected int ReadNumber() => _number;
        }

        private class ReadsPrivateBaseFieldClass : PrivateFieldBaseClass
        {
            public int Number => ReadNumber();
        }

        // Type.GetField does not return private fields of base types, so these are only found by searching the base types.
        [Test]
        public void PrivateBaseFieldReadThroughBaseMethod_FindsFieldReadByGetter()
        {
            var property = typeof(ReadsPrivateBaseFieldClass).GetProperty(nameof(ReadsPrivateBaseFieldClass.Number))!;
            var instance = new ReadsPrivateBaseFieldClass();

            var result = FieldHelper.GetPropertyBackingField(typeof(ReadsPrivateBaseFieldClass), property, null);

            result!.SetValue(instance, 42);
            var number = property.GetValue(instance);
            Assert.That(number, Is.EqualTo(42));
        }

        private class UnconventionalPrivateFieldBaseClass
        {
            private readonly int _unconventionalName = 1;
            protected int ReadNumber() => _unconventionalName;
        }

        private class ReadsUnconventionalPrivateBaseFieldClass : UnconventionalPrivateFieldBaseClass
        {
            public int Number => ReadNumber();
        }

        [Test]
        public void FieldNameOfPrivateBaseField_FindsFieldReadByGetter()
        {
            var property = typeof(ReadsUnconventionalPrivateBaseFieldClass).GetProperty(nameof(ReadsUnconventionalPrivateBaseFieldClass.Number))!;
            var instance = new ReadsUnconventionalPrivateBaseFieldClass();

            var result = FieldHelper.GetPropertyBackingField(typeof(ReadsUnconventionalPrivateBaseFieldClass), property, "_unconventionalName");

            result!.SetValue(instance, 42);
            var number = property.GetValue(instance);
            Assert.That(number, Is.EqualTo(42));
        }

        private class HidingNameClass : NameClass
        {
            public new string Name { get; set; } = null!;
        }

        [Test]
        public void PropertyHiddenWithNew_FindsHiddenPropertyField()
        {
            var property = typeof(NameClass).GetProperty(nameof(NameClass.Name))!;

            var result = FieldHelper.GetPropertyBackingField(typeof(HidingNameClass), property, null);

            Assert.That(result?.DeclaringType, Is.EqualTo(typeof(NameClass)));
        }

        private class ConventionalFieldInDerivedClass : NameClass
        {
            private readonly string _name = null!;
            public string Nickname => _name;
        }

        [Test]
        public void ConventionallyNamedFieldInDerivedType_FindsDeclaringTypeField()
        {
            var property = typeof(NameClass).GetProperty(nameof(NameClass.Name))!;

            var result = FieldHelper.GetPropertyBackingField(typeof(ConventionalFieldInDerivedClass), property, null);

            Assert.That(result?.DeclaringType, Is.EqualTo(typeof(NameClass)));
        }

        #endregion

        #region Interfaces

        private interface IIdInterface
        {
            int Id { get; set; }
        }

        private interface INameInterface
        {
            string Name { get; }
        }

        private class ImplicitIdClass : IIdInterface
        {
            public int Id { get; set; }
        }

        [Test]
        public void InterfaceProperty_FindsImplementationAutoPropertyBackingField()
        {
            var property = typeof(IIdInterface).GetProperty(nameof(IIdInterface.Id))!;

            var result = FieldHelper.GetPropertyBackingField(typeof(ImplicitIdClass), property, null);

            Assert.That(result?.Name, Is.EqualTo("<Id>k__BackingField"));
        }

        private class ExplicitNameClass : INameInterface
        {
            string INameInterface.Name { get; } = null!;
        }

        private class DerivedFromExplicitNameClass : ExplicitNameClass;

        private interface IValueInterface<T>
        {
            T Value { get; }
        }

        private class ExplicitGenericValueClass : IValueInterface<string>
        {
            string IValueInterface<string>.Value { get; } = null!;
        }

        [TestCase(typeof(ExplicitNameClass), typeof(INameInterface), "Name")]
        [TestCase(typeof(DerivedFromExplicitNameClass), typeof(INameInterface), "Name")]
        [TestCase(typeof(ExplicitGenericValueClass), typeof(IValueInterface<string>), "Value")]
        public void ExplicitInterfaceAutoProperty_FindsFieldReadByGetter(Type parentType, Type interfaceType, string propertyName)
        {
            var property = interfaceType.GetProperty(propertyName)!;
            var instance = Activator.CreateInstance(parentType)!;

            var result = FieldHelper.GetPropertyBackingField(parentType, property, null);

            result!.SetValue(instance, "value");
            var value = property.GetValue(instance);
            Assert.That(value, Is.EqualTo("value"));
        }

        private class ExplicitNameWithConventionalFieldClass : INameInterface
        {
            private readonly string _name = null!;
            string INameInterface.Name => _name;
        }

        [Test]
        public void ExplicitInterfacePropertyWithConventionalField_FindsConventionalField()
        {
            var property = typeof(INameInterface).GetProperty(nameof(INameInterface.Name))!;

            var result = FieldHelper.GetPropertyBackingField(typeof(ExplicitNameWithConventionalFieldClass), property, null);

            Assert.That(result?.Name, Is.EqualTo("_name"));
        }

        private class ExplicitNameWithUnconventionalFieldClass : INameInterface
        {
            private readonly string _unconventionalName = null!;
            string INameInterface.Name => _unconventionalName;
        }

        [Test]
        public void ExplicitInterfacePropertyAndFieldName_FindsNamedField()
        {
            var property = typeof(INameInterface).GetProperty(nameof(INameInterface.Name))!;

            var result = FieldHelper.GetPropertyBackingField(typeof(ExplicitNameWithUnconventionalFieldClass), property, "_unconventionalName");

            Assert.That(result?.Name, Is.EqualTo("_unconventionalName"));
        }

        private class VirtualIdClass : IIdInterface
        {
            public virtual int Id { get; set; }
        }

        private class OverridingIdClass : VirtualIdClass
        {
            public override int Id { get; set; }
        }

        [Test]
        public void InterfacePropertyWithOverriddenImplementation_FindsOverridingField()
        {
            var property = typeof(IIdInterface).GetProperty(nameof(IIdInterface.Id))!;

            var result = FieldHelper.GetPropertyBackingField(typeof(OverridingIdClass), property, null);

            Assert.That(result?.DeclaringType, Is.EqualTo(typeof(OverridingIdClass)));
        }

        private interface IDefaultNameInterface
        {
            string Name => "default";
        }

        private class DefaultNameClass : IDefaultNameInterface;

        [Test]
        public void DefaultInterfaceMember_ReturnsNull()
        {
            var property = typeof(IDefaultNameInterface).GetProperty(nameof(IDefaultNameInterface.Name))!;

            var result = FieldHelper.GetPropertyBackingField(typeof(DefaultNameClass), property, null);

            Assert.That(result, Is.Null);
        }

        #endregion
    }
}
