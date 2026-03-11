using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal class TryGetGenericTypeDefinitionTests
    {
        [Test]
        public void TryGetGenericTypeDefinition_NullType_ThrowsArgumentNullException()
        {
            Type type = null!;

            Assert.Throws<ArgumentNullException>(() =>
                type.TryGetGenericTypeDefinition(out _));
        }

        [Test]
        public void TryGetGenericTypeDefinition_ClosedGenericType_ReturnsTrueWithDefinition()
        {
            var type = typeof(List<int>);

            var result = type.TryGetGenericTypeDefinition(out var genericTypeDef);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(genericTypeDef, Is.EqualTo(typeof(List<>)));
            }
        }

        [Test]
        public void TryGetGenericTypeDefinition_OpenGenericType_ReturnsTrueWithDefinition()
        {
            var type = typeof(Dictionary<,>);

            var result = type.TryGetGenericTypeDefinition(out var genericTypeDef);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(genericTypeDef, Is.EqualTo(typeof(Dictionary<,>)));
            }
        }

        [Test]
        public void TryGetGenericTypeDefinition_NonGenericType_ReturnsFalse()
        {
            var type = typeof(string);

            var result = type.TryGetGenericTypeDefinition(out var genericTypeDef);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(genericTypeDef, Is.Null);
            }
        }

        [Test]
        public void TryGetGenericTypeDefinition_GenericInterface_ReturnsTrueWithDefinition()
        {
            var type = typeof(IEnumerable<string>);

            var result = type.TryGetGenericTypeDefinition(out var genericTypeDef);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(genericTypeDef, Is.EqualTo(typeof(IEnumerable<>)));
            }
        }

        [Test]
        public void TryGetGenericTypeDefinition_NonGenericInterface_ReturnsFalse()
        {
            var type = typeof(IDisposable);

            var result = type.TryGetGenericTypeDefinition(out var genericTypeDef);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(genericTypeDef, Is.Null);
            }
        }

        [Test]
        public void TryGetGenericTypeDefinition_ValueType_ReturnsFalse()
        {
            var type = typeof(int);

            var result = type.TryGetGenericTypeDefinition(out var genericTypeDef);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(genericTypeDef, Is.Null);
            }
        }

        [Test]
        public void TryGetGenericTypeDefinition_NullableValueType_ReturnsTrueWithDefinition()
        {
            var type = typeof(int?);

            var result = type.TryGetGenericTypeDefinition(out var genericTypeDef);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(genericTypeDef, Is.EqualTo(typeof(Nullable<>)));
            }
        }
    }
}
