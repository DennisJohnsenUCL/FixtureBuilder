using FixtureBuilder.UninitializedProviders;

namespace FixtureBuilder.Tests.FixtureTests;

/// <summary>
/// Smoke tests for <see cref="IFixtureConstructor{TEntity}.CreateUninitialized()"/> and
/// <see cref="IFixtureConstructor{TEntity}.CreateUninitialized(InitializeMembers)"/>,
/// verifying end-to-end uninitialized object creation through the full provider chain.
/// </summary>
[TestFixture]
internal class FixtureCreateUninitializedTests
{
    public class SimpleTestClass
    {
        public string? Name { get; set; }
        public int Count { get; set; }
    }

    public class ClassWithNullableAndNonNullable
    {
        public string Required { get; set; } = null!;
        public string? Optional { get; set; }
    }

    public class ClassWithConstructorSideEffect
    {
        public bool ConstructorWasCalled { get; }

        public ClassWithConstructorSideEffect()
        {
            ConstructorWasCalled = true;
        }
    }

    #region CreateUninitialized (parameterless)

    [Test]
    public void CreateUninitialized_SimpleClass_ReturnsConfigurator()
    {
        var fixture = Fixture.New<SimpleTestClass>();

        var result = fixture.CreateUninitialized();

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateUninitialized_SimpleClass_DoesNotInitializeMembers()
    {
        var fixture = Fixture.New<SimpleTestClass>();

        var result = fixture.CreateUninitialized();

        var built = result.Build();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(built.Name, Is.Null);
            Assert.That(built.Count, Is.Zero);
        }
    }

    #endregion

    #region CreateUninitialized (with InitializeMembers)

    [Test]
    public void CreateUninitialized_None_LeavesAllMembersDefault()
    {
        var fixture = Fixture.New<SimpleTestClass>();

        var result = fixture.CreateUninitialized(InitializeMembers.None);

        var built = result.Build();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(built.Name, Is.Null);
            Assert.That(built.Count, Is.Zero);
        }
    }

    [Test]
    public void CreateUninitialized_All_InitializesAllMembers()
    {
        var fixture = Fixture.New<SimpleTestClass>();

        var result = fixture.CreateUninitialized(InitializeMembers.All);

        var built = result.Build();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(built.Name, Is.Not.Null);
            Assert.That(built.Count, Is.Not.Zero);
        }
    }

    [Test]
    public void CreateUninitialized_NonNullables_InitializesNonNullableMembers()
    {
        var fixture = Fixture.New<ClassWithNullableAndNonNullable>();

        var result = fixture.CreateUninitialized(InitializeMembers.NonNullables);

        var built = result.Build();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(built.Required, Is.Not.Null);
            Assert.That(built.Optional, Is.Null);
        }
    }

    [Test]
    public void CreateUninitialized_ReturnsInstanceOfCorrectType()
    {
        var fixture = Fixture.New<SimpleTestClass>();

        var result = fixture.CreateUninitialized(InitializeMembers.None);

        var built = result.Build();

        Assert.That(built, Is.InstanceOf<SimpleTestClass>());
    }

    [Test]
    public void CreateUninitialized_BypassesConstructor()
    {
        var fixture = Fixture.New<ClassWithConstructorSideEffect>();

        var result = fixture.CreateUninitialized(InitializeMembers.None);

        var built = result.Build();

        Assert.That(built.ConstructorWasCalled, Is.False);
    }

    [Test]
    public void CreateUninitialized_TypeThatCannotBeCreatedUninitialized_ThrowsInvalidOperationException()
    {
        var fixture = Fixture.New<string>();

        Assert.Throws<InvalidOperationException>(() => fixture.CreateUninitialized(InitializeMembers.None));
    }

    #endregion
}
