using System.Collections;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Configuration.ValueConverters.Decorators;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.Decorators
{
    internal sealed class EnumerableElementCasterTests
    {
        [Test]
        public void Cast_NonEnumerableValue_ReturnsValueUnchanged()
        {
            var targetType = new TypeRequest(typeof(List<string>), typeof(List<string>));
            object value = 5;
            var context = new Mock<IFixtureContext>().Object;

            var caster = new EnumerableElementCaster();

            var result = caster.Cast(targetType, value, context);

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_NonEnumerableTargetType_ReturnsValueUnchanged()
        {
            var targetType = new TypeRequest(typeof(long), typeof(long));
            var value = new List<int> { 1, 2, 3 };
            var context = new Mock<IFixtureContext>().Object;

            var caster = new EnumerableElementCaster();

            var result = caster.Cast(targetType, value, context);

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_StringTargetType_ReturnsValueUnchanged()
        {
            var targetType = new TypeRequest(typeof(string), typeof(string));
            var value = new List<int> { 1, 2, 3 };
            var context = new Mock<IFixtureContext>().Object;

            var caster = new EnumerableElementCaster();

            var result = caster.Cast(targetType, value, context);

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_ArrayTargetType_ReturnsValueUnchanged()
        {
            var targetType = new TypeRequest(typeof(int[]), typeof(int[]));
            var value = new List<int> { 1, 2, 3 };
            var context = new Mock<IFixtureContext>().Object;

            var caster = new EnumerableElementCaster();

            var result = caster.Cast(targetType, value, context);

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_DictionaryTargetType_ReturnsValueUnchanged()
        {
            var targetType = new TypeRequest(typeof(Dictionary<,>), typeof(Dictionary<,>));
            var value = new List<int> { 1, 2, 3 };
            var context = new Mock<IFixtureContext>().Object;

            var caster = new EnumerableElementCaster();

            var result = caster.Cast(targetType, value, context);

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_SameElementTypes_ReturnsValueUnchanged()
        {
            var targetType = new TypeRequest(typeof(Stack<int>), typeof(Stack<int>));
            var value = new List<int> { 1, 2, 3 };
            var context = new Mock<IFixtureContext>().Object;

            var caster = new EnumerableElementCaster();

            var result = caster.Cast(targetType, value, context);

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_DifferentElementTypes_ReturnsListOfTargetElementType()
        {
            var targetType = new TypeRequest(typeof(List<object>), typeof(List<object>));
            var value = new List<int> { 1, 2, 3 };
            var context = new Mock<IFixtureContext>().Object;

            var caster = new EnumerableElementCaster();

            var result = caster.Cast(targetType, value, context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.SameAs(value));
                Assert.That(result, Is.TypeOf<List<object>>());
                Assert.That(result, Is.EqualTo(new List<object> { 1, 2, 3 }));
            }
        }

        [Test]
        public void Cast_NonGenericEnumerableSource_ReturnsListOfTargetElementType()
        {
            var targetType = new TypeRequest(typeof(List<string>), typeof(List<string>));
            var value = new ArrayList { "a", "b", "c" };
            var context = new Mock<IFixtureContext>().Object;

            var caster = new EnumerableElementCaster();

            var result = caster.Cast(targetType, value, context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.SameAs(value));
                Assert.That(result, Is.TypeOf<List<string>>());
                Assert.That(result, Is.EqualTo(new List<string> { "a", "b", "c" }));
            }
        }

        [Test]
        public void Cast_CannotCastElements_ThrowsException()
        {
            var targetType = new TypeRequest(typeof(List<long>), typeof(List<long>));
            var value = new List<int> { 1, 2, 3 };
            var context = new Mock<IFixtureContext>();
            var rootConverter = new Mock<IValueConverter>();
            rootConverter.Setup(c => c.Convert(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()))
                .Returns(NoResult.Instance);

            var composite = new Mock<ICompositeConverter>();
            var converterGraph = new ConverterGraph(rootConverter.Object, composite.Object);

            context.Setup(c => c.Converter).Returns(converterGraph);

            var caster = new EnumerableElementCaster();

            Assert.Throws<InvalidCastException>(() => caster.Cast(targetType, value, context.Object));
        }

        [Test]
        public void Cast_ElementsNotAssignable_ConvertsViaRoot()
        {
            var targetType = new TypeRequest(typeof(List<long>), typeof(List<long>));
            var value = new List<int> { 1, 2, 3 };

            var rootConverter = new Mock<IValueConverter>();
            rootConverter.Setup(c => c.Convert(It.Is<TypeRequest>(r => r.Type == typeof(long)), It.IsAny<object>(), It.IsAny<IFixtureContext>()))
                .Returns((TypeRequest r, object v, IFixtureContext c) => Convert.ChangeType(v, r.Type));

            var composite = new Mock<ICompositeConverter>();
            var converterGraph = new ConverterGraph(rootConverter.Object, composite.Object);

            var context = new Mock<IFixtureContext>();
            context.Setup(c => c.Converter).Returns(converterGraph);

            var caster = new EnumerableElementCaster();

            var result = (List<long>)caster.Cast(targetType, value, context.Object);

            Assert.That(result, Is.EqualTo(new List<long> { 1L, 2L, 3L }));
        }
    }
}
