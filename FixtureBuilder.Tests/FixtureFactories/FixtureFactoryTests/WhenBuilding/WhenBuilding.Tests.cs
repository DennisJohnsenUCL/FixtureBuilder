namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests.WhenBuilding
{
    internal sealed class WhenBuildingTests
    {
        #region Arguments

        [Test]
        public void NullAction_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WhenBuilding<StringConstructorClass>(null!));

            Assert.That(ex.ParamName, Is.EqualTo("builderAction"));
        }

        [Test]
        public void WithFunc_NullFunc_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();
            Func<string> func = null!;

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WhenBuilding<StringConstructorClass>(builder => builder.With(func)));

            Assert.That(ex.ParamName, Is.EqualTo("func"));
        }

        [Test]
        public void WithNamed_NullName_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WhenBuilding<StringConstructorClass>(builder => builder.With("scoped", null!)));

            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        [Test]
        public void WithNamedFunc_NullFunc_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();
            Func<string> func = null!;

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WhenBuilding<StringConstructorClass>(builder => builder.With(func, "value")));

            Assert.That(ex.ParamName, Is.EqualTo("func"));
        }

        [Test]
        public void WithNamedFunc_NullName_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WhenBuilding<StringConstructorClass>(builder => builder.With(() => "scoped", null!)));

            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        [Test]
        public void WithParameterFunc_NullFunc_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();
            Func<string> func = null!;

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WhenBuilding<StringConstructorClass>(builder => builder.WithParameter(func)));

            Assert.That(ex.ParamName, Is.EqualTo("func"));
        }

        [Test]
        public void WithParameterNamed_NullName_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WhenBuilding<StringConstructorClass>(builder => builder.WithParameter("scoped", null!)));

            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        [Test]
        public void WithParameterNamedFunc_NullFunc_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();
            Func<string> func = null!;

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WhenBuilding<StringConstructorClass>(builder => builder.WithParameter(func, "value")));

            Assert.That(ex.ParamName, Is.EqualTo("func"));
        }

        [Test]
        public void WithParameterNamedFunc_NullName_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WhenBuilding<StringConstructorClass>(builder => builder.WithParameter(() => "scoped", null!)));

            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        [Test]
        public void WithPropertyOrFieldFunc_NullFunc_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();
            Func<string> func = null!;

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WhenBuilding<PropertyHolder<string>>(builder => builder.WithPropertyOrField(func)));

            Assert.That(ex.ParamName, Is.EqualTo("func"));
        }

        [Test]
        public void WithPropertyOrFieldNamed_NullName_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WhenBuilding<PropertyHolder<string>>(builder => builder.WithPropertyOrField("scoped", null!)));

            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        [Test]
        public void WithPropertyOrFieldNamedFunc_NullFunc_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();
            Func<string> func = null!;

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WhenBuilding<PropertyHolder<string>>(builder => builder.WithPropertyOrField(func, "Value")));

            Assert.That(ex.ParamName, Is.EqualTo("func"));
        }

        [Test]
        public void WithPropertyOrFieldNamedFunc_NullName_ThrowsArgumentNullException()
        {
            var factory = new FixtureFactory();

            var ex = Assert.Throws<ArgumentNullException>(() => factory.WhenBuilding<PropertyHolder<string>>(builder => builder.WithPropertyOrField(() => "scoped", null!)));

            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        #endregion

        #region Scope

        [Test]
        public void ReturnsSameFactory()
        {
            var factory = new FixtureFactory();

            var result = factory.WhenBuilding<StringConstructorClass>(builder => builder.WithParameter("scoped"));

            Assert.That(result, Is.SameAs(factory));
        }

        [Test]
        public void RootType_AppliesConfiguration()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<StringConstructorClass>(builder => builder.WithParameter("scoped"));

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void OtherRootType_DoesNotApplyConfiguration()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<StringConstructorClass>(builder => builder.WithParameter("scoped"));

            var result = factory.New<OtherStringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void NestedMemberOfRootType_UsesRootTypeConfiguration()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<ClassParameterClass>(builder => builder.WithParameter("scoped"));

            var result = factory.New<ClassParameterClass>().Build();

            Assert.That(result.Dependency.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void NestedMemberOfRootType_CreateUninitialized_UsesRootTypeConfiguration()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<PropertyHolder<PropertyHolder<string>>>(builder => builder.WithPropertyOrField("scoped"));

            var result = factory.New<PropertyHolder<PropertyHolder<string>>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void TypeAsNestedMemberOfOtherRootType_DoesNotUseItsConfiguration()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<StringConstructorClass>(builder => builder.WithParameter("scoped"));

            var result = factory.New<ClassParameterClass>().Build();

            Assert.That(result.Dependency.Value, Is.EqualTo("value"));
        }

        [Test]
        public void DifferentRootTypes_EachUseTheirOwnConfiguration()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<StringConstructorClass>(builder => builder.WithParameter("first"))
                .WhenBuilding<OtherStringConstructorClass>(builder => builder.WithParameter("second"));

            var first = factory.New<StringConstructorClass>().Build();
            var second = factory.New<OtherStringConstructorClass>().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(first.Value, Is.EqualTo("first"));
                Assert.That(second.Value, Is.EqualTo("second"));
            }
        }

        [Test]
        public void ScopedValue_WinsOverLaterFactoryWideValue()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<StringConstructorClass>(builder => builder.With("scoped"))
                .With("factory-wide");

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void FactoryWideValue_AppliesWhereScopeHasNoMatch()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<StringAndIntConstructorClass>(builder => builder.WithParameter(42))
                .With("factory-wide");

            var result = factory.New<StringAndIntConstructorClass>().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Text, Is.EqualTo("factory-wide"));
                Assert.That(result.Number, Is.EqualTo(42));
            }
        }

        [Test]
        public void SeveralValuesInOneCall_AllApply()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<StringAndIntConstructorClass>(builder => builder.WithParameter("scoped").WithParameter(42));

            var result = factory.New<StringAndIntConstructorClass>().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Text, Is.EqualTo("scoped"));
                Assert.That(result.Number, Is.EqualTo(42));
            }
        }

        [Test]
        public void ValuesInSeparateCallsForSameRootType_AllApply()
        {
            var factory = new FixtureFactory()
                .WhenBuilding<StringAndIntConstructorClass>(builder => builder.WithParameter("scoped"))
                .WhenBuilding<StringAndIntConstructorClass>(builder => builder.WithParameter(42));

            var result = factory.New<StringAndIntConstructorClass>().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Text, Is.EqualTo("scoped"));
                Assert.That(result.Number, Is.EqualTo(42));
            }
        }

        #endregion

        #region Value overloads

        [Test]
        public void With_AppliesValue()
        {
            var factory = new FixtureFactory().WhenBuilding<StringConstructorClass>(builder => builder.With("scoped"));

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithFunc_AppliesFuncValue()
        {
            var factory = new FixtureFactory().WhenBuilding<StringConstructorClass>(builder => builder.With(() => "scoped"));

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithNamed_AppliesValueToNamedMember()
        {
            var factory = new FixtureFactory().WhenBuilding<StringConstructorClass>(builder => builder.With("scoped", "value"));

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithNamedFunc_AppliesFuncValueToNamedMember()
        {
            var factory = new FixtureFactory().WhenBuilding<StringConstructorClass>(builder => builder.With(() => "scoped", "value"));

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithParameter_AppliesValue()
        {
            var factory = new FixtureFactory().WhenBuilding<StringConstructorClass>(builder => builder.WithParameter("scoped"));

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithParameterFunc_AppliesFuncValue()
        {
            var factory = new FixtureFactory().WhenBuilding<StringConstructorClass>(builder => builder.WithParameter(() => "scoped"));

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithParameterNamed_AppliesValueToNamedParameter()
        {
            var factory = new FixtureFactory().WhenBuilding<StringConstructorClass>(builder => builder.WithParameter("scoped", "value"));

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithParameterNamedFunc_AppliesFuncValueToNamedParameter()
        {
            var factory = new FixtureFactory().WhenBuilding<StringConstructorClass>(builder => builder.WithParameter(() => "scoped", "value"));

            var result = factory.New<StringConstructorClass>().Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithPropertyOrField_AppliesValue()
        {
            var factory = new FixtureFactory().WhenBuilding<PropertyHolder<string>>(builder => builder.WithPropertyOrField("scoped"));

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithPropertyOrFieldFunc_AppliesFuncValue()
        {
            var factory = new FixtureFactory().WhenBuilding<PropertyHolder<string>>(builder => builder.WithPropertyOrField(() => "scoped"));

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithPropertyOrFieldNamed_AppliesValueToNamedMember()
        {
            var factory = new FixtureFactory().WhenBuilding<PropertyHolder<string>>(builder => builder.WithPropertyOrField("scoped", "Value"));

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        [Test]
        public void WithPropertyOrFieldNamedFunc_AppliesFuncValueToNamedMember()
        {
            var factory = new FixtureFactory().WhenBuilding<PropertyHolder<string>>(builder => builder.WithPropertyOrField(() => "scoped", "Value"));

            var result = factory.New<PropertyHolder<string>>().CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("scoped"));
        }

        #endregion
    }
}
