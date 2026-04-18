#pragma warning disable CS0649
#pragma warning disable CS0414

using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
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
            var result = typeof(FieldTestTarget).TryGetField("PublicField", out var fieldInfo);

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
            var result = typeof(FieldTestTarget).TryGetField("_privateField", out var fieldInfo);

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
            var result = typeof(FieldTestTarget).TryGetField("ProtectedField", out var fieldInfo);

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
            var result = typeof(FieldTestTarget).TryGetField("NoSuchField", out var fieldInfo);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(fieldInfo, Is.Null);
            }
        }
    }
}
