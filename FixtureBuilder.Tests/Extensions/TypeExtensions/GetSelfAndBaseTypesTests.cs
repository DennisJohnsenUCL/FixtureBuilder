using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class GetSelfAndBaseTypesTests
    {
        #region Classes

        private class Base;

        private class Middle : Base;

        private class Derived : Middle;

        [Test]
        public void Object_ReturnsOnlyObject()
        {
            var result = typeof(object).GetSelfAndBaseTypes();

            Assert.That(result, Is.EqualTo([typeof(object)]));
        }

        [Test]
        public void DirectSubclassOfObject_ReturnsTypeThenObject()
        {
            var result = typeof(Base).GetSelfAndBaseTypes();

            Assert.That(result, Is.EqualTo([typeof(Base), typeof(object)]));
        }

        [Test]
        public void MultiLevelHierarchy_ReturnsTypeThenBaseTypesFromMostToLeastDerived()
        {
            var result = typeof(Derived).GetSelfAndBaseTypes();

            Assert.That(result, Is.EqualTo([typeof(Derived), typeof(Middle), typeof(Base), typeof(object)]));
        }

        private class GenericBase<T>;

        private class ClosedGenericDerived : GenericBase<int>;

        [Test]
        public void GenericBase_ReturnsClosedGenericBaseType()
        {
            var result = typeof(ClosedGenericDerived).GetSelfAndBaseTypes();

            Assert.That(result, Is.EqualTo([typeof(ClosedGenericDerived), typeof(GenericBase<int>), typeof(object)]));
        }

        #endregion

        #region Interfaces

        private interface IBaseInterface;

        private interface IDerivedInterface : IBaseInterface;

        [Test]
        public void Interface_ReturnsOnlyInterface()
        {
            var result = typeof(IDerivedInterface).GetSelfAndBaseTypes();

            Assert.That(result, Is.EqualTo([typeof(IDerivedInterface)]));
        }

        #endregion

        #region Value types

        [Test]
        public void Struct_ReturnsStructThenValueTypeThenObject()
        {
            var result = typeof(PointStruct).GetSelfAndBaseTypes();

            Assert.That(result, Is.EqualTo([typeof(PointStruct), typeof(ValueType), typeof(object)]));
        }

        [Test]
        public void Enum_ReturnsEnumThenEnumBaseThenValueTypeThenObject()
        {
            var result = typeof(DayOfWeek).GetSelfAndBaseTypes();

            Assert.That(result, Is.EqualTo([typeof(DayOfWeek), typeof(Enum), typeof(ValueType), typeof(object)]));
        }

        #endregion
    }
}
