# FixtureBuilder

**FixtureBuilder** is a flexible fixture creation library for .NET. It enables advanced construction and configuration of test objects, supporting scenarios where you need to set fields (even private or backing fields), initialize read-only properties, or bypass constructors during test setup.

If you want to write clean tests that focus only on the method you want to test, then this is the tool for you. With FixtureBuilder you can construct a test object to be in the exact state you want it to be in for your test, and write tests that are dependent only on the specific method being tested.


## Features

- Instantiate objects with or without calling their constructors, or let FixtureBuilder build the object for you
- Configure object fields—including private fields or collections
- Set property values, supporting both setters and backing fields
- Invoke methods on the test object
- Chain configuration methods for fluent test setup


## Quick Example

```csharp
using FixtureBuilder;

public class User
{
	public string Name { get; }
	private int _age;

	public User(string name) { Name = name; }
}

// Build a test fixture for User, bypassing the constructor
var fixture = new Fixture<User>()
	.CreateUninitialized()
	.WithField("_age", 30)
	.WithSetter(u => u.Name, "Alice")
	.Build();
```


## Core APIs

### Creation
- `CreateUninitialized()`: Instantiates the object without invoking its constructor.

- `UseConstructor(params object[] arguments)`: Instantiates with specific constructor arguments.

- `UseAutoConstructor()`: Instantiates the object by automatically invoking its simplest constructor, recursively building dependencies

### Configuration
- `WithField(string fieldName, object value)`: Sets a field value directly.

- `WithField(Expression<Func<T, TProp>> expr, string fieldName, object value)`: Sets a field value on a member of the object.

- `WithBackingField(Expression<Func<T, TProp>> expr, object value)`: Sets the value of the backing field for a given property.

- `WithSetter(Expression<Func<T, TProp>> expr, TProp value)`: Sets a writable property via its setter.

- `With(Expression<Func<T, TProp>> expr, TProp value)`: Sets either a property (if writable) or its backing field.

- `Invoke(Expression<Action<T>> expr)`: Invokes a method on the test object or its member.

- `InvokePrivate(string methodName, params object?[] arguments)`: Invokes a private method on the test object.

- `InvokePrivate(Expression<Func<T, TProp>> expr, string methodName, params object?[] arguments)`: Invokes a private method on a member of the test object.

- `Instantiate(Expression<Func<T, TProp>> expr)`: Instantiates the given member using the default instantiation method.

- `Instantiate(Expression<Func<T, TProp>> expr, Func<IConstructor<TProp>, TProp>, func)`: Instantiates the given member using the chosen instantiation method via the `IConstructor`.

- `CastTo<TTarget>()`: Casts fixture to another type for chaining configurations.

## FixtureFactory

`FixtureFactory` is a pre-configured fixture producer. Where `Fixture<T>` creates a single test object, `FixtureFactory` lets you define shared configuration once — type mappings, value providers, default values — and then produce consistently configured fixtures from it.

### Basic Usage

```csharp
var factory = new FixtureFactory();
var fixture = factory.New<User>().UseAutoConstructor().Build();
```

### Pre-Configured Values

Use `With` methods to pre-configure values that apply to all fixtures the factory produces. These methods are fluent and can be chained.

```csharp
var factory = new FixtureFactory()
    .With("Alice")                         // All strings receive "Alice"
    .With("Alice", "Name")                 // Only members named "Name" receive "Alice"
    .WithParameter(30)                     // Constructor parameters of type int receive 30
    .WithParameter(30, "age")              // Only the parameter named "age" receives 30
    .WithPropertyOrField("Bob")            // Properties and fields of type string receive "Bob"
    .WithPropertyOrField("Bob", "Name")    // Only the property/field named "Name" receives "Bob"

var user = factory.New<User>().UseAutoConstructor().Build();
```

All `With` methods have overloads that take a func delegate instead of a flat value. These allow for registering factories that generate new values every time they are called.
`With` matches by type across all member kinds (constructor parameters, properties, fields). `WithParameter` and `WithPropertyOrField` restrict matching to specific member kinds. All overloads accepting a name parameter additionally require the member name to match.

### Scoped Configuration

Use `WhenBuilding` to scope pre-configured values to a specific test object type:

```csharp
var factory = new FixtureFactory()
    .WhenBuilding<User>(b => b
        .With<string>("Alice", "name")
        .WithParameter<int>(30, "age"))
    .WhenBuilding<Company>(b => b
        .With<string>("Acme Corp", "name"));

var user = factory.New<User>().UseAutoConstructor().Build();
var company = factory.New<Company>().UseAutoConstructor().Build();
```

Values configured inside `WhenBuilding` only apply when the factory is producing that specific type. This avoids conflicts when different types share parameter names or types.

### Options

Configure factory-wide options either at construction or afterwards:

```csharp
// At construction
var factory = new FixtureFactory(new FixtureOptions { AllowPrivateConstructors = true });

// After construction
factory.Options = new FixtureOptions { AllowPrivateConstructors = true };

// Or modify in place
factory.SetOptions(o => o.AllowPrivateConstructors = true);
```

Options apply to all fixtures produced by the factory.

### Existing Instances

Pass an existing instance to configure and build from:

```csharp
var existing = new User("Alice");
var fixture = factory.New(existing)
    .WithField("_age", 30)
    .Build();
```

### Extensibility

Register custom type links, value providers, and value converters to extend the factory's behavior:

```csharp
// Map an interface to a concrete type
factory.AddTypeLink(myTypeLink);
factory.AddTypeLink<ITypeA, TypeB>();
factory.AddTypeLink(typeof(ITypeA), typeof(TypeB));

// Add a custom value provider
factory.AddProvider(myProvider);

// Add a custom value converter
factory.AddConverter(myConverter);
```

Custom providers implement `ICustomProvider` and custom converters implement `ICustomConverter`. Both are available in the `FixtureBuilder.Core` namespace.

## MemberLens

Configuration methods WithField and InvokePrivate that take stringly-typed field and method names support the Visual Studio extension [MemberLens](https://marketplace.visualstudio.com/items?itemName=DennisJohnsen.MemberLens). With MemberLens installed, these methods will receive autocomplete suggestions for field names and method names on the test object or its member.


## Feedback & Issues

For bug reports, feature requests, or usage questions, please submit an issue on the Github repository.
