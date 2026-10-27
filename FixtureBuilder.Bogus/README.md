# FixtureBuilder.Bogus

**FixtureBuilder.Bogus** is a [Bogus](https://github.com/bchavez/Bogus) integration for [FixtureBuilder](https://github.com/DennisJohnsenUCL/FixtureBuilder). It adds realistic fake data generation to fixture configuration, letting you use `Func<Faker, T>` lambdas anywhere you would normally pass a flat value.

Each call to `Build()` produces a fresh instance with independently generated values. Call `Build(int count)` to produce multiple instances in one go — each with its own unique data.


## Quick Example

```csharp
using FixtureBuilder;
using FixtureBuilder.Bogus;

var users = Fixture.WithBogus<User>()
    .UseCustomInstantiator(f => new User(f.Name.FirstName()))
    .WithField("_age", f => f.Random.Int(18, 65))
    .Build(3);
```

This produces three `User` instances, each with a different random name and age.


## Getting Started

FixtureBuilder.Bogus is available as a NuGet package:

```
dotnet add package FixtureBuilder.Bogus
```

The entry point is the `Fixture.WithBogus<T>()` extension method, which returns an `IBogusFixtureConstructor<T>`. From there, the API mirrors FixtureBuilder's fluent interface — every value-taking method has a Faker-accepting overload alongside the standard passthrough.


## Construction

All of FixtureBuilder's construction methods are available, plus Faker-integrated variants:

```csharp
// Standard construction (passthrough to FixtureBuilder)
Fixture.WithBogus<User>().UseAutoConstructor()
Fixture.WithBogus<User>().UseConstructor("Alice", 30)
Fixture.WithBogus<User>().CreateUninitialized()

// Faker-integrated construction
Fixture.WithBogus<User>().UseConstructor(f => [f.Name.FirstName(), f.Random.Int(18, 65)])
Fixture.WithBogus<User>().UseCustomInstantiator(f => new User(f.Name.FirstName()))
```

`UseCustomInstantiator` gives you full control over construction with access to the Faker for data generation.


## Configuration

Every value-taking configuration method has a Faker overload. You can mix and match flat values with Faker lambdas freely:

```csharp
var user = Fixture.WithBogus<User>()
    .UseAutoConstructor()
    .With(u => u.Email, f => f.Internet.Email())     // Faker lambda
    .With(u => u.IsActive, true)                      // Flat value
    .WithSetter(u => u.Role, f => f.PickRandom<Role>())
    .WithField("_score", f => f.Random.Double(0, 100))
    .Build();
```

### Available Faker Overloads

- `With(expr, Func<Faker, TProp>)`
- `WithSetter(expr, Func<Faker, TProp>)`
- `WithField(fieldName, Func<Faker, TValue>)`
- `WithField(expr, fieldName, Func<Faker, TValue>)`
- `WithBackingField(expr, Func<Faker, TProp>)`
- `WithBackingField(expr, Func<Faker, TProp>, fieldName)`
- `WithBackingFieldUntyped(expr, Func<Faker, object?>)`
- `WithBackingFieldUntyped(expr, Func<Faker, object?>, fieldName)`
- `Invoke(Func<Faker, Expression<Action<T>>>)`
- `InvokePrivate(methodName, Func<Faker, object[]>)`
- `InvokePrivate(expr, methodName, Func<Faker, object[]>)`
- `Instantiate(expr, Func<IBogusConstructor<TProp>, TProp>)`

All standard FixtureBuilder configuration methods are also available as passthroughs for values that don't need generation.


## Member Instantiation

The `Instantiate` overload provides an `IBogusConstructor<TProp>` that combines FixtureBuilder's construction methods with Faker support:

```csharp
var order = Fixture.WithBogus<Order>()
    .Instantiate(o => o.Customer, c => c.UseConstructor(f => [f.Name.FullName()]))
    .Instantiate(o => o.Product, c => c.UseCustomInstantiator(f => new Product(f.Commerce.ProductName(), f.Random.Decimal(1, 500))))
    .Build();
```


## Building Multiple Instances

`Build(int count)` produces the specified number of instances, each with freshly generated values:

```csharp
var users = Fixture.WithBogus<User>()
    .With(u => u.Name, f => f.Name.FullName())
    .With(u => u.Email, f => f.Internet.Email())
    .Build(10);
```

The returned collection is stable — re-enumerating it returns the same instances.

Fixtures created from an existing instance (`Fixture.WithBogus(instance)` or `factory.New(instance)`) reconfigure and return that same instance on every build, so `Build(int count)` only accepts a count of 1 for them.


## Locale

The default locale is `"en"`. Pass a locale to any `WithBogus` overload to change it:

```csharp
// German locale — single fixture
var bogus = Fixture.WithBogus<User>("de");

// German locale — existing instance
var configured = Fixture.WithBogus(existingUser, "de");

// German locale — factory
var factory = FixtureFactory.WithBogus("de");
```

The locale is fixed at creation and can be read back via the `Locale` property on fixtures and on `BogusFixtureFactory`.

To seed the generated data, see [Seeding](#seeding) under `BogusFixtureFactory`.


## BogusFixtureFactory

`BogusFixtureFactory` is the Bogus-integrated counterpart to `FixtureFactory`. It wraps an inner `FixtureFactory`, adding Faker-accepting overloads to the registration methods. Create one via the `FixtureFactory.WithBogus()` extension method.

### Basic Usage

```csharp
var factory = FixtureFactory.WithBogus();
var user = factory.New<User>().UseAutoConstructor().Build();
```

### Pre-Configured Values

All `With`, `WithParameter`, and `WithPropertyOrField` methods have Faker overloads alongside the standard value and func overloads:

```csharp
var factory = FixtureFactory.WithBogus()
    .With<string>(f => f.Name.FirstName())
    .With<string>(f => f.Name.FirstName(), "name")
    .WithParameter<int>(f => f.Random.Int(18, 65))
    .WithPropertyOrField<string>(f => f.Internet.Email(), "Email");

var user = factory.New<User>().UseAutoConstructor().Build();
```

### Scoped Configuration

Use `WhenBuilding` to scope registrations to a specific type, with full access to Faker overloads inside the builder:

```csharp
var factory = FixtureFactory.WithBogus()
    .WhenBuilding<User>(b => b
        .With<string>(f => f.Name.FirstName(), "name")
        .WithParameter<int>(f => f.Random.Int(18, 65), "age"))
    .WhenBuilding<Company>(b => b
        .With<string>(f => f.Company.CompanyName(), "name"));

var user = factory.New<User>().UseAutoConstructor().Build();
var company = factory.New<Company>().UseAutoConstructor().Build();
```

Scoped registrations take precedence over factory-wide registrations, regardless of registration order.

### Seeding

Set the factory's `Random` property to a seeded `Randomizer` for repeatable test data:

```csharp
var factory = FixtureFactory.WithBogus();
factory.Random = new Randomizer(42);

var user = factory.New<User>()
    .With(u => u.Email, f => f.Internet.Email())
    .Build();
```

The seed only applies to values generated through the Faker. Values FixtureBuilder generates on its own, such as numbers, characters, and Guids filled in by `UseAutoConstructor` or `CreateUninitialized`, are not seeded.

Fixtures created from a factory share its randomizer, so the generated values depend on the order the fixtures are built in.

Fixtures created directly with `Fixture.WithBogus()` have their own unseeded randomizer. It can be read via the `Random` property but not replaced, so use a factory when you need a seed.

### Options

Configure options the same way as `FixtureFactory`:

```csharp
factory.Options = new FixtureOptions { PreferSimplestConstructor = true };
factory.SetOptions(o => o.PreferSimplestConstructor = true);
```

### Extensibility

In addition to the standard `AddProvider`, `AddConverter`, and `AddTypeLink` methods, `BogusFixtureFactory` adds `AddBogusProvider` for registering providers that receive a Faker instance:

```csharp
factory.AddBogusProvider(myBogusProvider);
```

Custom Bogus providers implement `IBogusCustomProvider`. `AddBogusProvider` is also available inside `WhenBuilding` for scoped providers.


## MemberLens

The `WithField` and `InvokePrivate` methods support the Visual Studio extension [MemberLens](https://marketplace.visualstudio.com/items?itemName=DennisJohnsen.MemberLens), providing autocomplete for field and method names.


## Feedback & Issues

For bug reports, feature requests, or usage questions, please submit an issue on the [GitHub repository](https://github.com/DennisJohnsenUCL/FixtureBuilder).
