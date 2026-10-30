#pragma warning disable CS0649
#pragma warning disable CS0414

using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.FieldHelperTests
{
    internal sealed class GetField
    {
        private class FieldTestTarget
        {
            public int PublicField;
            private readonly string _privateField = null!;
        }

        [Test]
        public void GetField_FirstNameMatches_ReturnsFirstMatch()
        {
            var result = FieldHelper.GetField(typeof(FieldTestTarget), ["PublicField", "_privateField"]);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Name, Is.EqualTo("PublicField"));
            }
        }

        [Test]
        public void GetField_SecondNameMatches_ReturnsSecondMatch()
        {
            var result = FieldHelper.GetField(typeof(FieldTestTarget), ["NoSuchField", "_privateField"]);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result?.Name, Is.EqualTo("_privateField"));
            }
        }

        [Test]
        public void GetField_NoNamesMatch_ReturnsNull()
        {
            var result = FieldHelper.GetField(typeof(FieldTestTarget), ["Fake", "AlsoFake"]);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetField_EmptyArray_ReturnsNull()
        {
            var result = FieldHelper.GetField(typeof(FieldTestTarget), []);

            Assert.That(result, Is.Null);
        }
    }
}
