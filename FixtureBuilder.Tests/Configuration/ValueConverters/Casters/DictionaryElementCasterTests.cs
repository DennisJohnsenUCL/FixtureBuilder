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
        #region Value returned unchanged

        [Test]
        public void Cast_ValueNotEnumerable_ReturnsValueUnchanged()
        {
            var caster = new DictionaryElementCaster();
            var request = new TypeRequest(typeof(Dictionary<string, int>), typeof(object));
            object value = 5;

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_ValueNotDictionaryOrKeyValuePairs_ReturnsValueUnchanged()
        {
            var caster = new DictionaryElementCaster();
            var request = new TypeRequest(typeof(Dictionary<string, int>), typeof(object));
            List<string> value = ["one", "two", "three"];

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_TargetNotDictionary_ReturnsValueUnchanged()
        {
            var caster = new DictionaryElementCaster();
            var request = new TypeRequest(typeof(List<string>), typeof(object));
            var value = new Dictionary<string, int> { ["one"] = 1 };

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_NonGenericDictionaryTarget_ReturnsValueUnchanged()
        {
            var caster = new DictionaryElementCaster();
            var request = new TypeRequest(typeof(Hashtable), typeof(object));
            var value = new Dictionary<string, int> { ["one"] = 1 };

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.SameAs(value));
        }

        [Test]
        public void Cast_SameElementTypes_ReturnsValueUnchanged()
        {
            var caster = new DictionaryElementCaster();
            var request = new TypeRequest(typeof(Dictionary<string, int>), typeof(object));
            var value = new SortedDictionary<string, int> { ["one"] = 1 };

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.SameAs(value));
        }

        #endregion

        #region Elements cast

        [Test]
        public void Cast_GenericDictionaryAssignableToTargetElementTypes_ReturnsDictionaryOfTargetElementTypes()
        {
            var caster = new DictionaryElementCaster();
            var request = new TypeRequest(typeof(Dictionary<object, object>), typeof(object));
            var value = new SortedDictionary<string, int> { ["one"] = 1, ["two"] = 2 };

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<Dictionary<object, object>>());
                Assert.That(result, Is.EqualTo(new Dictionary<object, object> { ["one"] = 1, ["two"] = 2 }));
            }
        }

        [Test]
        public void Cast_GenericDictionaryOfObjects_ReturnsDictionaryOfTargetElementTypes()
        {
            var caster = new DictionaryElementCaster();
            var request = new TypeRequest(typeof(Dictionary<string, int>), typeof(object));
            var value = new SortedDictionary<object, object> { ["one"] = 1, ["two"] = 2 };

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<Dictionary<string, int>>());
                Assert.That(result, Is.EqualTo(new Dictionary<string, int> { ["one"] = 1, ["two"] = 2 }));
            }
        }

        [Test]
        public void Cast_KeyValuePairs_ReturnsDictionaryOfTargetElementTypes()
        {
            var caster = new DictionaryElementCaster();
            var request = new TypeRequest(typeof(Dictionary<object, object>), typeof(object));
            List<KeyValuePair<string, int>> value = [new("one", 1), new("two", 2)];

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<Dictionary<object, object>>());
                Assert.That(result, Is.EqualTo(new Dictionary<object, object> { ["one"] = 1, ["two"] = 2 }));
            }
        }

        [Test]
        public void Cast_NonGenericDictionary_ReturnsDictionaryOfTargetElementTypes()
        {
            var caster = new DictionaryElementCaster();
            var request = new TypeRequest(typeof(Dictionary<string, int>), typeof(object));
            var value = new Hashtable { ["one"] = 1, ["two"] = 2 };

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<Dictionary<string, int>>());
                Assert.That(result, Is.EqualTo(new Dictionary<string, int> { ["one"] = 1, ["two"] = 2 }));
            }
        }

        [Test]
        public void Cast_EmptyDictionary_ReturnsEmptyDictionaryOfTargetElementTypes()
        {
            var caster = new DictionaryElementCaster();
            var request = new TypeRequest(typeof(Dictionary<object, object>), typeof(object));
            var value = new SortedDictionary<string, int>();

            var result = caster.Cast(request, value, Mock.Of<IFixtureContext>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<Dictionary<object, object>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Cast_ValuesNotAssignable_ReturnsDictionaryOfValuesConvertedByContextConverter()
        {
            var caster = new DictionaryElementCaster();
            var request = new TypeRequest(typeof(Dictionary<string, string>), typeof(object));
            var value = new SortedDictionary<string, int> { ["one"] = 1 };
            var converter = new Mock<ICompositeConverter>();
            converter.Setup(x => x.Convert(It.IsAny<TypeRequest>(), 1, typeof(string), It.IsAny<IFixtureContext>())).Returns("converted");
            var context = new Mock<IFixtureContext>();
            context.Setup(x => x.UnwrapAndLink(typeof(string), typeof(object))).Returns(typeof(string));
            context.Setup(x => x.Converter).Returns(converter.Object);

            var result = caster.Cast(request, value, context.Object);

            Assert.That(result, Is.EqualTo(new Dictionary<string, string> { ["one"] = "converted" }));
        }

        [Test]
        public void Cast_ValuesNotConvertible_ThrowsInvalidCastException()
        {
            var caster = new DictionaryElementCaster();
            var request = new TypeRequest(typeof(Dictionary<string, long>), typeof(object));
            var value = new SortedDictionary<string, int> { ["one"] = 1 };
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
