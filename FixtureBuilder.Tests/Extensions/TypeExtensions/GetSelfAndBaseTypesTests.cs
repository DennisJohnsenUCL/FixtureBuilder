using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests.Extensions.TypeExtensions
{
    internal sealed class GetSelfAndBaseTypesTests
    {
        #region Test helpers

        private interface IBaseInterface { }
        private interface IDerivedInterface : IBaseInterface { }

        private class Base { }
        private class Middle : Base { }
        private class Derived : Middle { }

        private class GenericBase<T> { }
        private class ClosedGenericDerived : GenericBase<int> { }

        private struct SimpleStruct { }
        private enum SimpleEnum { Value }

        #endregion

        #region Classes

        [Test]
        public void GetSelfAndBaseTypes_Object_ReturnsOnlyObject()
        {
            var result = typeof(object).GetSelfAndBaseTypes();

            Assert.That(result, Is.EqualTo(new[] { typeof(object) }));
        }

        [Test]
        public void GetSelfAndBaseTypes_TypeDerivesDirectlyFromObject_ReturnsTypeThenObject()
        {
            var result = typeof(Base).GetSelfAndBaseTypes();

            Assert.That(result, Is.EqualTo(new[] { typeof(Base), typeof(object) }));
        }

        [Test]
        public void GetSelfAndBaseTypes_MultiLevelHierarchy_ReturnsTypeThenBaseTypesFromMostToLeastDerived()
        {
            var result = typeof(Derived).GetSelfAndBaseTypes();

            Assert.That(result, Is.EqualTo(new[] { typeof(Derived), typeof(Middle), typeof(Base), typeof(object) }));
        }

        [Test]
        public void GetSelfAndBaseTypes_GenericBase_ReturnsClosedGenericBaseType()
        {
            var result = typeof(ClosedGenericDerived).GetSelfAndBaseTypes();

            Assert.That(result, Is.EqualTo(new[] { typeof(ClosedGenericDerived), typeof(GenericBase<int>), typeof(object) }));
        }

        #endregion

        #region Interfaces

        [Test]
        public void GetSelfAndBaseTypes_Interface_ReturnsOnlyInterface()
        {
            var result = typeof(IDerivedInterface).GetSelfAndBaseTypes();

            Assert.That(result, Is.EqualTo(new[] { typeof(IDerivedInterface) }));
        }

        #endregion

        #region Value types

        [Test]
        public void GetSelfAndBaseTypes_Struct_ReturnsStructThenValueTypeThenObject()
        {
            var result = typeof(SimpleStruct).GetSelfAndBaseTypes();

            Assert.That(result, Is.EqualTo(new[] { typeof(SimpleStruct), typeof(ValueType), typeof(object) }));
        }

        [Test]
        public void GetSelfAndBaseTypes_Enum_ReturnsEnumThenEnumBaseThenValueTypeThenObject()
        {
            var result = typeof(SimpleEnum).GetSelfAndBaseTypes();

            Assert.That(result, Is.EqualTo(new[] { typeof(SimpleEnum), typeof(Enum), typeof(ValueType), typeof(object) }));
        }

        #endregion

        #region Framework types

        [Test]
        public void GetSelfAndBaseTypes_ArgumentNullException_ReturnsFullExceptionHierarchy()
        {
            var result = typeof(ArgumentNullException).GetSelfAndBaseTypes();

            Assert.That(result, Is.EqualTo(new[]
            {
                typeof(ArgumentNullException),
                typeof(ArgumentException),
                typeof(SystemException),
                typeof(Exception),
                typeof(object)
            }));
        }

        #endregion
    }
}
