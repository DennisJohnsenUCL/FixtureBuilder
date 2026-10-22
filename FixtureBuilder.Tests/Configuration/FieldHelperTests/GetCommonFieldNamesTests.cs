using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.FieldHelperTests
{
    internal sealed class GetCommonFieldNamesTests
    {
        [Test]
        public void GetCommonFieldNames_ReturnsExpectedConventions()
        {
            var result = FieldHelper.GetCommonFieldNames("FirstName");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Has.Length.EqualTo(3));
                Assert.That(result[0], Is.EqualTo("_firstName"));
                Assert.That(result[1], Is.EqualTo("firstName"));
                Assert.That(result[2], Is.EqualTo("_FirstName"));
            }
        }

        [Test]
        public void GetCommonFieldNames_SingleCharName_ReturnsExpectedConventions()
        {
            var result = FieldHelper.GetCommonFieldNames("X");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result[0], Is.EqualTo("_x"));
                Assert.That(result[1], Is.EqualTo("x"));
                Assert.That(result[2], Is.EqualTo("_X"));
            }
        }

        [Test]
        public void GetCommonFieldNames_AlreadyLowercase_UnderscorePrefixedAndCamelCaseMatch()
        {
            var result = FieldHelper.GetCommonFieldNames("age");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result[0], Is.EqualTo("_age"));
                Assert.That(result[1], Is.EqualTo("age"));
                Assert.That(result[2], Is.EqualTo("_age"));
            }
        }
    }
}
