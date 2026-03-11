using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class GetGenericTypeDefinitionOrDefaultTests
    {
        [Test]
        public void GetGenericTypeDefinitionOrDefault_NullType_ThrowsArgumentNullException()
        {
            Type type = null!;

            Assert.Throws<ArgumentNullException>(() =>
                type.GetGenericTypeDefinitionOrDefault());
        }

        [Test]
        public void GetGenericTypeDefinitionOrDefault_ClosedGenericType_ReturnsDefinition()
        {
            var result = typeof(List<int>).GetGenericTypeDefinitionOrDefault();

            Assert.That(result, Is.EqualTo(typeof(List<>)));
        }

        [Test]
        public void GetGenericTypeDefinitionOrDefault_OpenGenericType_ReturnsDefinition()
        {
            var result = typeof(Dictionary<,>).GetGenericTypeDefinitionOrDefault();

            Assert.That(result, Is.EqualTo(typeof(Dictionary<,>)));
        }

        [Test]
        public void GetGenericTypeDefinitionOrDefault_NonGenericClass_ReturnsNull()
        {
            var result = typeof(string).GetGenericTypeDefinitionOrDefault();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetGenericTypeDefinitionOrDefault_NonGenericValueType_ReturnsNull()
        {
            var result = typeof(int).GetGenericTypeDefinitionOrDefault();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetGenericTypeDefinitionOrDefault_NullableValueType_ReturnsNullableDefinition()
        {
            var result = typeof(int?).GetGenericTypeDefinitionOrDefault();

            Assert.That(result, Is.EqualTo(typeof(Nullable<>)));
        }

        [Test]
        public void GetGenericTypeDefinitionOrDefault_GenericInterface_ReturnsDefinition()
        {
            var result = typeof(IList<string>).GetGenericTypeDefinitionOrDefault();

            Assert.That(result, Is.EqualTo(typeof(IList<>)));
        }

        [Test]
        public void GetGenericTypeDefinitionOrDefault_NonGenericInterface_ReturnsNull()
        {
            var result = typeof(IDisposable).GetGenericTypeDefinitionOrDefault();

            Assert.That(result, Is.Null);
        }
    }
}
