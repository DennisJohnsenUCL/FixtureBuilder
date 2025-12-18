namespace FixtureBuilder.Tests
{
    internal sealed class InstantiationTests
    {
        class ClassWithMember
        {
            public NestedClass NestedClass = null!;
        }
        [Test]
        public void ClassWithMembers_InstantiatesClassMembers()
        {
            var fixture = Fixture.New<ClassWithMember>().Build();

            Assert.That(fixture.NestedClass, Is.Not.Null);
        }

        class ClassWithOnlyNullable
        {
            public NestedClass? NullableClass { get; set; }
        }
        [Test]
        public void ClassWithOnlyNullable_DoesNotInstantiateNullableMember()
        {
            var fixture = Fixture.New<ClassWithOnlyNullable>().Build();

            Assert.That(fixture.NullableClass, Is.Null);
        }

        [Test]
        public void ClassWithNullable_DoesNotInstantiateNullableMember()
        {
            var fixture = Fixture.New<ClassWithNullable>().Build();

            Assert.That(fixture.NullableClass, Is.Null);
        }

        class ReadOnlyFieldClass()
        {
            public readonly string ReadOnlyField = null!;
        }
        [Test]
        public void ReadOnlyFieldNotSet_HasDefaultValue()
        {
            var fixture = Fixture.New<ReadOnlyFieldClass>().Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(fixture.ReadOnlyField, Is.Not.Null);
                Assert.That(fixture.ReadOnlyField, Is.EqualTo(""));
            }
        }
    }
}
