using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class ImplementsTests
    {
        #region Test helpers

        private interface IEmpty { }
        private interface IGenericInterface<T> { }
        private interface IMultiGeneric<T1, T2> { }

        private class SimpleImplementation : IEmpty { }
        private class GenericImplementation : IGenericInterface<int> { }
        private class MultipleImplementation : IEmpty, IGenericInterface<string> { }
        private class MultiGenericImplementation : IMultiGeneric<int, string> { }
        private class NoInterfaces { }

        #endregion

        #region Null and argument validation

        [Test]
        public void Implements_NullType_ThrowsArgumentNullException()
        {
            Type type = null!;

            Assert.Throws<ArgumentNullException>(() =>
                type.Implements(typeof(IDisposable)));
        }

        [Test]
        public void Implements_NullTarget_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                typeof(string).Implements(null!));
        }

        [Test]
        public void Implements_TargetIsNotInterface_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                typeof(string).Implements(typeof(object)));

            Assert.That(ex.ParamName, Is.EqualTo("target"));
        }

        #endregion

        #region Non-generic interfaces

        [Test]
        public void Implements_TypeImplementsNonGenericInterface_ReturnsTrue()
        {
            var result = typeof(SimpleImplementation).Implements(typeof(IEmpty));

            Assert.That(result, Is.True);
        }

        [Test]
        public void Implements_TypeDoesNotImplementInterface_ReturnsFalse()
        {
            var result = typeof(NoInterfaces).Implements(typeof(IEmpty));

            Assert.That(result, Is.False);
        }

        #endregion

        #region Closed generic interfaces

        [Test]
        public void Implements_TypeImplementsClosedGenericInterface_ReturnsTrue()
        {
            var result = typeof(GenericImplementation).Implements(typeof(IGenericInterface<int>));

            Assert.That(result, Is.True);
        }

        [Test]
        public void Implements_TypeImplementsDifferentClosedGeneric_ReturnsFalse()
        {
            var result = typeof(GenericImplementation).Implements(typeof(IGenericInterface<string>));

            Assert.That(result, Is.False);
        }

        #endregion

        #region Open generic type definitions

        [Test]
        public void Implements_TypeImplementsOpenGenericDefinition_ReturnsTrue()
        {
            var result = typeof(GenericImplementation).Implements(typeof(IGenericInterface<>));

            Assert.That(result, Is.True);
        }

        [Test]
        public void Implements_TypeImplementsOpenMultiGenericDefinition_ReturnsTrue()
        {
            var result = typeof(MultiGenericImplementation).Implements(typeof(IMultiGeneric<,>));

            Assert.That(result, Is.True);
        }

        [Test]
        public void Implements_TypeDoesNotImplementOpenGenericDefinition_ReturnsFalse()
        {
            var result = typeof(NoInterfaces).Implements(typeof(IGenericInterface<>));

            Assert.That(result, Is.False);
        }

        #endregion

        #region Framework types

        [Test]
        public void Implements_ListImplementsIEnumerable_ReturnsTrue()
        {
            var result = typeof(List<int>).Implements(typeof(IEnumerable<int>));

            Assert.That(result, Is.True);
        }

        [Test]
        public void Implements_ListImplementsOpenIEnumerable_ReturnsTrue()
        {
            var result = typeof(List<int>).Implements(typeof(IEnumerable<>));

            Assert.That(result, Is.True);
        }

        [Test]
        public void Implements_StringImplementsIDisposable_ReturnsFalse()
        {
            var result = typeof(string).Implements(typeof(IDisposable));

            Assert.That(result, Is.False);
        }

        #endregion

        [Test]
        public void Implements_TypeWithMultipleInterfaces_MatchesBoth()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(typeof(MultipleImplementation).Implements(typeof(IEmpty)), Is.True);
                Assert.That(typeof(MultipleImplementation).Implements(typeof(IGenericInterface<string>)), Is.True);
                Assert.That(typeof(MultipleImplementation).Implements(typeof(IGenericInterface<>)), Is.True);
            }
        }
    }
}
