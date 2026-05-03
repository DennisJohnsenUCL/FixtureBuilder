#pragma warning disable CS0649

namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    public class CastToTests
    {
        private interface IAnimal
        {
            string Name { get; }
            int Age { get; }
        }

        private class Dog : IAnimal, ILiving
        {
            public string Breed { get; set; } = string.Empty;

            string ILiving.Name => _name;

            string IAnimal.Name => _name;
            int IAnimal.Age => _age;

            private readonly string _name = string.Empty;
            private readonly int _age;
        }

        [Test]
        public void CastTo_ReturnsCorrectType()
        {
            var result = Fixture.WithBogus<Dog>()
                .CastTo<IAnimal>()
                .Build();

            Assert.That(result, Is.InstanceOf<IAnimal>());
        }

        [Test]
        public void CastTo_ConfigurationBeforeCast_IsApplied()
        {
            var result = Fixture.WithBogus<Dog>()
                .With(d => d.Breed, "Labrador")
                .CastTo<IAnimal>()
                .Build();

            var dog = (Dog)result;
            Assert.That(dog.Breed, Is.EqualTo("Labrador"));
        }

        [Test]
        public void CastTo_ConfigurationAfterCast_IsApplied()
        {
            var result = Fixture.WithBogus<Dog>()
                .CastTo<IAnimal>()
                .WithBackingField(a => a.Name, "Rex")
                .Build();

            Assert.That(result.Name, Is.EqualTo("Rex"));
        }

        [Test]
        public void CastTo_ConfigurationBeforeAndAfterCast_BothApplied()
        {
            var result = Fixture.WithBogus<Dog>()
                .With(d => d.Breed, "Poodle")
                .CastTo<IAnimal>()
                .WithBackingField(a => a.Name, "Fido")
                .Build();

            var dog = (Dog)result;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(dog.Breed, Is.EqualTo("Poodle"));
                Assert.That(result.Name, Is.EqualTo("Fido"));
            }
        }

        [Test]
        public void CastTo_WithFaker_BeforeAndAfterCast()
        {
            var result = Fixture.WithBogus<Dog>()
                .With(d => d.Breed, f => f.Random.Word())
                .CastTo<IAnimal>()
                .WithBackingField(a => a.Name, f => f.Name.FirstName())
                .Build();

            var dog = (Dog)result;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(dog.Breed, Is.Not.Empty);
                Assert.That(result.Name, Is.Not.Empty);
            }
        }

        [Test]
        public void CastTo_BuildMultiple_ProducesDistinctInstances()
        {
            var results = Fixture.WithBogus<Dog>()
                .With(d => d.Breed, f => f.Random.Word())
                .CastTo<IAnimal>()
                .WithBackingField(a => a.Name, f => f.Name.FirstName())
                .Build(2)
                .ToList();

            Assert.That(results[0], Is.Not.SameAs(results[1]));
        }

        [Test]
        public void CastTo_BuildMultiple_FakerValuesBeforeCast_AreDifferent()
        {
            var results = Fixture.WithBogus<Dog>()
                .With(d => d.Breed, f => f.Random.Word())
                .CastTo<IAnimal>()
                .Build(2)
                .ToList();

            var dog0 = (Dog)results[0];
            var dog1 = (Dog)results[1];

            Assert.That(dog0.Breed, Is.Not.EqualTo(dog1.Breed));
        }

        [Test]
        public void CastTo_BuildMultiple_FakerValuesAfterCast_AreDifferent()
        {
            var results = Fixture.WithBogus<Dog>()
                .CastTo<IAnimal>()
                .WithBackingField(a => a.Name, f => f.Name.FirstName())
                .Build(2)
                .ToList();

            Assert.That(results[0].Name, Is.Not.EqualTo(results[1].Name));
        }

        [Test]
        public void CastTo_BuildMultiple_AllConfigurationAppliedToEach()
        {
            var results = Fixture.WithBogus<Dog>()
                .With(d => d.Breed, f => f.Random.Word())
                .CastTo<IAnimal>()
                .WithBackingField(a => a.Name, f => f.Name.FirstName())
                .WithBackingField(a => a.Age, f => f.Random.Int(1, 15))
                .Build(3)
                .ToList();

            foreach (var animal in results)
            {
                var dog = (Dog)animal;

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(dog.Breed, Is.Not.Empty);
                    Assert.That(animal.Name, Is.Not.Empty);
                    Assert.That(animal.Age, Is.InRange(1, 15));
                }
            }
        }

        [Test]
        public void CastTo_InvalidCast_Throws()
        {
            Assert.That(() => Fixture.WithBogus<Dog>()
                .CastTo<string>()
                .Build(),
                Throws.TypeOf<InvalidCastException>());
        }

        private interface ILiving
        {
            string Name { get; }
        }

        [Test]
        public void CastTo_DoubleCast_AllConfigurationApplied()
        {
            var results = Fixture.WithBogus<Dog>()
                .With(d => d.Breed, f => f.Random.Word())
                .CastTo<IAnimal>()
                .WithBackingField(a => a.Name, f => f.Name.FirstName())
                .WithBackingField(a => a.Age, f => f.Random.Int(1, 15))
                .CastTo<ILiving>()
                .Build(3)
                .ToList();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(results[0], Is.Not.SameAs(results[1]));
                Assert.That(results[1], Is.Not.SameAs(results[2]));
            }

            foreach (var living in results)
            {
                var dog = (Dog)living;

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(dog.Breed, Is.Not.Empty);
                    Assert.That(living.Name, Is.Not.Empty);
                    Assert.That(((IAnimal)dog).Age, Is.InRange(1, 15));
                }
            }
        }

        [Test]
        public void CastTo_DoubleCast_BuildMultiple_AllFakerValuesAreFresh()
        {
            var results = Fixture.WithBogus<Dog>()
                .With(d => d.Breed, f => f.Random.Word())
                .CastTo<IAnimal>()
                .WithBackingField(a => a.Name, f => f.Name.FirstName())
                .CastTo<ILiving>()
                .Build(2)
                .ToList();

            var dog0 = (Dog)results[0];
            var dog1 = (Dog)results[1];

            using (Assert.EnterMultipleScope())
            {
                Assert.That(dog0.Breed, Is.Not.EqualTo(dog1.Breed));
                Assert.That(results[0].Name, Is.Not.EqualTo(results[1].Name));
            }
        }

        [Test]
        public void CastTo_DoubleCast_ConfigurationAfterSecondCast_IsApplied()
        {
            var result = Fixture.WithBogus<Dog>()
                .With(d => d.Breed, "Husky")
                .CastTo<IAnimal>()
                .WithBackingField(a => a.Age, 5)
                .CastTo<ILiving>()
                .Build();

            var dog = (Dog)result;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(dog.Breed, Is.EqualTo("Husky"));
                Assert.That(((IAnimal)dog).Age, Is.EqualTo(5));
                Assert.That(result.Name, Is.Empty);
            }
        }
    }
}
