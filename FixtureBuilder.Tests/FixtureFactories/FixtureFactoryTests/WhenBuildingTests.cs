#pragma warning disable CS0649

namespace FixtureBuilder.Tests.FixtureFactories.FixtureFactoryTests
{
    internal sealed class WhenBuildingTests
    {
        private FixtureFactory _factory;

        public class PersonClass(string name)
        {
            public string Name { get; } = name;
        }

        public class BusinessClass(string name)
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
            _factory = new FixtureFactory();
        }

        [Test]
        public void WhenBuilding_ReturnsFactoryForChaining()
        {
            var result = _factory.WhenBuilding<PersonClass>(b => b.With("hello"));

            Assert.That(result, Is.SameAs(_factory));
        }

        [Test]
        public void WhenBuilding_NullAction_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _factory.WhenBuilding<PersonClass>(null!));
        }

        [Test]
        public void WhenBuilding_MatchingRootType_AppliesValue()
        {
            _factory.WhenBuilding<PersonClass>(b => b.WithParameter("person-name"));

            var fixture = _factory.New<PersonClass>();
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.EqualTo("person-name"));
        }

        [Test]
        public void WhenBuilding_NonMatchingRootType_DoesNotApply()
        {
            _factory.WhenBuilding<PersonClass>(b => b.WithParameter("person-name"));

            var fixture = _factory.New<BusinessClass>();
            var result = TestHelper.GetFixture(fixture);

            Assert.That(result.Name, Is.Not.EqualTo("person-name"));
        }

        [Test]
        public void WhenBuilding_DifferentRootTypes_DifferentValues()
        {
            _factory
                .WhenBuilding<PersonClass>(b => b.WithParameter("person-name"))
                .WhenBuilding<BusinessClass>(b => b.WithParameter("business-name"));

            var person = TestHelper.GetFixture(_factory.New<PersonClass>());
            var business = TestHelper.GetFixture(_factory.New<BusinessClass>());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(person.Name, Is.EqualTo("person-name"));
                Assert.That(business.Name, Is.EqualTo("business-name"));
            }
        }

        public class MultiParamClass(string name, int age)
        {
            public string Name { get; } = name;
            public int Age { get; } = age;
        }

        [Test]
        public void WhenBuilding_MultipleWithCalls_AllApplied()
        {
            _factory.WhenBuilding<MultiParamClass>(b => b
                .WithParameter("person-name")
                .WithParameter(42));

            var fixture = _factory.New<MultiParamClass>();
            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("person-name"));
                Assert.That(result.Age, Is.EqualTo(42));
            }
        }
    }
}
