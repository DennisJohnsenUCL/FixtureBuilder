using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class ImplementsTests
    {
        #region Arguments

        [Test]
        public void NullType_ThrowsArgumentNullException()
        {
            Type type = null!;

            Assert.Throws<ArgumentNullException>(() => type.Implements(typeof(IDisposable)));
        }

        [Test]
        public void NullTarget_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => typeof(string).Implements(null!));
        }

        [Test]
        public void TargetNotInterface_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => typeof(string).Implements(typeof(object)));

            Assert.That(ex.ParamName, Is.EqualTo("target"));
        }

        #endregion

        #region Matching

        private interface ITwoTypeArgumentsInterface<T1, T2>;

        private class ClosedGenericInterfaceClass : IGenericInterface<int>;

        private class TwoTypeArgumentsInterfaceClass : ITwoTypeArgumentsInterface<int, string>;

        private class SeveralInterfacesClass : IMarkerInterface, IGenericInterface<string>;

        [TestCase(typeof(MarkerImplementationClass), typeof(IMarkerInterface))]
        [TestCase(typeof(ClosedGenericInterfaceClass), typeof(IGenericInterface<int>))]
        [TestCase(typeof(ClosedGenericInterfaceClass), typeof(IGenericInterface<>))]
        [TestCase(typeof(TwoTypeArgumentsInterfaceClass), typeof(ITwoTypeArgumentsInterface<,>))]
        [TestCase(typeof(SeveralInterfacesClass), typeof(IMarkerInterface))]
        [TestCase(typeof(SeveralInterfacesClass), typeof(IGenericInterface<string>))]
        [TestCase(typeof(SeveralInterfacesClass), typeof(IGenericInterface<>))]
        [TestCase(typeof(List<int>), typeof(IEnumerable<int>))]
        [TestCase(typeof(List<int>), typeof(IEnumerable<>))]
        public void ImplementedInterface_ReturnsTrue(Type type, Type target)
        {
            var result = type.Implements(target);

            Assert.That(result, Is.True);
        }

        [TestCase(typeof(EmptyClass), typeof(IMarkerInterface))]
        [TestCase(typeof(ClosedGenericInterfaceClass), typeof(IGenericInterface<string>))]
        [TestCase(typeof(EmptyClass), typeof(IGenericInterface<>))]
        [TestCase(typeof(string), typeof(IDisposable))]
        public void InterfaceNotImplemented_ReturnsFalse(Type type, Type target)
        {
            var result = type.Implements(target);

            Assert.That(result, Is.False);
        }

        #endregion
    }
}
