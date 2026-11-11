using System.Collections;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Configuration.ValueConverters.Casters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.Casters
{
    internal sealed class EnumerableElementCasterTests
    {
        #region Value returned unchanged

        [Test]
        public void Cast_ValueNotEnumerable_ReturnsValueUnchanged()
        {
            var caster = new EnumerableElementCaster();
            var request = new TypeRequest(typeof(List<string>), typeof(object));
            object value = 5;

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_TargetNotEnumerable_ReturnsValueUnchanged()
        {
            var caster = new EnumerableElementCaster();
            var request = new TypeRequest(typeof(long), typeof(object));
            List<int> value = [1, 2, 3];

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_StringTarget_ReturnsValueUnchanged()
        {
            var caster = new EnumerableElementCaster();
            var request = new TypeRequest(typeof(string), typeof(object));
            List<int> value = [1, 2, 3];

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_ArrayTarget_ReturnsValueUnchanged()
        {
            var caster = new EnumerableElementCaster();
            var request = new TypeRequest(typeof(long[]), typeof(object));
            List<int> value = [1, 2, 3];

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_DictionaryTarget_ReturnsValueUnchanged()
        {
            var caster = new EnumerableElementCaster();
            var request = new TypeRequest(typeof(Dictionary<string, int>), typeof(object));
            List<int> value = [1, 2, 3];

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_SameElementTypes_ReturnsValueUnchanged()
        {
            var caster = new EnumerableElementCaster();
            var request = new TypeRequest(typeof(Stack<int>), typeof(object));
            List<int> value = [1, 2, 3];

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.SameAs(value));
        }

        #endregion

        #region Elements cast

        [Test]
        public void Cast_ElementsAssignableToTargetElementType_ReturnsListOfTargetElementType()
        {
            var caster = new EnumerableElementCaster();
            var request = new TypeRequest(typeof(List<object>), typeof(object));
            List<int> value = [1, 2, 3];

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<List<object>>());
                Assert.That(result, Is.EqualTo([1, 2, 3]));
            }
        }

        [Test]
        public void Cast_NonGenericSource_ReturnsListOfTargetElementType()
        {
            var caster = new EnumerableElementCaster();
            var request = new TypeRequest(typeof(List<string>), typeof(object));
            var value = new ArrayList { "a", "b", "c" };

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<List<string>>());
                Assert.That(result, Is.EqualTo(["a", "b", "c"]));
            }
        }

        [Test]
        public void Cast_ElementsNotAssignable_ReturnsListOfElementsConvertedByContextConverter()
        {
            var caster = new EnumerableElementCaster();
            var request = new TypeRequest(typeof(List<string>), typeof(object));
            List<int> value = [1, 2];
            var converter = new Mock<ICompositeConverter>();
            converter.Setup(x => x.Convert(It.IsAny<TypeRequest>(), 1, typeof(string), It.IsAny<IFixtureContext>())).Returns("one");
            converter.Setup(x => x.Convert(It.IsAny<TypeRequest>(), 2, typeof(string), It.IsAny<IFixtureContext>())).Returns("two");
            var context = new Mock<IFixtureContext>();
            context.Setup(x => x.UnwrapAndLink(typeof(string), typeof(object))).Returns(typeof(string));
            context.Setup(x => x.Converter).Returns(converter.Object);

            var result = caster.Cast(request, value, context.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<List<string>>());
                Assert.That(result, Is.EqualTo(["one", "two"]));
            }
        }

        [Test]
        public void Cast_ElementsNotConvertible_ThrowsInvalidCastException()
        {
            var caster = new EnumerableElementCaster();
            var request = new TypeRequest(typeof(List<long>), typeof(object));
            List<int> value = [1, 2, 3];
            var converter = new Mock<ICompositeConverter>();
            converter.Setup(x => x.Convert(It.IsAny<TypeRequest>(), It.IsAny<object>(), typeof(long), It.IsAny<IFixtureContext>())).Returns(NoResult.Instance);
            var context = new Mock<IFixtureContext>();
            context.Setup(x => x.UnwrapAndLink(typeof(long), typeof(object))).Returns(typeof(long));
            context.Setup(x => x.Converter).Returns(converter.Object);

            Assert.Throws<InvalidCastException>(() => caster.Cast(request, value, context.Object));
        }

        #endregion
    }
}
