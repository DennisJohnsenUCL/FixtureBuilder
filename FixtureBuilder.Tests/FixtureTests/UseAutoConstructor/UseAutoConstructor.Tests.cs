namespace FixtureBuilder.Tests.FixtureTests.UseAutoConstructor
{
    internal sealed class UseAutoConstructorTests
    {
        [Test]
        public void ParameterlessConstructor_ConstructsWithIt()
        {
            var fixture = Fixture.New<ConstructorSetsValueClass>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Value, Is.EqualTo("constructor"));
        }

        private class PrimitiveParametersClass(string name, int count, long id)
        {
            public string Name { get; } = name;
            public int Count { get; } = count;
            public long Id { get; } = id;
        }

        [Test]
        public void PrimitiveParameters_ResolvesDefaultValues()
        {
            var fixture = Fixture.New<PrimitiveParametersClass>();

            var result = fixture.UseAutoConstructor().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("name"));
                Assert.That(result.Count, Is.InRange(1, 10));
                Assert.That(result.Id, Is.InRange(1, 10));
            }
        }

        [Test]
        public void ClassParameter_AutoConstructsParameter()
        {
            var fixture = Fixture.New<ClassParameterClass>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Dependency.Value, Is.EqualTo("value"));
        }

        private class Child(int number)
        {
            public int Number { get; } = number;
        }

        private class Parent(Child child, string label)
        {
            public Child Child { get; } = child;
            public string Label { get; } = label;
        }

        private class Grandparent(Parent parent)
        {
            public Parent Parent { get; } = parent;
        }

        [Test]
        public void NestedClassParameters_AutoConstructsWholeGraph()
        {
            var fixture = Fixture.New<Grandparent>();

            var result = fixture.UseAutoConstructor().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Parent.Label, Is.EqualTo("label"));
                Assert.That(result.Parent.Child.Number, Is.InRange(1, 10));
            }
        }

        [Test]
        public void InterfaceParameterWithoutTypeLink_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<InterfaceParameterClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());
        }

        private class AbstractParameterClass(AbstractClass dependency)
        {
            public AbstractClass Dependency { get; } = dependency;
        }

        [Test]
        public void AbstractParameter_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<AbstractParameterClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());
        }

        private class StructParameterClass(PointStruct point)
        {
            public PointStruct Point { get; } = point;
        }

        [Test]
        public void StructWithoutConstructorsParameter_ResolvesZeroedStruct()
        {
            var fixture = Fixture.New<StructParameterClass>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Point.X, Is.Zero);
        }

        private class NullableParameterClass(string? value)
        {
            public string? Value { get; } = value;
        }

        [Test]
        public void NullableParameter_ResolvesNull()
        {
            var fixture = Fixture.New<NullableParameterClass>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Value, Is.Null);
        }

        private class DefaultValueParameterClass(int value = 3)
        {
            public int Value { get; } = value;
        }

        [Test]
        public void DefaultValueParameter_ResolvesDefaultValue()
        {
            var fixture = Fixture.New<DefaultValueParameterClass>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Value, Is.EqualTo(3));
        }

        private class InParameterClass(in int number)
        {
            public int Number { get; } = number;
        }

        [Test]
        public void InParameter_ResolvesValue()
        {
            var fixture = Fixture.New<InParameterClass>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Number, Is.InRange(1, 10));
        }

        private class InInterfaceParameterClass(in IMarkerInterface dependency)
        {
            public IMarkerInterface Dependency { get; } = dependency;
        }

        [Test]
        public void InParameterOfLinkedType_ResolvesLinkedType()
        {
            var factory = new FixtureFactory().AddTypeLink<IMarkerInterface, MarkerImplementationClass>();
            var fixture = factory.New<InInterfaceParameterClass>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Dependency, Is.TypeOf<MarkerImplementationClass>());
        }

        private class ListInterfaceParameterClass(IList<int> numbers)
        {
            public IList<int> Numbers { get; } = numbers;
        }

        [Test]
        public void CollectionInterfaceParameter_ResolvesEmptyDefaultImplementation()
        {
            var fixture = Fixture.New<ListInterfaceParameterClass>();

            var result = fixture.UseAutoConstructor().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Numbers, Is.TypeOf<List<int>>());
                Assert.That(result.Numbers, Is.Empty);
            }
        }

        private class AsyncParametersClass(Task task, Task<int> taskOfInt, ValueTask valueTask, ValueTask<int> valueTaskOfInt)
        {
            public Task Task { get; } = task;
            public Task<int> TaskOfInt { get; } = taskOfInt;
            public ValueTask ValueTask { get; } = valueTask;
            public ValueTask<int> ValueTaskOfInt { get; } = valueTaskOfInt;
        }

        [Test]
        public void AsyncParameters_ResolvesCompletedTasks()
        {
            var fixture = Fixture.New<AsyncParametersClass>();

            var result = fixture.UseAutoConstructor().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Task.IsCompletedSuccessfully, Is.True);
                Assert.That(result.TaskOfInt.IsCompletedSuccessfully, Is.True);
                Assert.That(result.ValueTask.IsCompletedSuccessfully, Is.True);
                Assert.That(result.ValueTaskOfInt.IsCompletedSuccessfully, Is.True);
            }
        }

        private class DelegateParameterClass(Func<string, int> parse)
        {
            public Func<string, int> Parse { get; } = parse;
        }

        [Test]
        public void DelegateParameter_ResolvesDelegateReturningDefault()
        {
            var fixture = Fixture.New<DelegateParameterClass>();

            var result = fixture.UseAutoConstructor().Build();

            var returned = result.Parse("text");
            Assert.That(returned, Is.Zero);
        }

        [Test]
        public void DelegateFixture_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<Func<int>>();

            Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());
        }

        [Test]
        public void AlreadyConstructed_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<ConstructorSetsValueClass>();
            fixture.UseAutoConstructor();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseAutoConstructor());

            Assert.That(ex.Message, Does.Contain("already constructed"));
        }
    }
}
