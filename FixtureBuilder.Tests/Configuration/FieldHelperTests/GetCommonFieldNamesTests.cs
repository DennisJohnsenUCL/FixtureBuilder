using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.FieldHelperTests
{
    internal sealed class GetCommonFieldNamesTests
    {
        [Test]
        public void PascalCaseName_ReturnsConventionalFieldNames()
        {
            var result = FieldHelper.GetCommonFieldNames("FirstName");

            Assert.That(result, Is.EqualTo(["_firstName", "firstName", "_FirstName"]));
        }

        [Test]
        public void SingleCharacterName_ReturnsConventionalFieldNames()
        {
            var result = FieldHelper.GetCommonFieldNames("X");

            Assert.That(result, Is.EqualTo(["_x", "x", "_X"]));
        }

        [Test]
        public void CamelCaseName_ReturnsConventionalFieldNames()
        {
            var result = FieldHelper.GetCommonFieldNames("age");

            Assert.That(result, Is.EqualTo(["_age", "age", "_age"]));
        }
    }
}
