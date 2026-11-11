using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.Core
{
    internal sealed class FixtureRequestTests
    {
        [Test]
        public void Constructor_SetsRootType()
        {
            var property = typeof(PropertyHolder<string>).GetProperty("Value")!;

            var request = new FixtureRequest(typeof(string), property, typeof(PropertyHolder<PropertyHolder<string>>));

            Assert.That(request.RootType, Is.EqualTo(typeof(PropertyHolder<PropertyHolder<string>>)));
        }

        [Test]
        public void Constructor_PropertySource_SetsNameAndOriginalTypeFromProperty()
        {
            var property = typeof(PropertyHolder<int?>).GetProperty("Value")!;

            var request = new FixtureRequest(typeof(int), property, typeof(object));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(request.Name, Is.EqualTo("Value"));
                Assert.That(request.OriginalType, Is.EqualTo(typeof(int?)));
            }
        }

        [Test]
        public void Constructor_FieldSource_SetsNameAndOriginalTypeFromField()
        {
            var field = typeof(FieldHolder<int?>).GetField("Value")!;

            var request = new FixtureRequest(typeof(int), field, typeof(object));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(request.Name, Is.EqualTo("Value"));
                Assert.That(request.OriginalType, Is.EqualTo(typeof(int?)));
            }
        }

        [Test]
        public void Constructor_ParameterSource_SetsNameAndOriginalTypeFromParameter()
        {
            static void Method(int? parameter) { }
            var parameter = TestHelper.FirstParameter(Method);

            var request = new FixtureRequest(typeof(int), parameter, typeof(object));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(request.Name, Is.EqualTo("parameter"));
                Assert.That(request.OriginalType, Is.EqualTo(typeof(int?)));
            }
        }

        [Test]
        public void Constructor_ByRefParameterSource_SetsOriginalTypeToElementType()
        {
            static void Method(in int parameter) { }
            var parameter = TestHelper.FirstParameter(Method);

            var request = new FixtureRequest(typeof(int), parameter, typeof(object));

            Assert.That(request.OriginalType, Is.EqualTo(typeof(int)));
        }
    }
}
