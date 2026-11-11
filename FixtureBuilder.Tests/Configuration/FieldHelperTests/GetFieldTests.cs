using FixtureBuilder.Configuration;

namespace FixtureBuilder.Tests.Configuration.FieldHelperTests
{
    internal sealed class GetFieldTests
    {
        private class PublicAndPrivateFieldClass
        {
            public int PublicField;
            private readonly string _privateField = null!;
        }

        [Test]
        public void FirstNameMatches_ReturnsFirstNamedField()
        {
            var result = FieldHelper.GetField(typeof(PublicAndPrivateFieldClass), ["PublicField", "_privateField"]);

            Assert.That(result?.Name, Is.EqualTo("PublicField"));
        }

        [Test]
        public void OnlySecondNameMatches_ReturnsSecondNamedField()
        {
            var result = FieldHelper.GetField(typeof(PublicAndPrivateFieldClass), ["Unknown", "_privateField"]);

            Assert.That(result?.Name, Is.EqualTo("_privateField"));
        }

        [Test]
        public void NoNameMatches_ReturnsNull()
        {
            var result = FieldHelper.GetField(typeof(PublicAndPrivateFieldClass), ["Unknown", "AlsoUnknown"]);

            Assert.That(result, Is.Null);
        }
    }
}
