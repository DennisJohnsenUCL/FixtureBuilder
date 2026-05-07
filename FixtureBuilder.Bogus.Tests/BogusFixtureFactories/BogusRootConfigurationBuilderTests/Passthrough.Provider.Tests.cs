#pragma warning disable CS0649

using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories.BogusRootConfigurationBuilderTests
{
    internal sealed class PassthroughProviderTests
    {
        private BogusFixtureFactory _factory = null!;

        public class ConstructorClass(string name)
        {
            public string Name { get; } = name;
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
            _factory = FixtureFactory.WithBogus();
        }

        #region With

        [Test]
        public void With_Value_AppliesValue()
        {
            _factory.WhenBuilding<ConstructorClass>(b => b.With("injected"));

            var result = _factory.Build<ConstructorClass>();

            Assert.That(result.Name, Is.EqualTo("injected"));
        }

        [Test]
        public void With_Func_AppliesValue()
        {
            _factory.WhenBuilding<ConstructorClass>(b => b.With(() => "injected"));

            var result = _factory.Build<ConstructorClass>();

            Assert.That(result.Name, Is.EqualTo("injected"));
        }

        [Test]
        public void With_ValueAndName_AppliesValue()
        {
            _factory.WhenBuilding<ConstructorClass>(b => b.With("injected", "name"));

            var result = _factory.Build<ConstructorClass>();

            Assert.That(result.Name, Is.EqualTo("injected"));
        }

        [Test]
        public void With_FuncAndName_AppliesValue()
        {
            _factory.WhenBuilding<ConstructorClass>(b => b.With(() => "injected", "name"));

            var result = _factory.Build<ConstructorClass>();

            Assert.That(result.Name, Is.EqualTo("injected"));
        }

        #endregion

        #region WithParameter

        [Test]
        public void WithParameter_Value_AppliesValue()
        {
            _factory.WhenBuilding<ConstructorClass>(b => b.WithParameter("param-injected"));

            var result = _factory.Build<ConstructorClass>();

            Assert.That(result.Name, Is.EqualTo("param-injected"));
        }

        [Test]
        public void WithParameter_Func_AppliesValue()
        {
            _factory.WhenBuilding<ConstructorClass>(b => b.WithParameter(() => "param-injected"));

            var result = _factory.Build<ConstructorClass>();

            Assert.That(result.Name, Is.EqualTo("param-injected"));
        }

        [Test]
        public void WithParameter_ValueAndName_AppliesValue()
        {
            _factory.WhenBuilding<ConstructorClass>(b => b.WithParameter("param-injected", "name"));

            var result = _factory.Build<ConstructorClass>();

            Assert.That(result.Name, Is.EqualTo("param-injected"));
        }

        [Test]
        public void WithParameter_FuncAndName_AppliesValue()
        {
            _factory.WhenBuilding<ConstructorClass>(b => b.WithParameter(() => "param-injected", "name"));

            var result = _factory.Build<ConstructorClass>();

            Assert.That(result.Name, Is.EqualTo("param-injected"));
        }

        #endregion

        #region WithPropertyOrField

        [Test]
        public void WithPropertyOrField_Value_AppliesValue()
        {
            _factory.SetOptions(o => o.DefaultInstantiationMethod = InstantiationMethod.CreateUninitialized);
            _factory.SetOptions(o => o.DefaultInitializeMembers = InitializeMembers.All);

            _factory.WhenBuilding<PropertyClass>(b => b.WithPropertyOrField("prop-injected"));

            var result = _factory.Build<PropertyClass>();

            Assert.That(result.Name, Is.EqualTo("prop-injected"));
        }

        [Test]
        public void WithPropertyOrField_Func_AppliesValue()
        {
            _factory.SetOptions(o => o.DefaultInstantiationMethod = InstantiationMethod.CreateUninitialized);
            _factory.SetOptions(o => o.DefaultInitializeMembers = InitializeMembers.All);

            _factory.WhenBuilding<PropertyClass>(b => b.WithPropertyOrField(() => "prop-injected"));

            var result = _factory.Build<PropertyClass>();

            Assert.That(result.Name, Is.EqualTo("prop-injected"));
        }

        [Test]
        public void WithPropertyOrField_ValueAndName_AppliesValue()
        {
            _factory.SetOptions(o => o.DefaultInstantiationMethod = InstantiationMethod.CreateUninitialized);
            _factory.SetOptions(o => o.DefaultInitializeMembers = InitializeMembers.All);

            _factory.WhenBuilding<PropertyClass>(b => b.WithPropertyOrField("prop-injected", "Name"));

            var result = _factory.Build<PropertyClass>();

            Assert.That(result.Name, Is.EqualTo("prop-injected"));
        }

        [Test]
        public void WithPropertyOrField_FuncAndName_AppliesValue()
        {
            _factory.SetOptions(o => o.DefaultInstantiationMethod = InstantiationMethod.CreateUninitialized);
            _factory.SetOptions(o => o.DefaultInitializeMembers = InitializeMembers.All);

            _factory.WhenBuilding<PropertyClass>(b => b.WithPropertyOrField(() => "prop-injected", "Name"));

            var result = _factory.Build<PropertyClass>();

            Assert.That(result.Name, Is.EqualTo("prop-injected"));
        }

        [Test]
        public void WithPropertyOrField_Field_AppliesValue()
        {
            _factory.SetOptions(o => o.DefaultInstantiationMethod = InstantiationMethod.CreateUninitialized);
            _factory.SetOptions(o => o.DefaultInitializeMembers = InitializeMembers.All);

            _factory.WhenBuilding<FieldClass>(b => b.WithPropertyOrField(42));

            var result = _factory.Build<FieldClass>();

            Assert.That(result.Value, Is.EqualTo(42));
        }

        #endregion
    }
}