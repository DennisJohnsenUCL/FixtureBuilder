using System.Reflection;

namespace FixtureBuilder.Tests.Core;

internal sealed class DataMemberInfoTests
{
    // ── Helper types used by reflection ──────────────────────────────

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    private sealed class MarkerAttribute : Attribute;

    private class SampleClass
    {
        [Marker]
        public string TaggedProperty { get; set; } = "prop-value";

        [Marker]
        public string TaggedField = "field-value";

        public int IntProperty { get; set; } = 42;

        public int IntField = 99;

        public static string StaticProperty { get; set; } = "static-prop";

        public static string StaticField = "static-field";

        public string InstanceProperty { get; set; } = "instance-prop";

        public string InstanceField = "instance-field";
    }

    // ── Factory helpers ──────────────────────────────────────────────

    private static DataMemberInfo FromProperty(string name) =>
        new(typeof(SampleClass).GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)!);

    private static DataMemberInfo FromField(string name) =>
        new(typeof(SampleClass).GetField(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)!);

    // ═════════════════════════════════════════════════════════════════
    //  Constructor / IsPropertyInfo / IsFieldInfo
    // ═════════════════════════════════════════════════════════════════

    [Test]
    public void Constructor_WithPropertyInfo_SetsIsPropertyInfoTrue()
    {
        var sut = FromProperty(nameof(SampleClass.TaggedProperty));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.IsPropertyInfo, Is.True);
            Assert.That(sut.IsFieldInfo, Is.False);
        }
    }

    [Test]
    public void Constructor_WithFieldInfo_SetsIsFieldInfoTrue()
    {
        var sut = FromField(nameof(SampleClass.TaggedField));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.IsFieldInfo, Is.True);
            Assert.That(sut.IsPropertyInfo, Is.False);
        }
    }

    // ═════════════════════════════════════════════════════════════════
    //  Property / Field accessor guards
    // ═════════════════════════════════════════════════════════════════

    [Test]
    public void Property_WhenBackedByProperty_ReturnsPropertyInfo()
    {
        var sut = FromProperty(nameof(SampleClass.TaggedProperty));

        Assert.That(sut.Property, Is.Not.Null);
        Assert.That(sut.Property.Name, Is.EqualTo(nameof(SampleClass.TaggedProperty)));
    }

    [Test]
    public void Property_WhenBackedByField_ThrowsInvalidOperationException()
    {
        var sut = FromField(nameof(SampleClass.TaggedField));

        Assert.That(() => sut.Property, Throws.InvalidOperationException);
    }

    [Test]
    public void Field_WhenBackedByField_ReturnsFieldInfo()
    {
        var sut = FromField(nameof(SampleClass.TaggedField));

        Assert.That(sut.Field, Is.Not.Null);
        Assert.That(sut.Field.Name, Is.EqualTo(nameof(SampleClass.TaggedField)));
    }

    [Test]
    public void Field_WhenBackedByProperty_ThrowsInvalidOperationException()
    {
        var sut = FromProperty(nameof(SampleClass.TaggedProperty));

        Assert.That(() => sut.Field, Throws.InvalidOperationException);
    }

    // ═════════════════════════════════════════════════════════════════
    //  DataMemberType
    // ═════════════════════════════════════════════════════════════════

    [Test]
    public void DataMemberType_WhenBackedByProperty_ReturnsPropertyType()
    {
        var sut = FromProperty(nameof(SampleClass.IntProperty));

        Assert.That(sut.DataMemberType, Is.EqualTo(typeof(int)));
    }

    [Test]
    public void DataMemberType_WhenBackedByField_ReturnsFieldType()
    {
        var sut = FromField(nameof(SampleClass.IntField));

        Assert.That(sut.DataMemberType, Is.EqualTo(typeof(int)));
    }

    // ═════════════════════════════════════════════════════════════════
    //  DeclaringType
    // ═════════════════════════════════════════════════════════════════

    [Test]
    public void DeclaringType_WhenBackedByProperty_ReturnsSampleClass()
    {
        var sut = FromProperty(nameof(SampleClass.TaggedProperty));

        Assert.That(sut.DeclaringType, Is.EqualTo(typeof(SampleClass)));
    }

    [Test]
    public void DeclaringType_WhenBackedByField_ReturnsSampleClass()
    {
        var sut = FromField(nameof(SampleClass.TaggedField));

        Assert.That(sut.DeclaringType, Is.EqualTo(typeof(SampleClass)));
    }

    // ═════════════════════════════════════════════════════════════════
    //  GetValue / SetValue
    // ═════════════════════════════════════════════════════════════════

    [Test]
    public void GetValue_WhenBackedByProperty_ReadsPropertyValue()
    {
        var obj = new SampleClass { IntProperty = 7 };
        var sut = FromProperty(nameof(SampleClass.IntProperty));

        Assert.That(sut.GetValue(obj), Is.EqualTo(7));
    }

    [Test]
    public void GetValue_WhenBackedByField_ReadsFieldValue()
    {
        var obj = new SampleClass { IntField = 13 };
        var sut = FromField(nameof(SampleClass.IntField));

        Assert.That(sut.GetValue(obj), Is.EqualTo(13));
    }

    [Test]
    public void SetValue_WhenBackedByProperty_WritesPropertyValue()
    {
        var obj = new SampleClass();
        var sut = FromProperty(nameof(SampleClass.IntProperty));

        sut.SetValue(obj, 123);

        Assert.That(obj.IntProperty, Is.EqualTo(123));
    }

    [Test]
    public void SetValue_WhenBackedByField_WritesFieldValue()
    {
        var obj = new SampleClass();
        var sut = FromField(nameof(SampleClass.IntField));

        sut.SetValue(obj, 456);

        Assert.That(obj.IntField, Is.EqualTo(456));
    }

    // ═════════════════════════════════════════════════════════════════
    //  Name
    // ═════════════════════════════════════════════════════════════════

    [Test]
    public void Name_WhenBackedByProperty_ReturnsPropertyName()
    {
        var sut = FromProperty(nameof(SampleClass.TaggedProperty));

        Assert.That(sut.Name, Is.EqualTo(nameof(SampleClass.TaggedProperty)));
    }

    [Test]
    public void Name_WhenBackedByField_ReturnsFieldName()
    {
        var sut = FromField(nameof(SampleClass.TaggedField));

        Assert.That(sut.Name, Is.EqualTo(nameof(SampleClass.TaggedField)));
    }

    // ═════════════════════════════════════════════════════════════════
    //  MemberType
    // ═════════════════════════════════════════════════════════════════

    [Test]
    public void MemberType_WhenBackedByProperty_ReturnsPropertyMemberType()
    {
        var sut = FromProperty(nameof(SampleClass.TaggedProperty));

        Assert.That(sut.MemberType, Is.EqualTo(MemberTypes.Property));
    }

    [Test]
    public void MemberType_WhenBackedByField_ReturnsFieldMemberType()
    {
        var sut = FromField(nameof(SampleClass.TaggedField));

        Assert.That(sut.MemberType, Is.EqualTo(MemberTypes.Field));
    }

    // ═════════════════════════════════════════════════════════════════
    //  ReflectedType
    // ═════════════════════════════════════════════════════════════════

    [Test]
    public void ReflectedType_WhenBackedByProperty_ReturnsSampleClass()
    {
        var sut = FromProperty(nameof(SampleClass.TaggedProperty));

        Assert.That(sut.ReflectedType, Is.EqualTo(typeof(SampleClass)));
    }

    [Test]
    public void ReflectedType_WhenBackedByField_ReturnsSampleClass()
    {
        var sut = FromField(nameof(SampleClass.TaggedField));

        Assert.That(sut.ReflectedType, Is.EqualTo(typeof(SampleClass)));
    }

    // ═════════════════════════════════════════════════════════════════
    //  Module
    // ═════════════════════════════════════════════════════════════════

    [Test]
    public void Module_WhenBackedByProperty_ReturnsModule()
    {
        var sut = FromProperty(nameof(SampleClass.TaggedProperty));

        Assert.That(sut.Module, Is.EqualTo(typeof(SampleClass).Module));
    }

    [Test]
    public void Module_WhenBackedByField_ReturnsModule()
    {
        var sut = FromField(nameof(SampleClass.TaggedField));

        Assert.That(sut.Module, Is.EqualTo(typeof(SampleClass).Module));
    }

    // ═════════════════════════════════════════════════════════════════
    //  IsStatic
    // ═════════════════════════════════════════════════════════════════

    [Test]
    public void IsStatic_WhenBackedByStaticProperty_ReturnsTrue()
    {
        var sut = FromProperty(nameof(SampleClass.StaticProperty));

        Assert.That(sut.IsStatic, Is.True);
    }

    [Test]
    public void IsStatic_WhenBackedByInstanceProperty_ReturnsFalse()
    {
        var sut = FromProperty(nameof(SampleClass.InstanceProperty));

        Assert.That(sut.IsStatic, Is.False);
    }

    [Test]
    public void IsStatic_WhenBackedByStaticField_ReturnsTrue()
    {
        var sut = FromField(nameof(SampleClass.StaticField));

        Assert.That(sut.IsStatic, Is.True);
    }

    [Test]
    public void IsStatic_WhenBackedByInstanceField_ReturnsFalse()
    {
        var sut = FromField(nameof(SampleClass.InstanceField));

        Assert.That(sut.IsStatic, Is.False);
    }

    // ═════════════════════════════════════════════════════════════════
    //  CustomAttributes
    // ═════════════════════════════════════════════════════════════════

    [Test]
    public void CustomAttributes_WhenBackedByProperty_ReturnsPropertyAttributes()
    {
        var sut = FromProperty(nameof(SampleClass.TaggedProperty));

        Assert.That(sut.CustomAttributes,
            Has.Some.Matches<CustomAttributeData>(a => a.AttributeType == typeof(MarkerAttribute)));
    }

    [Test]
    public void CustomAttributes_WhenBackedByField_ReturnsFieldAttributes()
    {
        var sut = FromField(nameof(SampleClass.TaggedField));

        Assert.That(sut.CustomAttributes,
            Has.Some.Matches<CustomAttributeData>(a => a.AttributeType == typeof(MarkerAttribute)));
    }

    // ═════════════════════════════════════════════════════════════════
    //  IsDefined
    // ═════════════════════════════════════════════════════════════════

    [Test]
    public void IsDefined_WhenBackedByProperty_WithPresentAttribute_ReturnsTrue()
    {
        var sut = FromProperty(nameof(SampleClass.TaggedProperty));

        Assert.That(sut.IsDefined(typeof(MarkerAttribute), false), Is.True);
    }

    [Test]
    public void IsDefined_WhenBackedByProperty_WithAbsentAttribute_ReturnsFalse()
    {
        var sut = FromProperty(nameof(SampleClass.IntProperty));

        Assert.That(sut.IsDefined(typeof(MarkerAttribute), false), Is.False);
    }

    [Test]
    public void IsDefined_WhenBackedByField_WithPresentAttribute_ReturnsTrue()
    {
        var sut = FromField(nameof(SampleClass.TaggedField));

        Assert.That(sut.IsDefined(typeof(MarkerAttribute), false), Is.True);
    }

    [Test]
    public void IsDefined_WhenBackedByField_WithAbsentAttribute_ReturnsFalse()
    {
        var sut = FromField(nameof(SampleClass.IntField));

        Assert.That(sut.IsDefined(typeof(MarkerAttribute), false), Is.False);
    }

    // ═════════════════════════════════════════════════════════════════
    //  GetCustomAttributes(bool)
    // ═════════════════════════════════════════════════════════════════

    [Test]
    public void GetCustomAttributes_Bool_WhenBackedByProperty_ReturnsAttributes()
    {
        var sut = FromProperty(nameof(SampleClass.TaggedProperty));

        Assert.That(sut.GetCustomAttributes(false), Has.Some.InstanceOf<MarkerAttribute>());
    }

    [Test]
    public void GetCustomAttributes_Bool_WhenBackedByField_ReturnsAttributes()
    {
        var sut = FromField(nameof(SampleClass.TaggedField));

        Assert.That(sut.GetCustomAttributes(false), Has.Some.InstanceOf<MarkerAttribute>());
    }

    // ═════════════════════════════════════════════════════════════════
    //  GetCustomAttributes(Type, bool)
    // ═════════════════════════════════════════════════════════════════

    [Test]
    public void GetCustomAttributes_TypeBool_WhenBackedByProperty_ReturnsMatchingAttributes()
    {
        var sut = FromProperty(nameof(SampleClass.TaggedProperty));

        Assert.That(sut.GetCustomAttributes(typeof(MarkerAttribute), false), Is.Not.Empty);
    }

    [Test]
    public void GetCustomAttributes_TypeBool_WhenBackedByProperty_WithAbsentType_ReturnsEmpty()
    {
        var sut = FromProperty(nameof(SampleClass.IntProperty));

        Assert.That(sut.GetCustomAttributes(typeof(MarkerAttribute), false), Is.Empty);
    }

    [Test]
    public void GetCustomAttributes_TypeBool_WhenBackedByField_ReturnsMatchingAttributes()
    {
        var sut = FromField(nameof(SampleClass.TaggedField));

        Assert.That(sut.GetCustomAttributes(typeof(MarkerAttribute), false), Is.Not.Empty);
    }

    [Test]
    public void GetCustomAttributes_TypeBool_WhenBackedByField_WithAbsentType_ReturnsEmpty()
    {
        var sut = FromField(nameof(SampleClass.IntField));

        Assert.That(sut.GetCustomAttributes(typeof(MarkerAttribute), false), Is.Empty);
    }
}
