namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class WithTests
    {
        #region With

        [Test]
        public void With_MatchingType_AppliesValue()
        {
            var factory = new FixtureFactory().With("injected");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("injected"));
        }

        [Test]
        public void With_OtherType_DoesNotApply()
        {
            var factory = new FixtureFactory().With(99L);

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void With_BaseTypeOfMember_DoesNotApply()
        {
            var factory = new FixtureFactory().With<object>("injected");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        private class NullableIntConstructorClass(int? number)
        {
            public int? Number { get; } = number;
        }

        [Test]
        public void With_DeclaredNullableValueType_AppliesValue()
        {
            var factory = new FixtureFactory().With<int?>(42);

            var result = factory.New<NullableIntConstructorClass>().Build();

            Assert.That(result.Number, Is.EqualTo(42));
        }

        private class EnumerableConstructorClass(IEnumerable<int> numbers)
        {
            public IEnumerable<int> Numbers { get; } = numbers;
        }

        [Test]
        public void With_DeclaredInterfaceType_AppliesValue()
        {
            int[] numbers = [1, 2, 3];
            var factory = new FixtureFactory().With<IEnumerable<int>>(numbers);

            var result = factory.New<EnumerableConstructorClass>().Build();

            Assert.That(result.Numbers, Is.SameAs(numbers));
        }

        [Test]
        public void With_NullValue_AppliesNull()
        {
            const string? value = null;
            var factory = new FixtureFactory().With(value);

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.Null);
        }

        [Test]
        public void WithFunc_MatchingType_AppliesFuncValue()
        {
            var factory = new FixtureFactory().With(() => "injected");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("injected"));
        }

        [Test]
        public void WithNamed_MatchingTypeAndName_AppliesValue()
        {
            var factory = new FixtureFactory().With("injected", "value");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("injected"));
        }

        [Test]
        public void WithNamed_OtherName_DoesNotApply()
        {
            var factory = new FixtureFactory().With("injected", "other");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void WithNamed_NameInOtherCase_DoesNotApply()
        {
            var factory = new FixtureFactory().With("injected", "Value");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void WithNamedFunc_MatchingTypeAndName_AppliesFuncValue()
        {
            var factory = new FixtureFactory().With(() => "injected", "value");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("injected"));
        }

        #endregion

        #region WithParameter

        [Test]
        public void WithParameter_MatchingParameterType_AppliesValue()
        {
            var factory = new FixtureFactory().WithParameter("injected");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("injected"));
        }

        [Test]
        public void WithParameter_PropertyOfMatchingType_DoesNotApply()
        {
            var factory = new FixtureFactory().WithParameter("injected");

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("Value"));
        }

        [Test]
        public void WithParameterFunc_MatchingParameterType_AppliesFuncValue()
        {
            var factory = new FixtureFactory().WithParameter(() => "injected");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("injected"));
        }

        [Test]
        public void WithParameterNamed_MatchingTypeAndName_AppliesValue()
        {
            var factory = new FixtureFactory().WithParameter("injected", "value");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("injected"));
        }

        [Test]
        public void WithParameterNamed_OtherName_DoesNotApply()
        {
            var factory = new FixtureFactory().WithParameter("injected", "other");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void WithParameterNamedFunc_MatchingTypeAndName_AppliesFuncValue()
        {
            var factory = new FixtureFactory().WithParameter(() => "injected", "value");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("injected"));
        }

        #endregion

        #region WithPropertyOrField

        [Test]
        public void WithPropertyOrField_MatchingPropertyType_AppliesValue()
        {
            var factory = new FixtureFactory().WithPropertyOrField("injected");

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("injected"));
        }

        [Test]
        public void WithPropertyOrField_MatchingFieldType_AppliesValue()
        {
            var factory = new FixtureFactory().WithPropertyOrField(42);

            var result = factory.New<FieldHolder<int>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo(42));
        }

        [Test]
        public void WithPropertyOrField_ParameterOfMatchingType_DoesNotApply()
        {
            var factory = new FixtureFactory().WithPropertyOrField("injected");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void WithPropertyOrFieldFunc_MatchingPropertyType_AppliesFuncValue()
        {
            var factory = new FixtureFactory().WithPropertyOrField(() => "injected");

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("injected"));
        }

        [Test]
        public void WithPropertyOrFieldNamed_MatchingTypeAndName_AppliesValue()
        {
            var factory = new FixtureFactory().WithPropertyOrField("injected", "Value");

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("injected"));
        }

        [Test]
        public void WithPropertyOrFieldNamed_OtherName_DoesNotApply()
        {
            var factory = new FixtureFactory().WithPropertyOrField("injected", "Other");

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("Value"));
        }

        [Test]
        public void WithPropertyOrFieldNamedFunc_MatchingTypeAndName_AppliesFuncValue()
        {
            var factory = new FixtureFactory().WithPropertyOrField(() => "injected", "Value");

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("injected"));
        }

        #endregion

        #region Registration

        [Test]
        public void ReturnsSameFactory()
        {
            var factory = new FixtureFactory();

            var result = factory.With("injected");

            Assert.That(result, Is.SameAs(factory));
        }

        [Test]
        public void ValuesOfDifferentTypes_AllApply()
        {
            var factory = new FixtureFactory().With("injected").With(42);

            var result = factory.New<StringAndIntConstructorClass>().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Text, Is.EqualTo("injected"));
                Assert.That(result.Number, Is.EqualTo(42));
            }
        }

        [Test]
        public void LaterValue_WinsOverEarlierValue()
        {
            var factory = new FixtureFactory().With("first").With("second");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("second"));
        }

        private class TwoStringsConstructorClass(string first, string second)
        {
            public string First { get; } = first;
            public string Second { get; } = second;
        }

        [Test]
        public void Func_IsInvokedForEachMatchingMember()
        {
            var calls = 0;
            var factory = new FixtureFactory().With(() => $"call {++calls}");

            var result = factory.New<TwoStringsConstructorClass>().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.First, Is.EqualTo("call 1"));
                Assert.That(result.Second, Is.EqualTo("call 2"));
            }
        }

        [Test]
        public void Func_IsNotInvokedForNonMatchingMembers()
        {
            var calls = 0;
            var factory = new FixtureFactory().With(() =>
            {
                calls++;
                return 1.5;
            });

            factory.New<StringConstructorClass>().Build();

            Assert.That(calls, Is.Zero);
        }

        #endregion
    }
}
