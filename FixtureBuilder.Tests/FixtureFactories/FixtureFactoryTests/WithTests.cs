#pragma warning disable CS0649

using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class WithTests
    {
        private FixtureFactory _factory;

        public class ConstructorClass(string name)
        {
            public string Name { get; } = name;
        }

        public class MultiParamClass(string name, int age)
        {
            public string Name { get; } = name;
            public int Age { get; } = age;
        }

        public class PropertyClass
        {
            public string Name { get; set; } = "";
        }

        public class FieldClass
        {
            public int Value;
        }

        [SetUp]
        public void SetUp()
        {
            _factory = new FixtureFactory();
        }

        // --- With<T>(value) ---

        [Test]
        public void With_ReturnsFactoryForChaining()
        {
            var result = _factory.With("hello");

            Assert.That(result, Is.SameAs(_factory));
        }

        [Test]
        public void With_MatchingType_AppliesValue()
        {
            _factory.With("injected");

            var fixture = _factory.New<ConstructorClass>();
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("injected"));
        }

        [Test]
        public void With_NonMatchingType_DoesNotApply()
        {
            _factory.With(99L);

            var fixture = _factory.New<ConstructorClass>();
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.Not.EqualTo("99"));
        }

        // --- With<T>(value, name) ---

        [Test]
        public void WithName_MatchingTypeAndName_AppliesValue()
        {
            _factory.With("injected", "name");

            var fixture = _factory.New<ConstructorClass>();
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("injected"));
        }

        [Test]
        public void WithName_MatchingTypeWrongName_DoesNotApply()
        {
            _factory.With("injected", "other");

            var fixture = _factory.New<ConstructorClass>();
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.Not.EqualTo("injected"));
        }

        // --- WithParameter<T>(value) ---

        [Test]
        public void WithParameter_MatchingParameterType_AppliesValue()
        {
            _factory.WithParameter("param-injected");

            var fixture = _factory.New<ConstructorClass>();
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("param-injected"));
        }

        [Test]
        public void WithParameter_MatchingTypeButProperty_DoesNotApply()
        {
            _factory.WithParameter("param-injected");

            var fixture = _factory.New<PropertyClass>().CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.Not.EqualTo("param-injected"));
        }

        // --- WithParameter<T>(value, name) ---

        [Test]
        public void WithParameterName_MatchingTypeAndName_AppliesValue()
        {
            _factory.WithParameter("param-injected", "name");

            var fixture = _factory.New<ConstructorClass>();
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("param-injected"));
        }

        [Test]
        public void WithParameterName_MatchingTypeWrongName_DoesNotApply()
        {
            _factory.WithParameter("param-injected", "other");

            var fixture = _factory.New<ConstructorClass>();
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.Not.EqualTo("param-injected"));
        }

        // --- WithPropertyOrField<T>(value) ---

        [Test]
        public void WithPropertyOrField_MatchingPropertyType_AppliesValue()
        {
            _factory.WithPropertyOrField("prop-injected");

            var fixture = _factory.New<PropertyClass>().CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("prop-injected"));
        }

        [Test]
        public void WithPropertyOrField_MatchingFieldType_AppliesValue()
        {
            _factory.WithPropertyOrField(42);

            var fixture = _factory.New<FieldClass>().CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Value, Is.EqualTo(42));
        }

        [Test]
        public void WithPropertyOrField_MatchingTypeButParameter_DoesNotApply()
        {
            _factory.WithPropertyOrField("prop-injected");

            var fixture = _factory.New<ConstructorClass>();
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.Not.EqualTo("prop-injected"));
        }

        // --- WithPropertyOrField<T>(value, name) ---

        [Test]
        public void WithPropertyOrFieldName_MatchingTypeAndName_AppliesValue()
        {
            _factory.WithPropertyOrField("prop-injected", "Name");

            var fixture = _factory.New<PropertyClass>().CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("prop-injected"));
        }

        [Test]
        public void WithPropertyOrFieldName_MatchingTypeWrongName_DoesNotApply()
        {
            _factory.WithPropertyOrField("prop-injected", "Other");

            var fixture = _factory.New<PropertyClass>().CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.Not.EqualTo("prop-injected"));
        }

        // --- Chaining ---

        [Test]
        public void Chaining_MultipleValues_AllApplied()
        {
            _factory.With("injected").With(42);

            var fixture = _factory.New<MultiParamClass>();
            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("injected"));
                Assert.That(result.Age, Is.EqualTo(42));
            }
        }

        // --- With<T>(func) ---

        [Test]
        public void With_Func_MatchingType_AppliesValue()
        {
            _factory.With(() => "injected");

            var fixture = _factory.New<ConstructorClass>();
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("injected"));
        }

        // --- With<T>(func, name) ---

        [Test]
        public void WithName_Func_MatchingTypeAndName_AppliesValue()
        {
            _factory.With(() => "injected", "name");

            var fixture = _factory.New<ConstructorClass>();
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("injected"));
        }

        // --- WithParameter<T>(func) ---

        [Test]
        public void WithParameter_Func_MatchingParameterType_AppliesValue()
        {
            _factory.WithParameter(() => "param-injected");

            var fixture = _factory.New<ConstructorClass>();
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("param-injected"));
        }

        // --- WithParameter<T>(func, name) ---

        [Test]
        public void WithParameterName_Func_MatchingTypeAndName_AppliesValue()
        {
            _factory.WithParameter(() => "param-injected", "name");

            var fixture = _factory.New<ConstructorClass>();
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("param-injected"));
        }

        // --- WithPropertyOrField<T>(func) ---

        [Test]
        public void WithPropertyOrField_Func_MatchingPropertyType_AppliesValue()
        {
            _factory.WithPropertyOrField(() => "prop-injected");

            var fixture = _factory.New<PropertyClass>().CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("prop-injected"));
        }

        // --- WithPropertyOrField<T>(func, name) ---

        [Test]
        public void WithPropertyOrFieldName_Func_MatchingTypeAndName_AppliesValue()
        {
            _factory.WithPropertyOrField(() => "prop-injected", "Name");

            var fixture = _factory.New<PropertyClass>().CreateUninitialized(InitializeMembers.All);
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("prop-injected"));
        }
    }
}
