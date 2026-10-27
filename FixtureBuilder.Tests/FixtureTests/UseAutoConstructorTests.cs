using FixtureBuilder.Core;

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

        [Test]
        public void AlreadyConstructed_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<Empty>().UseAutoConstructor();

            var ex = Assert.Throws<InvalidOperationException>(() =>
                ((IFixtureConstructor<Empty>)fixture).UseAutoConstructor());

            Assert.That(ex!.Message, Does.Contain("already constructed"));
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

        class SelfParameterWithAlternative
        {
            public bool UsedSelfParameterConstructor { get; }

#pragma warning disable IDE0060 // Remove unused parameter
            public SelfParameterWithAlternative(int first, int second) { }
            public SelfParameterWithAlternative(SelfParameterWithAlternative? parent = null) { UsedSelfParameterConstructor = true; }
#pragma warning restore IDE0060 // Remove unused parameter
        }
        [Test]
        public void SelfParameterConstructor_IsChosenAfterOtherConstructors()
        {
            var fixture = Fixture.New<SelfParameterWithAlternative>()
                .UseAutoConstructor();

            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.UsedSelfParameterConstructor, Is.False);
        }

        class Category(string name, Category? parent)
        {
            public string Name { get; } = name;
            public Category? Parent { get; } = parent;
        }
        [Test]
        public void NullableSelfParameter_ResolvesToNull()
        {
            var fixture = Fixture.New<Category>()
                .UseAutoConstructor();

            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.Not.Null.And.Not.Empty);
                Assert.That(result.Parent, Is.Null);
            }
        }

        [Test]
        public void NullableSelfParameter_NullsNotPreferred_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<Category>();
            TestHelper.SetOption(fixture, o => o.PreferNullParameterValues = false);

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());

            Assert.That(ex!.Message, Does.Contain("Failed to find a constructor for Category"));
        }

        class CategoryWithDefault(string name, CategoryWithDefault? parent = null)
        {
            public string Name { get; } = name;
            public CategoryWithDefault? Parent { get; } = parent;
        }
        [Test]
        public void SelfParameterWithDefault_NullsNotPreferred_ResolvesToDefault()
        {
            var fixture = Fixture.New<CategoryWithDefault>();
            TestHelper.SetOption(fixture, o => o.PreferNullParameterValues = false);

            fixture.UseAutoConstructor();
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Parent, Is.Null);
        }

        class Order(Category category)
        {
            public Category Category { get; } = category;
        }
        [Test]
        public void NullableSelfParameter_NestedType_UsesRootOptions()
        {
            var factory = new FixtureFactory(new FixtureOptions { PreferNullParameterValues = false })
                .WhenBuilding<Order>(b => b.SetOptions(o => o.PreferNullParameterValues = true));

            var fixture = factory.New<Order>()
                .UseAutoConstructor();

            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Category.Parent, Is.Null);
        }

        class WithAsyncParameters(Task task, Task<int> taskOfInt, ValueTask valueTask, ValueTask<int> valueTaskOfInt)
        {
            public Task Task { get; } = task;
            public Task<int> TaskOfInt { get; } = taskOfInt;
            public ValueTask ValueTask { get; } = valueTask;
            public ValueTask<int> ValueTaskOfInt { get; } = valueTaskOfInt;
        }
        [Test]
        public void AsyncParameters_AreCompleted()
        {
            var fixture = Fixture.New<WithAsyncParameters>()
                .UseAutoConstructor();

            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Task.IsCompletedSuccessfully, Is.True);
                Assert.That(result.TaskOfInt.IsCompletedSuccessfully, Is.True);
                Assert.That(result.ValueTask.IsCompletedSuccessfully, Is.True);
                Assert.That(result.ValueTaskOfInt.IsCompletedSuccessfully, Is.True);
            }
        }

        class WithDelegate(Func<string, int> parse)
        {
            public Func<string, int> Parse { get; } = parse;
        }
        [Test]
        public void DelegateParameter_ReturnsDefault()
        {
            var fixture = Fixture.New<WithDelegate>()
                .UseAutoConstructor();

            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Parse("text"), Is.Zero);
        }
    }
}
