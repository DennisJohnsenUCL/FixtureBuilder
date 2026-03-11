using FixtureBuilder.Helpers;

namespace FixtureBuilder.Tests.Helpers.FieldHelperTests
{
    internal sealed class TryGetFieldTests
    {
        private class FieldTestTarget
        {
            public int PublicField;
            private readonly string _privateField = null!;
            protected double ProtectedField;
        }

        [Test]
        public void TryGetField_PublicInstanceField_ReturnsTrueAndFieldInfo()
        {
            var result = FieldHelper.TryGetField(typeof(FieldTestTarget), "PublicField", out var fieldInfo);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(fieldInfo, Is.Not.Null);
                Assert.That(fieldInfo.Name, Is.EqualTo("PublicField"));
                Assert.That(fieldInfo.FieldType, Is.EqualTo(typeof(int)));
            }
        }

        [Test]
        public void TryGetField_PrivateInstanceField_ReturnsTrueAndFieldInfo()
        {
            var result = FieldHelper.TryGetField(typeof(FieldTestTarget), "_privateField", out var fieldInfo);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(fieldInfo, Is.Not.Null);
                Assert.That(fieldInfo.Name, Is.EqualTo("_privateField"));
                Assert.That(fieldInfo.FieldType, Is.EqualTo(typeof(string)));
            }
        }

        [Test]
        public void TryGetField_ProtectedInstanceField_ReturnsTrueAndFieldInfo()
        {
            var result = FieldHelper.TryGetField(typeof(FieldTestTarget), "ProtectedField", out var fieldInfo);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(fieldInfo, Is.Not.Null);
                Assert.That(fieldInfo.Name, Is.EqualTo("ProtectedField"));
            }
        }

        [Test]
        public void TryGetField_NonExistentField_ReturnsFalseAndNull()
        {
            var result = FieldHelper.TryGetField(typeof(FieldTestTarget), "NoSuchField", out var fieldInfo);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(fieldInfo, Is.Null);
            }
        }
    }
}
