using FixtureBuilder.Helpers;

namespace FixtureBuilder.Tests.Helpers.FieldHelperTests
{
    internal sealed class TryGetField
    {
        private class FieldTestTarget
        {
            public int PublicField;
            private readonly string _privateField = null!;
        }

        [Test]
        public void TryGetField_FirstNameMatches_ReturnsTrueWithFirstMatch()
        {
            var result = FieldHelper.TryGetField(typeof(FieldTestTarget), ["PublicField", "_privateField"], out var fieldInfo);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(fieldInfo.Name, Is.EqualTo("PublicField"));
            }
        }

        [Test]
        public void TryGetField_SecondNameMatches_ReturnsTrueWithSecondMatch()
        {
            var result = FieldHelper.TryGetField(typeof(FieldTestTarget), ["NoSuchField", "_privateField"], out var fieldInfo);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(fieldInfo.Name, Is.EqualTo("_privateField"));
            }
        }

        [Test]
        public void TryGetField_NoNamesMatch_ReturnsFalseAndNull()
        {
            var result = FieldHelper.TryGetField(typeof(FieldTestTarget), ["Fake", "AlsoFake"], out var fieldInfo);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(fieldInfo, Is.Null);
            }
        }

        [Test]
        public void TryGetField_EmptyArray_ReturnsFalseAndNull()
        {
            var result = FieldHelper.TryGetField(typeof(FieldTestTarget), [], out var fieldInfo);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(fieldInfo, Is.Null);
            }
        }
    }
}
