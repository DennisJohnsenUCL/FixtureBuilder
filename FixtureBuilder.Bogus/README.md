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

The entry point is the `Fixture.WithBogus<T>()` extension method, which returns an `IBogusFixtureConstructor<T>`. From there, the API mirrors FixtureBuilder's fluent interface — every configuration method has a Faker-accepting overload alongside the standard passthrough.


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
- `WithBackingFieldUntyped(expr, Func<Faker, object?>)`
- `Invoke(Func<Faker, Expression<Action<T>>>)`
- `InvokePrivate(methodName, Func<Faker, object[]>)`
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


## Locale and Seed

Set the locale via the `WithBogus` overload, and the seed via the `Random` property:

```csharp
// German locale
var bogus = Fixture.WithBogus<User>("de");

// Deterministic seed for repeatable test data
bogus.Random = new Randomizer(42);

var user = bogus
    .With(u => u.Name, f => f.Name.FirstName())
    .Build();
```

Both `Locale` and `Random` are available on `IBogusFixtureConstructor<T>` before a construction method is called.


## MemberLens

The `WithField` and `InvokePrivate` methods support the Visual Studio extension [MemberLens](https://marketplace.visualstudio.com/items?itemName=DennisJohnsen.MemberLens), providing autocomplete for field and method names.


## Feedback & Issues

For bug reports, feature requests, or usage questions, please submit an issue on the [GitHub repository](https://github.com/DennisJohnsenUCL/FixtureBuilder).