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


## MemberLens

Configuration methods WithField and InvokePrivate that take stringly-typed field and method names support the Visual Studio extension [MemberLens](https://marketplace.visualstudio.com/items?itemName=DennisJohnsen.MemberLens). With MemberLens installed, these methods will receive autocomplete suggestions for field names and method names on the test object or its member.


## Feedback & Issues

For bug reports, feature requests, or usage questions, please submit an issue on the Github repository.
