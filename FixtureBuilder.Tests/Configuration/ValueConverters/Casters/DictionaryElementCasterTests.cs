using System.Collections;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Configuration.ValueConverters.Casters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.Casters
{
    internal sealed class DictionaryElementCasterTests
    {
        [Test]
        public void Cast_NonEnumerableValue_ReturnsValueUnchanged()
        {
            var targetType = new TypeRequest(typeof(Dictionary<string, int>), typeof(Dictionary<string, int>));
            object value = 5;
            var context = new Mock<IFixtureContext>().Object;

            var caster = new DictionaryElementCaster();

            var result = caster.Cast(targetType, value, context);

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_ValueNotDictionaryOrEnumerableKvp_ReturnsValueUnchanged()
        {
            var targetType = new TypeRequest(typeof(Dictionary<string, int>), typeof(Dictionary<string, int>));
            var value = new List<string> { "one", "two", "three" };
            var context = new Mock<IFixtureContext>().Object;

            var caster = new DictionaryElementCaster();

            var result = caster.Cast(targetType, value, context);

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_TargetTypeNotDictionary_ReturnsValueUnchanged()
        {
            var targetType = new TypeRequest(typeof(List<string>), typeof(List<string>));
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var caster = new DictionaryElementCaster();

            var result = caster.Cast(targetType, value, context);

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_NonGenericDictionaryTargetType_ReturnsValueUnchanged()
        {
            var targetType = new TypeRequest(typeof(Hashtable), typeof(Hashtable));
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var caster = new DictionaryElementCaster();

            var result = caster.Cast(targetType, value, context);

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_SameElementTypes_ReturnsValueUnchanged()
        {
            var targetType = new TypeRequest(typeof(Dictionary<string, int>), typeof(Dictionary<string, int>));
            var value = new SortedDictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var caster = new DictionaryElementCaster();

            var result = caster.Cast(targetType, value, context);

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_GenericDictionaryValue_DifferentElementTypes_CastsElements()
        {
            var targetType = new TypeRequest(typeof(Dictionary<object, object>), typeof(Dictionary<object, object>));
            var value = new SortedDictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var caster = new DictionaryElementCaster();

            var result = caster.Cast(targetType, value, context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.SameAs(value));
                Assert.That(result, Is.TypeOf<Dictionary<object, object>>());
                Assert.That(result, Is.EqualTo(new Dictionary<object, object> { { "one", 1 }, { "two", 2 }, { "three", 3 } }));
            }
        }

        [Test]
        public void Cast_GenericDictionaryValue_DifferentElementTypes_DownCastsElements()
        {
            var targetType = new TypeRequest(typeof(Dictionary<string, int>), typeof(Dictionary<string, int>));
            var value = new SortedDictionary<object, object> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var caster = new DictionaryElementCaster();

            var result = caster.Cast(targetType, value, context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.SameAs(value));
                Assert.That(result, Is.TypeOf<Dictionary<string, int>>());
                Assert.That(result, Is.EqualTo(new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } }));
            }
        }

        [Test]
        public void Cast_KvpEnumerable_DifferentElementTypes_CastsElements()
        {
            var targetType = new TypeRequest(typeof(Dictionary<object, object>), typeof(Dictionary<object, object>));
            var value = new List<KeyValuePair<string, int>> { new("one", 1), new("two", 2), new("three", 3) };
            var context = new Mock<IFixtureContext>().Object;

            var caster = new DictionaryElementCaster();

            var result = caster.Cast(targetType, value, context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.SameAs(value));
                Assert.That(result, Is.TypeOf<Dictionary<object, object>>());
                Assert.That(result, Is.EqualTo(new Dictionary<object, object> { { "one", 1 }, { "two", 2 }, { "three", 3 } }));
            }
        }

        [Test]
        public void Cast_NonGenericDictionaryValue_CastsToTargetElementType()
        {
            var targetType = new TypeRequest(typeof(Dictionary<string, int>), typeof(Dictionary<string, int>));
            var value = new Hashtable { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var caster = new DictionaryElementCaster();

            var result = caster.Cast(targetType, value, context);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.SameAs(value));
                Assert.That(result, Is.TypeOf<Dictionary<string, int>>());
                Assert.That(result, Is.EqualTo(new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } }));
            }
        }

        [Test]
        public void Cast_EmptyDictionaryValue_DifferentElementTypes_CastsToTargetType()
        {
            var targetType = new TypeRequest(typeof(Dictionary<object, object>), typeof(Dictionary<object, object>));
            var value = new SortedDictionary<string, int>();
            var context = new Mock<IFixtureContext>().Object;

            var caster = new DictionaryElementCaster();

            var result = caster.Cast(targetType, value, context);

            Assert.That(result, Is.TypeOf<Dictionary<object, object>>().And.Empty);
        }

        [Test]
        public void Cast_CannotCastElements_ThrowsException()
        {
            var targetType = new TypeRequest(typeof(Dictionary<string, long>), typeof(Dictionary<string, long>));
            var value = new SortedDictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };

            var converter = new Mock<ICompositeConverter>();
            converter.Setup(c => c.Convert(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<IFixtureContext>()))
                .Returns(NoResult.Instance);

            var context = new Mock<IFixtureContext>();
            context.Setup(c => c.Converter).Returns(converter.Object);
            context.Setup(c => c.UnwrapAndLink(It.IsAny<Type>(), It.IsAny<Type>())).Returns((Type type, Type _) => type);

            var caster = new DictionaryElementCaster();

            Assert.Throws<InvalidCastException>(() => caster.Cast(targetType, value, context.Object));
        }

        [Test]
        public void Cast_ElementsNotAssignable_ConvertsViaContextConverter()
        {
            var targetType = new TypeRequest(typeof(Dictionary<string, long>), typeof(Dictionary<string, long>));
            var value = new SortedDictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };

            var converter = new Mock<ICompositeConverter>();
            converter.Setup(c => c.Convert(It.Is<TypeRequest>(r => r.Type == typeof(long)), It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<IFixtureContext>()))
                .Returns((TypeRequest r, object v, Type _, IFixtureContext c) => Convert.ChangeType(v, r.Type));

            var context = new Mock<IFixtureContext>();
            context.Setup(c => c.Converter).Returns(converter.Object);
            context.Setup(c => c.UnwrapAndLink(It.IsAny<Type>(), It.IsAny<Type>())).Returns((Type type, Type _) => type);

            var caster = new DictionaryElementCaster();

            var result = (Dictionary<string, long>)caster.Cast(targetType, value, context.Object);

            Assert.That(result, Is.EqualTo(new Dictionary<string, long> { { "one", 1L }, { "two", 2L }, { "three", 3L } }));
        }
    }
}
