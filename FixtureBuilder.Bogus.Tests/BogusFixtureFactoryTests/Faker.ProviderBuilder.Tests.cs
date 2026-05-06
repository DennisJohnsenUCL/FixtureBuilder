using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactoryTests
{
    internal sealed class FakerWithTests
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

        [SetUp]
        public void SetUp()
        {
            _factory = FixtureFactory.WithBogus();
        }

        #region With

        [Test]
        public void With_Faker_AppliesValue()
        {
            var result = _factory.With(f => f.Name.FirstName()).Build<ConstructorClass>();

            Assert.That(result.Name, Is.Not.Empty);
        }

        [Test]
        public void With_Faker_ProducesUniqueValues()
        {
            _factory.With(f => f.Name.FirstName());

            var result1 = _factory.Build<ConstructorClass>();
            var result2 = _factory.Build<ConstructorClass>();

            Assert.That(result1.Name, Is.Not.EqualTo(result2.Name));
        }

        [Test]
        public void With_FakerAndName_AppliesValue()
        {
            var result = _factory.With(f => f.Name.FirstName(), "name").Build<ConstructorClass>();

            Assert.That(result.Name, Is.Not.Empty);
        }

        #endregion

        #region WithParameter

        [Test]
        public void WithParameter_Faker_AppliesValue()
        {
            var result = _factory.WithParameter(f => f.Name.FirstName()).Build<ConstructorClass>();

            Assert.That(result.Name, Is.Not.Empty);
        }

        [Test]
        public void WithParameter_FakerAndName_AppliesValue()
        {
            var result = _factory.WithParameter(f => f.Name.FirstName(), "name").Build<ConstructorClass>();

            Assert.That(result.Name, Is.Not.Empty);
        }

        #endregion

        #region WithPropertyOrField

        [Test]
        public void WithPropertyOrField_Faker_AppliesValue()
        {
            _factory.SetOptions(o => o.DefaultInstantiationMethod = InstantiationMethod.CreateUninitialized);
            _factory.SetOptions(o => o.DefaultInitializeMembers = InitializeMembers.All);

            var result = _factory.WithPropertyOrField<string>(f => f.Name.FirstName()).Build<PropertyClass>();

            Assert.That(result.Name, Is.Not.Empty);
        }

        [Test]
        public void WithPropertyOrField_Faker_ProducesUniqueValues()
        {
            _factory.SetOptions(o => o.DefaultInstantiationMethod = InstantiationMethod.CreateUninitialized);
            _factory.SetOptions(o => o.DefaultInitializeMembers = InitializeMembers.All);
            _factory.WithPropertyOrField(f => f.Name.FirstName());

            var result1 = _factory.Build<PropertyClass>();
            var result2 = _factory.Build<PropertyClass>();

            Assert.That(result1.Name, Is.Not.EqualTo(result2.Name));
        }

        [Test]
        public void WithPropertyOrField_FakerAndName_AppliesValue()
        {
            _factory.SetOptions(o => o.DefaultInstantiationMethod = InstantiationMethod.CreateUninitialized);
            _factory.SetOptions(o => o.DefaultInitializeMembers = InitializeMembers.All);

            var result = _factory.WithPropertyOrField(f => f.Name.FirstName(), "Name").Build<PropertyClass>();

            Assert.That(result.Name, Is.Not.Empty);
        }

        #endregion
    }
}
