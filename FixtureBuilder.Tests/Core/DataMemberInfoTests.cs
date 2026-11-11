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
        public void Field_FieldMember_ReturnsFieldInfo()
        {
            var member = TestHelper.FieldMember(typeof(FieldHolder<int>), "Value");

            var result = member.Field;

            Assert.That(result, Is.EqualTo(typeof(FieldHolder<int>).GetField("Value")));
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
    }
}
