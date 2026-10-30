#pragma warning disable CS0169

using System.Reflection;
using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.FieldHelperTests
{
    internal sealed class GetPropertyBackingFieldTests
    {
        private class SimpleTarget
        {
            private readonly int _value;
            public int Value { get; set; }
        }

        private class AutoPropTarget
        {
            public string Name { get; set; } = null!;
        }

        private interface IWithProp
        {
            int Id { get; set; }
        }

        private class ImplementsInterface : IWithProp
        {
            public int Id { get; set; }
        }

        private class DerivedTarget : SimpleTarget { }

        private class VirtualBase
        {
            public virtual int Number { get; set; }
        }

        private class AutoOverride : VirtualBase
        {
            public override int Number { get; set; }
        }

        private class AutoOverrideChild : AutoOverride { }

        private class GetterOnlyOverride : VirtualBase
        {
            public override int Number => base.Number;
        }

        private class HidingDerived : AutoPropTarget
        {
            public new string Name { get; set; } = null!;
        }

        private class ConventionalFieldDerived : AutoPropTarget
        {
            private readonly string _name = null!;
            public string Nickname => _name;
        }

        private class InternalPropTarget
        {
            internal string Name { get; set; } = null!;
        }

        private interface INamed
        {
            string Name { get; }
        }

        private interface IHasValue<T>
        {
            T Value { get; }
        }

        private class ExplicitNamed : INamed
        {
            string INamed.Name { get; } = null!;
        }

        private class ExplicitNamedChild : ExplicitNamed { }

        private class ExplicitHasValue : IHasValue<string>
        {
            string IHasValue<string>.Value { get; } = null!;
        }

        private class ExplicitHandWrittenNamed : INamed
        {
            private readonly string _name = null!;
            string INamed.Name => _name;
        }

        private class ExplicitCustomNamed : INamed
        {
            private readonly string _custom = null!;
            string INamed.Name => _custom;
        }

        private class VirtualImplementation : IWithProp
        {
            public virtual int Id { get; set; }
        }

        private class OverridingImplementation : VirtualImplementation
        {
            public override int Id { get; set; }
        }

        private interface IWithDefault
        {
            string Name => "default";
        }

        private class UsesDefault : IWithDefault { }

        [Test]
        public void GetPropertyBackingField_ExplicitFieldName_FindsField()
        {
            var property = typeof(SimpleTarget).GetProperty("Value")!;

            var result = FieldHelper.GetPropertyBackingField(typeof(SimpleTarget), property, "_value");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Name, Is.EqualTo("_value"));
            }
        }

        [Test]
        public void GetPropertyBackingField_NullFieldName_FindsAutoPropertyBackingField()
        {
            var property = typeof(AutoPropTarget).GetProperty("Name")!;

            var result = FieldHelper.GetPropertyBackingField(typeof(AutoPropTarget), property, null);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetPropertyBackingField_FieldOnDeclaringType_FallsBackToDeclaringType()
        {
            var property = typeof(SimpleTarget).GetProperty("Value")!;

            var result = FieldHelper.GetPropertyBackingField(typeof(DerivedTarget), property, "_value");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Name, Is.EqualTo("_value"));
                Assert.That(result?.DeclaringType, Is.EqualTo(typeof(SimpleTarget)));
            }
        }

        [Test]
        public void GetPropertyBackingField_InterfaceProperty_NullFieldName_FindsBackingField()
        {
            var interfaceProp = typeof(IWithProp).GetProperty("Id")!;

            var result = FieldHelper.GetPropertyBackingField(typeof(ImplementsInterface), interfaceProp, null);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetPropertyBackingField_ExplicitFieldName_NotFound_ReturnsNull()
        {
            var property = typeof(SimpleTarget).GetProperty("Value")!;

            var result = FieldHelper.GetPropertyBackingField(typeof(SimpleTarget), property, "nonexistent");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetPropertyBackingField_NullFieldName_NoMatchAnywhere_ReturnsNull()
        {
            var property = typeof(IWithProp).GetProperty("Id")!;

            var result = FieldHelper.GetPropertyBackingField(typeof(SimpleTarget), property, null);

            Assert.That(result, Is.Null);
        }

        // Expression trees reference the original virtual declaration, so the base property is what arrives here.
        [TestCase(typeof(AutoOverride))]
        [TestCase(typeof(AutoOverrideChild))]
        public void GetPropertyBackingField_OverriddenAutoProperty_FindsOverridingField(Type parentType)
        {
            var property = typeof(VirtualBase).GetProperty("Number")!;

            var result = FieldHelper.GetPropertyBackingField(parentType, property, null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.DeclaringType, Is.EqualTo(typeof(AutoOverride)));
            }
        }

        [Test]
        public void GetPropertyBackingField_GetterOnlyOverride_FindsBaseField()
        {
            var property = typeof(VirtualBase).GetProperty("Number")!;

            var result = FieldHelper.GetPropertyBackingField(typeof(GetterOnlyOverride), property, null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.DeclaringType, Is.EqualTo(typeof(VirtualBase)));
            }
        }

        [Test]
        public void GetPropertyBackingField_PropertyHiddenWithNew_FindsHiddenPropertyField()
        {
            var property = typeof(AutoPropTarget).GetProperty("Name")!;

            var result = FieldHelper.GetPropertyBackingField(typeof(HidingDerived), property, null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.DeclaringType, Is.EqualTo(typeof(AutoPropTarget)));
            }
        }

        [Test]
        public void GetPropertyBackingField_DerivedTypeHasConventionallyNamedField_FindsDeclaringTypeField()
        {
            var property = typeof(AutoPropTarget).GetProperty("Name")!;

            var result = FieldHelper.GetPropertyBackingField(typeof(ConventionalFieldDerived), property, null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.DeclaringType, Is.EqualTo(typeof(AutoPropTarget)));
            }
        }

        [Test]
        public void GetPropertyBackingField_InternalProperty_FindsBackingField()
        {
            var property = typeof(InternalPropTarget).GetProperty("Name", BindingFlags.Instance | BindingFlags.NonPublic)!;

            var result = FieldHelper.GetPropertyBackingField(typeof(InternalPropTarget), property, null);

            Assert.That(result, Is.Not.Null);
        }

        [TestCase(typeof(ExplicitNamed), typeof(INamed), "Name")]
        [TestCase(typeof(ExplicitNamedChild), typeof(INamed), "Name")]
        [TestCase(typeof(ExplicitHasValue), typeof(IHasValue<string>), "Value")]
        public void GetPropertyBackingField_ExplicitInterfaceAutoProperty_FindsFieldReadByGetter(Type parentType, Type interfaceType, string propertyName)
        {
            var interfaceProp = interfaceType.GetProperty(propertyName)!;
            var instance = Activator.CreateInstance(parentType)!;

            var result = FieldHelper.GetPropertyBackingField(parentType, interfaceProp, null);

            Assert.That(result, Is.Not.Null);
            result!.SetValue(instance, "value");
            Assert.That(interfaceProp.GetValue(instance), Is.EqualTo("value"));
        }

        [Test]
        public void GetPropertyBackingField_ExplicitInterfaceHandWrittenField_FindsConventionallyNamedField()
        {
            var interfaceProp = typeof(INamed).GetProperty("Name")!;

            var result = FieldHelper.GetPropertyBackingField(typeof(ExplicitHandWrittenNamed), interfaceProp, null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Name, Is.EqualTo("_name"));
            }
        }

        [Test]
        public void GetPropertyBackingField_InterfaceProperty_ExplicitFieldName_FindsField()
        {
            var interfaceProp = typeof(INamed).GetProperty("Name")!;

            var result = FieldHelper.GetPropertyBackingField(typeof(ExplicitCustomNamed), interfaceProp, "_custom");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Name, Is.EqualTo("_custom"));
            }
        }

        [Test]
        public void GetPropertyBackingField_InterfaceImplementationOverridden_FindsOverridingField()
        {
            var interfaceProp = typeof(IWithProp).GetProperty("Id")!;

            var result = FieldHelper.GetPropertyBackingField(typeof(OverridingImplementation), interfaceProp, null);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.DeclaringType, Is.EqualTo(typeof(OverridingImplementation)));
            }
        }

        [Test]
        public void GetPropertyBackingField_DefaultInterfaceMember_ReturnsNull()
        {
            var interfaceProp = typeof(IWithDefault).GetProperty("Name")!;

            var result = FieldHelper.GetPropertyBackingField(typeof(UsesDefault), interfaceProp, null);

            Assert.That(result, Is.Null);
        }
    }
}
