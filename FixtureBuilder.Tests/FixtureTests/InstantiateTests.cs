#pragma warning disable IDE0079
#pragma warning disable CA1822

namespace FixtureBuilder.Tests.FixtureTests
{
    internal class InstantiateTests
    {
        class Inner
        {
            public string Value { get; set; } = "constructed";
        }

        class InnerWithParam(string value)
        {
            public string Value { get; } = value;
        }

        class Outer
        {
            public Inner Nested { get; set; } = null!;
        }

        class OuterReadOnly
        {
            public Inner Nested { get; } = null!;
        }

        class OuterWithParamNested
        {
            public InnerWithParam Nested { get; set; } = null!;
        }

        class DeeperOuter
        {
            public Outer Nested { get; set; } = null!;
        }

        [Test]
        public void Instantiate_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<Outer>();

            fixture.Instantiate(o => o.Nested);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Nested, Is.Not.Null);
        }

        [Test]
        public void Instantiate_AutoConstructsInstance()
        {
            var fixture = TestHelper.MakeFixture<Outer>();

            fixture.Instantiate(o => o.Nested);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Nested.Value, Is.EqualTo("constructed"));
        }

        [Test]
        public void Instantiate_ReadOnlyProperty_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<OuterReadOnly>();

            fixture.Instantiate(o => o.Nested);

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Nested, Is.Not.Null);
        }

        [Test]
        public void Instantiate_InstantiatesFixture()
        {
            var fixture = TestHelper.MakeFixture<Outer>();

            fixture.Instantiate(o => o.Nested);

            Assert.That(TestHelper.GetFixture(fixture), Is.Not.Null);
        }

        [Test]
        public void Instantiate_UseAutoConstructor_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<Outer>();

            fixture.Instantiate(o => o.Nested, m => m.UseAutoConstructor());

            var result = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Nested, Is.Not.Null);
                Assert.That(result.Nested.Value, Is.EqualTo("constructed"));
            }
        }

        [Test]
        public void Instantiate_UseConstructor_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<OuterWithParamNested>();

            fixture.Instantiate(o => o.Nested, m => m.UseConstructor("hello"));

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Nested.Value, Is.EqualTo("hello"));
        }

        [Test]
        public void Instantiate_CreateUninitialized_SetsProperty()
        {
            var fixture = TestHelper.MakeFixture<Outer>();

            fixture.Instantiate(o => o.Nested, m => m.CreateUninitialized());

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Nested, Is.Not.Null);
        }

        [Test]
        public void Instantiate_CreateUninitialized_DoesNotInitializeMembers()
        {
            var fixture = TestHelper.MakeFixture<Outer>();

            fixture.Instantiate(o => o.Nested, m => m.CreateUninitialized());

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Nested.Value, Is.Null);
        }

        [Test]
        public void Instantiate_WithFunc_ReadOnlyProperty_SetsBackingField()
        {
            var fixture = TestHelper.MakeFixture<OuterReadOnly>();

            fixture.Instantiate(o => o.Nested, m => m.UseAutoConstructor());

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Nested, Is.Not.Null);
        }

        [Test]
        public void Instantiate_ReturnsConfigurator_ForChaining()
        {
            var fixture = TestHelper.MakeFixture<Outer>();

            var result = fixture.Instantiate(o => o.Nested);

            Assert.That(result, Is.SameAs(fixture));
        }

        [Test]
        public void Instantiate_WithFunc_ReturnsConfigurator_ForChaining()
        {
            var fixture = TestHelper.MakeFixture<Outer>();

            var result = fixture.Instantiate(o => o.Nested, m => m.UseAutoConstructor());

            Assert.That(result, Is.SameAs(fixture));
        }

        class OuterWithField
        {
            public Inner Nested = null!;
        }

        [Test]
        public void Instantiate_FieldTarget_SetsField()
        {
            var fixture = TestHelper.MakeFixture<OuterWithField>();

            fixture.Instantiate(o => o.Nested);

            var result = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Nested, Is.Not.Null);
                Assert.That(result.Nested.Value, Is.EqualTo("constructed"));
            }
        }

        [Test]
        public void Instantiate_WithFunc_FieldTarget_SetsField()
        {
            var fixture = TestHelper.MakeFixture<OuterWithField>();

            fixture.Instantiate(o => o.Nested, m => m.UseAutoConstructor());

            var result = TestHelper.GetFixture(fixture);
            Assert.That(result.Nested, Is.Not.Null);
        }

        class OuterWithNestedField
        {
            public MiddleWithField Middle = null!;
        }
        class MiddleWithField
        {
            public Inner Nested = null!;
        }
        [Test]
        public void Instantiate_DoublyNestedFieldTarget_SetsField()
        {
            var fixture = TestHelper.MakeFixture<OuterWithNestedField>();

            fixture.Instantiate(o => o.Middle.Nested);

            var result = TestHelper.GetFixture(fixture);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Middle, Is.Not.Null);
                Assert.That(result.Middle.Nested, Is.Not.Null);
                Assert.That(result.Middle.Nested.Value, Is.EqualTo("constructed"));
            }
        }
    }
}
