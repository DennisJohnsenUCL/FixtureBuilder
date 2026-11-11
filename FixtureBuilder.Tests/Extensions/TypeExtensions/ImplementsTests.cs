using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class ImplementsTests
    {
        private interface ITwoTypeArgumentsInterface<T1, T2>;

        private class ClosedGenericInterfaceClass : IGenericInterface<int>;

        private class TwoTypeArgumentsInterfaceClass : ITwoTypeArgumentsInterface<int, string>;

        private class SeveralInterfacesClass : IMarkerInterface, IGenericInterface<string>;

        [TestCase(typeof(ClosedGenericInterfaceClass), typeof(IGenericInterface<>))]
        [TestCase(typeof(TwoTypeArgumentsInterfaceClass), typeof(ITwoTypeArgumentsInterface<,>))]
        [TestCase(typeof(SeveralInterfacesClass), typeof(IGenericInterface<>))]
        [TestCase(typeof(List<int>), typeof(IEnumerable<>))]
        public void ImplementedInterface_ReturnsTrue(Type type, Type target)
        {
            var result = type.Implements(target);

            Assert.That(result, Is.True);
        }

        [TestCase(typeof(EmptyClass), typeof(IGenericInterface<>))]
        public void InterfaceNotImplemented_ReturnsFalse(Type type, Type target)
        {
            var result = type.Implements(target);

            Assert.That(result, Is.False);
        }
    }
}
