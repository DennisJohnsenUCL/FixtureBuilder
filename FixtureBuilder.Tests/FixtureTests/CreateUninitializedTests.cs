using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Tests.FixtureTests;

internal class CreateUninitializedTests
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

    [Test]
    public void CreateUninitialized_SimpleClass_DoesNotInitializeMembers()
    {
        var fixture = Fixture.New<SimpleTestClass>();

        fixture.CreateUninitialized();

        var field = TestHelper.GetFixture(fixture);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(field.Name, Is.Null);
            Assert.That(field.Count, Is.Default);
        }
    }

    [Test]
    public void CreateUninitialized_None_LeavesAllMembersDefault()
    {
        var fixture = Fixture.New<SimpleTestClass>();

        fixture.CreateUninitialized(InitializeMembers.None);

        var field = TestHelper.GetFixture(fixture);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(field.Name, Is.Null);
            Assert.That(field.Count, Is.Default);
        }
    }

    [Test]
    public void CreateUninitialized_All_InitializesAllMembers()
    {
        var fixture = Fixture.New<ClassWithNullableAndNonNullable>();

        fixture.CreateUninitialized(InitializeMembers.All);

        var field = TestHelper.GetFixture(fixture);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(field.Required, Is.Not.Null);
            Assert.That(field.Optional, Is.Not.Null);
        }
    }

    [Test]
    public void CreateUninitialized_NonNullables_InitializesNonNullableMembers()
    {
        var fixture = Fixture.New<ClassWithNullableAndNonNullable>();

        fixture.CreateUninitialized(InitializeMembers.NonNullables);

        var field = TestHelper.GetFixture(fixture);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(field.Required, Is.Not.Null);
            Assert.That(field.Optional, Is.Null);
        }
    }

    public class ClassWithConstructorSideEffect
    {
        public bool ConstructorWasCalled { get; }
        public ClassWithConstructorSideEffect()
            => ConstructorWasCalled = true;
    }
    [Test]
    public void CreateUninitialized_BypassesConstructor()
    {
        var fixture = Fixture.New<ClassWithConstructorSideEffect>();

        fixture.CreateUninitialized(InitializeMembers.None);

        var field = TestHelper.GetFixture(fixture);
        Assert.That(field.ConstructorWasCalled, Is.False);
    }

    [Test]
    public void CreateUninitialized_TypeThatCannotBeCreatedUninitialized_ThrowsInvalidOperationException()
    {
        var fixture = Fixture.New<string>();

        Assert.Throws<InvalidOperationException>(() => fixture.CreateUninitialized(InitializeMembers.None));
    }

    [Test]
    public void CreateUninitialized_NoParameter_OptionAll_InitializesAll()
    {
        var fixture = Fixture.New<ClassWithNullableAndNonNullable>();
        TestHelper.SetOption(fixture, o => o.DefaultInitializeMembers = InitializeMembers.All);

        fixture.CreateUninitialized();

        var field = TestHelper.GetFixture(fixture);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(field.Required, Is.Not.Null);
            Assert.That(field.Optional, Is.Not.Null);
        }
    }

    [Test]
    public void CreateUninitialized_NoParameter_OptionNonNullables_InitializesNonNullables()
    {
        var fixture = Fixture.New<ClassWithNullableAndNonNullable>();
        TestHelper.SetOption(fixture, o => o.DefaultInitializeMembers = InitializeMembers.NonNullables);

        fixture.CreateUninitialized();

        var field = TestHelper.GetFixture(fixture);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(field.Required, Is.Not.Null);
            Assert.That(field.Optional, Is.Null);
        }
    }

    [Test]
    public void CreateUninitialized_NoParameter_OptionNone_InitializesNone()
    {
        var fixture = Fixture.New<ClassWithNullableAndNonNullable>();
        TestHelper.SetOption(fixture, o => o.DefaultInitializeMembers = InitializeMembers.None);

        fixture.CreateUninitialized();

        var field = TestHelper.GetFixture(fixture);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(field.Required, Is.Null);
            Assert.That(field.Optional, Is.Null);
        }
    }

    class OuterAndInner
    {
        public Middle Middle { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
    class Middle
    {
        public OuterAndInner OuterAndInner { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    [Test]
    public void CircularDependency_InitializeNonNullables_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Fixture.New<OuterAndInner>().CreateUninitialized(InitializeMembers.NonNullables));
    }

    [Test]
    public void CircularDependency_InitializeNonNullables_WithAllowResolve_ResolvesWithShell()
    {
        var fixture = Fixture.New<OuterAndInner>();

        TestHelper.SetOption(fixture, o => o.AllowResolveCircularDependencies = true);
        fixture.CreateUninitialized(InitializeMembers.NonNullables);
        var result = TestHelper.GetFixture(fixture);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Name, Is.EqualTo("Name"));
            Assert.That(result.Middle, Is.Not.Null);
            Assert.That(result.Middle.Name, Is.EqualTo("Name"));
            Assert.That(result.Middle.OuterAndInner, Is.Not.Null);
            Assert.That(result.Middle.OuterAndInner.Name, Is.EqualTo("Name"));
        }
    }
}
