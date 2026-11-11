using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace FixtureBuilder.Tests.FixtureTests.WithBackingField
{
    internal sealed class WithBackingFieldCollectionsTests
    {
        #region Field collection types

        private class EnumerableOverFieldClass<TField> where TField : IEnumerable<string>
        {
            private readonly TField _values = default!;
            public IEnumerable<string> Values => _values;
        }

        [Test]
        public void ListField_SetsConvertedCollection()
        {
            var fixture = TestHelper.MakeFixture<EnumerableOverFieldClass<List<string>>>();

            var result = fixture.WithBackingField(x => x.Values, ["value"], "_values").Build();

            Assert.That(result.Values, Is.EqualTo(["value"]));
        }

        [Test]
        public void ImmutableListField_SetsConvertedCollection()
        {
            var fixture = TestHelper.MakeFixture<EnumerableOverFieldClass<ImmutableList<string>>>();

            var result = fixture.WithBackingField(x => x.Values, ["value"], "_values").Build();

            Assert.That(result.Values, Is.EqualTo(["value"]));
        }

        [Test]
        public void FrozenSetField_SetsConvertedCollection()
        {
            var fixture = TestHelper.MakeFixture<EnumerableOverFieldClass<FrozenSet<string>>>();

            var result = fixture.WithBackingField(x => x.Values, ["value"], "_values").Build();

            Assert.That(result.Values, Is.EqualTo(["value"]));
        }

        [Test]
        public void ArrayField_SetsConvertedCollection()
        {
            var fixture = TestHelper.MakeFixture<EnumerableOverFieldClass<string[]>>();

            var result = fixture.WithBackingField(x => x.Values, ["value"], "_values").Build();

            Assert.That(result.Values, Is.EqualTo(["value"]));
        }

        [Test]
        public void BlockingCollectionField_SetsConvertedCollection()
        {
            var fixture = TestHelper.MakeFixture<EnumerableOverFieldClass<BlockingCollection<string>>>();

            var result = fixture.WithBackingField(x => x.Values, ["value"], "_values").Build();

            Assert.That(result.Values, Is.EqualTo(["value"]));
        }

        private class ArrayListFieldClass
        {
            private readonly ArrayList _values = null!;
            public IEnumerable<object> Values => _values.Cast<object>();
        }

        [Test]
        public void ArrayListField_SetsConvertedCollection()
        {
            var fixture = TestHelper.MakeFixture<ArrayListFieldClass>();

            var result = fixture.WithBackingField(x => x.Values, [123], "_values").Build();

            Assert.That(result.Values, Is.EqualTo([123]));
        }

        private class ArrayOverListFieldClass
        {
            private readonly List<string> _values = null!;
            public string[] Values => [.. _values];
        }

        [Test]
        public void ListField_ArrayProperty_SetsConvertedCollection()
        {
            var fixture = TestHelper.MakeFixture<ArrayOverListFieldClass>();

            var result = fixture.WithBackingField(x => x.Values, ["value"], "_values").Build();

            Assert.That(result.Values, Is.EqualTo(["value"]));
        }

        private class StackOverNonGenericListFieldClass
        {
            private readonly IList _values = null!;
            public Stack Values => new(_values);
        }

        [Test]
        public void NonGenericListField_StackProperty_SetsConvertedCollection()
        {
            var fixture = TestHelper.MakeFixture<StackOverNonGenericListFieldClass>();
            string[] elements = ["value"];
            var values = new Stack(elements);

            var result = fixture.WithBackingField(x => x.Values, values, "_values").Build();

            Assert.That(result.Values, Is.EqualTo(["value"]));
        }

        private class ReadOnlyListOverListFieldClass
        {
            private readonly List<string> _values = null!;
            public IReadOnlyList<string> Values => _values;
        }

        [Test]
        public void ConventionalFieldOfDifferentCollectionType_SetsConvertedCollection()
        {
            var fixture = TestHelper.MakeFixture<ReadOnlyListOverListFieldClass>();

            var result = fixture.WithBackingField(x => x.Values, ["value"]).Build();

            Assert.That(result.Values, Is.EqualTo(["value"]));
        }

        private class EnumerableOverListInterfaceFieldClass
        {
            private readonly IList<int> _values = null!;
            public IEnumerable<int> Values => _values;
        }

        [Test]
        public void InterfaceField_ValueOfOtherCollectionType_SetsDefaultImplementation()
        {
            var fixture = TestHelper.MakeFixture<EnumerableOverListInterfaceFieldClass>();
            HashSet<int> values = [1, 2, 3];

            var result = fixture.WithBackingField(x => x.Values, values, "_values").Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Values, Is.TypeOf<List<int>>());
                Assert.That(result.Values, Is.EqualTo([1, 2, 3]));
            }
        }

        #endregion

        #region Element nullability

        private class NullableValueElementsFieldClass
        {
            private readonly List<int?> _values = null!;
            public IEnumerable<int> Values => _values.Cast<int>();
        }

        [Test]
        public void NullableValueElementsField_NonNullableElements_SetsConvertedCollection()
        {
            var fixture = TestHelper.MakeFixture<NullableValueElementsFieldClass>();
            List<int> values = [123];

            var result = fixture.WithBackingField(x => x.Values, values, "_values").Build();

            Assert.That(result.Values, Is.EqualTo([123]));
        }

        private class NullableReferenceElementsFieldClass
        {
            private readonly List<string?> _values = null!;
            public IEnumerable<string> Values => _values.Cast<string>();
        }

        [Test]
        public void NullableReferenceElementsField_NonNullableElements_SetsCollection()
        {
            var fixture = TestHelper.MakeFixture<NullableReferenceElementsFieldClass>();
            List<string> values = ["value"];

            var result = fixture.WithBackingField(x => x.Values, values, "_values").Build();

            Assert.That(result.Values, Is.EqualTo(["value"]));
        }

        private class NonNullableReferenceElementsFieldClass
        {
            private readonly List<string> _values = null!;
            public IEnumerable<string?> Values => _values.Cast<string?>();
        }

        [Test]
        public void NonNullableReferenceElementsField_NullableElements_SetsCollection()
        {
            var fixture = TestHelper.MakeFixture<NonNullableReferenceElementsFieldClass>();
            List<string?> values = ["value", null];

            var result = fixture.WithBackingField(x => x.Values, values, "_values").Build();

            Assert.That(result.Values, Is.EqualTo(["value", null]));
        }

        #endregion
    }
}
