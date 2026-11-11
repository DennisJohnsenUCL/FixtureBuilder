using System.Reflection;

namespace FixtureBuilder.Tests.Core
{
    internal sealed class DataMemberInfoTests
    {
        #region Member kind

        [Test]
        public void IsPropertyInfo_PropertyMember_ReturnsTrue()
        {
            var member = TestHelper.PropertyMember(typeof(PropertyHolder<int>), "Value");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(member.IsPropertyInfo, Is.True);
                Assert.That(member.IsFieldInfo, Is.False);
            }
        }

        [Test]
        public void IsFieldInfo_FieldMember_ReturnsTrue()
        {
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(member.IsFieldInfo, Is.True);
                Assert.That(member.IsPropertyInfo, Is.False);
            }
        }

        [Test]
        public void Property_PropertyMember_ReturnsPropertyInfo()
        {
            var member = TestHelper.PropertyMember(typeof(PropertyHolder<int>), "Value");

            var result = member.Property;

            Assert.That(result, Is.EqualTo(typeof(PropertyHolder<int>).GetProperty("Value")));
        }

        [Test]
        public void Property_FieldMember_ThrowsInvalidOperationException()
        {
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");

            Assert.Throws<InvalidOperationException>(() => _ = member.Property);
        }

        [Test]
        public void Field_FieldMember_ReturnsFieldInfo()
        {
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");

            var result = member.Field;

            Assert.That(result, Is.EqualTo(typeof(FieldHolder<int>).GetField("Value")));
        }

        [Test]
        public void Field_PropertyMember_ThrowsInvalidOperationException()
        {
            var member = TestHelper.PropertyMember(typeof(PropertyHolder<int>), "Value");

            Assert.Throws<InvalidOperationException>(() => _ = member.Field);
        }

        #endregion

        #region Member information

        [Test]
        public void Name_PropertyMember_ReturnsPropertyName()
        {
            var member = TestHelper.PropertyMember(typeof(PropertyHolder<int>), "Value");

            Assert.That(member.Name, Is.EqualTo("Value"));
        }

        [Test]
        public void Name_FieldMember_ReturnsFieldName()
        {
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");

            Assert.That(member.Name, Is.EqualTo("Value"));
        }

        [Test]
        public void DataMemberType_PropertyMember_ReturnsPropertyType()
        {
            var member = TestHelper.PropertyMember(typeof(PropertyHolder<int>), "Value");

            Assert.That(member.DataMemberType, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void DataMemberType_FieldMember_ReturnsFieldType()
        {
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");

            Assert.That(member.DataMemberType, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void DeclaringType_PropertyMember_ReturnsDeclaringType()
        {
            var member = TestHelper.PropertyMember(typeof(PropertyHolder<int>), "Value");

            Assert.That(member.DeclaringType, Is.EqualTo(typeof(PropertyHolder<int>)));
        }

        [Test]
        public void DeclaringType_FieldMember_ReturnsDeclaringType()
        {
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");

            Assert.That(member.DeclaringType, Is.EqualTo(typeof(FieldHolder<int>)));
        }

        [Test]
        public void ReflectedType_PropertyMember_ReturnsReflectedType()
        {
            var member = TestHelper.PropertyMember(typeof(PropertyHolder<int>), "Value");

            Assert.That(member.ReflectedType, Is.EqualTo(typeof(PropertyHolder<int>)));
        }

        [Test]
        public void ReflectedType_FieldMember_ReturnsReflectedType()
        {
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");

            Assert.That(member.ReflectedType, Is.EqualTo(typeof(FieldHolder<int>)));
        }

        [Test]
        public void MemberType_PropertyMember_ReturnsProperty()
        {
            var member = TestHelper.PropertyMember(typeof(PropertyHolder<int>), "Value");

            Assert.That(member.MemberType, Is.EqualTo(MemberTypes.Property));
        }

        [Test]
        public void MemberType_FieldMember_ReturnsField()
        {
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");

            Assert.That(member.MemberType, Is.EqualTo(MemberTypes.Field));
        }

        [Test]
        public void Module_PropertyMember_ReturnsDeclaringModule()
        {
            var member = TestHelper.PropertyMember(typeof(PropertyHolder<int>), "Value");

            Assert.That(member.Module, Is.EqualTo(typeof(PropertyHolder<int>).Module));
        }

        [Test]
        public void Module_FieldMember_ReturnsDeclaringModule()
        {
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");

            Assert.That(member.Module, Is.EqualTo(typeof(FieldHolder<int>).Module));
        }

        #endregion

        #region Values

        [Test]
        public void GetValue_PropertyMember_ReturnsPropertyValue()
        {
            var member = TestHelper.PropertyMember(typeof(PropertyHolder<int>), "Value");
            var instance = new PropertyHolder<int> { Value = 7 };

            var result = member.GetValue(instance);

            Assert.That(result, Is.EqualTo(7));
        }

        [Test]
        public void GetValue_FieldMember_ReturnsFieldValue()
        {
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");
            var instance = new FieldHolder<int> { Value = 13 };

            var result = member.GetValue(instance);

            Assert.That(result, Is.EqualTo(13));
        }

        [Test]
        public void SetValue_PropertyMember_SetsProperty()
        {
            var member = TestHelper.PropertyMember(typeof(PropertyHolder<int>), "Value");
            var instance = new PropertyHolder<int>();

            member.SetValue(instance, 123);

            Assert.That(instance.Value, Is.EqualTo(123));
        }

        [Test]
        public void SetValue_FieldMember_SetsField()
        {
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");
            var instance = new FieldHolder<int>();

            member.SetValue(instance, 456);

            Assert.That(instance.Value, Is.EqualTo(456));
        }

        #endregion

        #region IsStatic

        [Test]
        public void IsStatic_StaticProperty_ReturnsTrue()
        {
            var member = TestHelper.PropertyMember(typeof(StaticMembersClass), nameof(StaticMembersClass.StaticProperty));

            Assert.That(member.IsStatic, Is.True);
        }

        [Test]
        public void IsStatic_InstanceProperty_ReturnsFalse()
        {
            var member = TestHelper.PropertyMember(typeof(PropertyHolder<int>), "Value");

            Assert.That(member.IsStatic, Is.False);
        }

        [Test]
        public void IsStatic_StaticField_ReturnsTrue()
        {
            var member = TestHelper.FieldMember(typeof(StaticMembersClass), nameof(StaticMembersClass.StaticField));

            Assert.That(member.IsStatic, Is.True);
        }

        [Test]
        public void IsStatic_InstanceField_ReturnsFalse()
        {
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");

            Assert.That(member.IsStatic, Is.False);
        }

        #endregion

        #region Attributes

        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
        private sealed class MarkerAttribute : Attribute;

        private class MarkedMembersClass
        {
            [Marker]
            public string MarkedProperty { get; set; } = null!;

            [Marker]
            public string MarkedField = null!;
        }

        [Test]
        public void CustomAttributes_MarkedProperty_ContainsMarker()
        {
            var member = TestHelper.PropertyMember(typeof(MarkedMembersClass), nameof(MarkedMembersClass.MarkedProperty));

            var attributeTypes = member.CustomAttributes.Select(attribute => attribute.AttributeType);

            Assert.That(attributeTypes, Does.Contain(typeof(MarkerAttribute)));
        }

        [Test]
        public void CustomAttributes_MarkedField_ContainsMarker()
        {
            var member = TestHelper.FieldMember(typeof(MarkedMembersClass), nameof(MarkedMembersClass.MarkedField));

            var attributeTypes = member.CustomAttributes.Select(attribute => attribute.AttributeType);

            Assert.That(attributeTypes, Does.Contain(typeof(MarkerAttribute)));
        }

        [Test]
        public void IsDefined_MarkedProperty_ReturnsTrue()
        {
            var member = TestHelper.PropertyMember(typeof(MarkedMembersClass), nameof(MarkedMembersClass.MarkedProperty));

            var result = member.IsDefined(typeof(MarkerAttribute), false);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsDefined_UnmarkedProperty_ReturnsFalse()
        {
            var member = TestHelper.PropertyMember(typeof(PropertyHolder<int>), "Value");

            var result = member.IsDefined(typeof(MarkerAttribute), false);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsDefined_MarkedField_ReturnsTrue()
        {
            var member = TestHelper.FieldMember(typeof(MarkedMembersClass), nameof(MarkedMembersClass.MarkedField));

            var result = member.IsDefined(typeof(MarkerAttribute), false);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsDefined_UnmarkedField_ReturnsFalse()
        {
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");

            var result = member.IsDefined(typeof(MarkerAttribute), false);

            Assert.That(result, Is.False);
        }

        [Test]
        public void GetCustomAttributes_MarkedProperty_ReturnsMarker()
        {
            var member = TestHelper.PropertyMember(typeof(MarkedMembersClass), nameof(MarkedMembersClass.MarkedProperty));

            var result = member.GetCustomAttributes(false);

            Assert.That(result, Has.One.InstanceOf<MarkerAttribute>());
        }

        [Test]
        public void GetCustomAttributes_MarkedField_ReturnsMarker()
        {
            var member = TestHelper.FieldMember(typeof(MarkedMembersClass), nameof(MarkedMembersClass.MarkedField));

            var result = member.GetCustomAttributes(false);

            Assert.That(result, Has.One.InstanceOf<MarkerAttribute>());
        }

        [Test]
        public void GetCustomAttributesOfType_MarkedProperty_ReturnsMarker()
        {
            var member = TestHelper.PropertyMember(typeof(MarkedMembersClass), nameof(MarkedMembersClass.MarkedProperty));

            var result = member.GetCustomAttributes(typeof(MarkerAttribute), false);

            Assert.That(result, Has.One.InstanceOf<MarkerAttribute>());
        }

        [Test]
        public void GetCustomAttributesOfType_UnmarkedProperty_ReturnsEmpty()
        {
            var member = TestHelper.PropertyMember(typeof(PropertyHolder<int>), "Value");

            var result = member.GetCustomAttributes(typeof(MarkerAttribute), false);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetCustomAttributesOfType_MarkedField_ReturnsMarker()
        {
            var member = TestHelper.FieldMember(typeof(MarkedMembersClass), nameof(MarkedMembersClass.MarkedField));

            var result = member.GetCustomAttributes(typeof(MarkerAttribute), false);

            Assert.That(result, Has.One.InstanceOf<MarkerAttribute>());
        }

        [Test]
        public void GetCustomAttributesOfType_UnmarkedField_ReturnsEmpty()
        {
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");

            var result = member.GetCustomAttributes(typeof(MarkerAttribute), false);

            Assert.That(result, Is.Empty);
        }

        #endregion
    }
}
