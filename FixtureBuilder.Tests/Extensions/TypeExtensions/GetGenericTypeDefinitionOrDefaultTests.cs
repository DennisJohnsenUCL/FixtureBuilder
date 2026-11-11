using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class GetGenericTypeDefinitionOrDefaultTests
    {
        [Test]
        public void NullType_ThrowsArgumentNullException()
        {
            Type type = null!;

            Assert.Throws<ArgumentNullException>(() => type.GetGenericTypeDefinitionOrDefault());
        }

        [TestCase(typeof(List<int>), typeof(List<>))]
        [TestCase(typeof(Dictionary<,>), typeof(Dictionary<,>))]
        [TestCase(typeof(int?), typeof(Nullable<>))]
        [TestCase(typeof(IList<string>), typeof(IList<>))]
        public void GenericType_ReturnsGenericTypeDefinition(Type type, Type expectedDefinition)
        {
            var result = type.GetGenericTypeDefinitionOrDefault();

            Assert.That(result, Is.EqualTo(expectedDefinition));
        }

        [Test]
        public void NonGenericClass_ReturnsNull()
        {
            var result = typeof(string).GetGenericTypeDefinitionOrDefault();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void NonGenericValueType_ReturnsNull()
        {
            var result = typeof(int).GetGenericTypeDefinitionOrDefault();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void NonGenericInterface_ReturnsNull()
        {
            var result = typeof(IDisposable).GetGenericTypeDefinitionOrDefault();

            Assert.That(result, Is.Null);
        }
    }
}
