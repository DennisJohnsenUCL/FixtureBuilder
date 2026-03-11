using FixtureBuilder.Helpers;

namespace FixtureBuilder.Tests.Helpers.FieldHelperTests
{
    internal sealed class GetCommonFieldNamesTests
    {
        [Test]
        public void GetCommonFieldNames_ReturnsExpectedConventions()
        {
            var result = FieldHelper.GetCommonFieldNames("FirstName");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Has.Length.EqualTo(4));
                Assert.That(result[0], Is.EqualTo("<FirstName>k__BackingField"));
                Assert.That(result[1], Is.EqualTo("_firstName"));
                Assert.That(result[2], Is.EqualTo("firstName"));
                Assert.That(result[3], Is.EqualTo("_FirstName"));
            }
        }

        [Test]
        public void GetCommonFieldNames_SingleCharName_ReturnsExpectedConventions()
        {
            var result = FieldHelper.GetCommonFieldNames("X");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result[0], Is.EqualTo("<X>k__BackingField"));
                Assert.That(result[1], Is.EqualTo("_x"));
                Assert.That(result[2], Is.EqualTo("x"));
                Assert.That(result[3], Is.EqualTo("_X"));
            }
        }

        [Test]
        public void GetCommonFieldNames_AlreadyLowercase_UnderscorePrefixedAndCamelCaseMatch()
        {
            var result = FieldHelper.GetCommonFieldNames("age");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result[0], Is.EqualTo("<age>k__BackingField"));
                Assert.That(result[1], Is.EqualTo("_age"));
                Assert.That(result[2], Is.EqualTo("age"));
                Assert.That(result[3], Is.EqualTo("_age"));
            }
        }
    }
}
