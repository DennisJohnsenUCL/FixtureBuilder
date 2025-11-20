# FixtureBuilder

**FixtureBuilder** is a flexible fixture creation library for .NET. It enables advanced construction and configuration of test objects, supporting scenarios where you need to set fields (even private or backing fields), initialize read-only properties, or bypass constructors during test setup.


## Features

- Instantiate entities with or without calling their constructors
- Configure entity fields—including private fields or collections
- Set property values, supporting both setters and backing fields
- Cast fixture objects to different types
- Chain configuration methods for fluent test setup


## Quick Example

```csharp

using FixtureBuilder;

// Suppose you have a class:
public class User

{
	public string Name { get; }

	private int _age;

	public User(string name) { Name = name; }

}

// Build a test fixture for User, bypassing the constructor
var fixture = new Fixture<User>()
	.BypassConstructor()
	.WithField("_age", 30)
	.WithSetter(u => u.Name, "Alice")
	.Build();
```


## Core APIs

- `BypassConstructor()`: Instantiates the entity without invoking its constructor.
- 
- `UseConstructor(params object\[] args)`: Instantiates with specific constructor arguments.

- `WithField(string fieldName, object value)`: Sets a field value directly.

- `WithSetter(Expression<Func<TEntity, TProp>> expr, TProp value)`: Sets a writable property via its setter.

- `With<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value)`: Sets either a property (if writable) or its backing field.

- `CastTo<TTarget>()`: Casts fixture to another type for chaining configurations.

- 
## When to Use

- Complex test object setup where constructors enforce constraints

- Setting private fields or initializing read-only properties for tests


## Limitations

- Only intended for use with reference types (`where TEntity : class`)

- Not suitable for production scenarios; designed for testing utilities only


## License

This package is released under the MIT license.


---


## Feedback & Issues

For bug reports, feature requests, or usage questions, please contact the maintainer.
