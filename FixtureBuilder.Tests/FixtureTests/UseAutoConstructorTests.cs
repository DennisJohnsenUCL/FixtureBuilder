namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class UseAutoConstructorTests
    {
        private class Dependency(string value)
        {
            public string Value { get; } = value;
        }

        class Empty { }
        [Test]
        public void ParameterlessConstructor_CreatesInstance()
        {
            var fixture = Fixture.New<Empty>()
                .UseAutoConstructor();

            var result = TestHelper.GetFixture(fixture);

            Assert.That(result, Is.Not.Null);
        }

        class WithPrimitives(string name, int count, long id)
        {
            public string Name { get; } = name;
            public int Count { get; } = count;
            public long Id { get; } = id;
        }
        [Test]
        public void PrimitiveParameters_AreResolved()
        {
            var fixture = Fixture.New<WithPrimitives>()
                .UseAutoConstructor();

            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Name, Is.Not.Null.And.Not.Empty);
                Assert.That(result.Count, Is.Not.Zero);
                Assert.That(result.Id, Is.Not.Zero);
            }
        }

        private class WithClassDependency(Dependency dep)
        {
            public Dependency Dep { get; } = dep;
        }
        [Test]
        public void ClassDependency_IsRecursivelyResolved()
        {
            var fixture = Fixture.New<WithClassDependency>()
                .UseAutoConstructor();

            var result = TestHelper.GetFixture(fixture);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Dep, Is.Not.Null);
            Assert.That(result.Dep.Value, Is.Not.Null.And.Not.Empty);
        }

        class DeepChild(int number)
        {
            public int Number { get; } = number;
        }

        class DeepParent(DeepChild child, string label)
        {
            public DeepChild Child { get; } = child;
            public string Label { get; } = label;
        }

        class DeepGrandparent(DeepParent parent)
        {
            public DeepParent Parent { get; } = parent;
        }
        [Test]
        public void DeeplyNestedDependencies_AreFullyResolved()
        {
            var fixture = Fixture.New<DeepGrandparent>()
                .UseAutoConstructor();

            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Parent, Is.Not.Null);
                Assert.That(result.Parent.Label, Is.Not.Null.And.Not.Empty);
                Assert.That(result.Parent.Child, Is.Not.Null);
                Assert.That(result.Parent.Child.Number, Is.Not.Zero);
            }
        }

        class WithNullable(string? optionalName, int count)
        {
            public string? OptionalName { get; } = optionalName;
            public int Count { get; } = count;
        }
        [Test]
        public void NullableParameter_ResolvesToNull()
        {
            var fixture = Fixture.New<WithNullable>()
                .UseAutoConstructor();

            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.OptionalName, Is.Null);
                Assert.That(result.Count, Is.Not.Zero);
            }
        }

        private class WithDefault(string label, int retries = 3)
        {
            public string Label { get; } = label;
            public int Retries { get; } = retries;
        }
        [Test]
        public void DefaultParameterValue_IsPreserved()
        {
            var fixture = Fixture.New<WithDefault>()
                .UseAutoConstructor();

            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Label, Is.Not.Null.And.Not.Empty);
                Assert.That(result.Retries, Is.EqualTo(3));
            }
        }

        class KitchenSink(Dependency dep, string name, int count, string? optionalTag, long sequence = 99)
        {
            public Dependency Dep { get; } = dep;
            public string Name { get; } = name;
            public int Count { get; } = count;
            public string? OptionalTag { get; } = optionalTag;
            public long Sequence { get; } = sequence;
        }
        [Test]
        public void MixedParameterTypes_AllResolvedCorrectly()
        {
            var fixture = Fixture.New<KitchenSink>()
                .UseAutoConstructor();

            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Dep, Is.Not.Null);
                Assert.That(result.Dep.Value, Is.Not.Null.And.Not.Empty);
                Assert.That(result.Name, Is.Not.Null.And.Not.Empty);
                Assert.That(result.Count, Is.Not.Zero);
                Assert.That(result.OptionalTag, Is.Null);
                Assert.That(result.Sequence, Is.EqualTo(99));
            }
        }

        private interface ICannotConstruct { }
        [Test]
        public void InterfaceType_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(
                () => Fixture.New<ICannotConstruct>().UseAutoConstructor());
        }

        private abstract class CannotConstructAbstract { }
        [Test]
        public void AbstractType_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(
                () => Fixture.New<CannotConstructAbstract>().UseAutoConstructor());
        }

        class OuterAndInner(Middle middle)
        {
            public Middle Middle = middle;
        }
        class Middle(OuterAndInner outerAndInner)
        {
            public OuterAndInner OuterAndInner = outerAndInner;
        }
        [Test]
        public void CircularDependency_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<OuterAndInner>().UseAutoConstructor());
        }
    }
}
