#### 1.1.1

- New method Instantiate allows for setting a member or a nested member on the test object without providing a specific value.
  - A specific construction method can be chosen with a lambda parameter on Instantiate, allowing for precise control.
  - A default option will be used if no construction method is chosen.
- Expressions may now contain public fields when configuring nested members on the text object, not just properties.
- Fixed circular dependencies causing a stack overflow when constructing objects with UseAutoConstructor or CreateInitialized.
  - The circular dependency is now caught early and an exception is thrown.
- Fixed InvokePrivate throwing an exception for overloaded methods, and failing to find method overlods with optional parameters.
  - The best fitting method is now chosen based on the given parameters.

#### 1.1.0

- Construction method .BypassConstructor has been renamed to .CreateUninitialized.
  - An enum (InitializeMembers) can be provided to control member initialization.
- WithField has been split into two methods as it did two different things depending on arguments provided.
  - WithField now sets the given field on the test object or, if given, a member of the test object.
  - WithBackingField sets the value of the backing field for the given property.
- New method WithBackingFieldUntyped can be used to configure backing fields for properties in cases where the property and backing fields have different (unassignable) types.
- New method Invoke can be used to invoke methods on the test object or its members during the configuration stage.
- New method InvokePrivate can be used to invoke private methods on the test object or its members during the configuration stage.
- New method UseAutoConstructor constructs the test object by invoking the simplest constructor, recursively building dependencies of that constructor.
- Support for the Visual Studio extension MemberLens has been added to methods WithField and InvokePrivate.

#### 1.0.4

- Added .NET 10 as a target framework for compatibility.
- Optimized instantiation of members when constructing fixture.
- Added support for more types of collections when setting backing fields for properties of differing types with WithField.
- Fixed errors related to setting nullable value type fields with WithField.

#### 1.0.3

- Added better detection of nullability of reference types.

#### 1.0.2

- Added support to WithField for properties with backing fields of different collection types.

#### 1.0.1

- Renamed class to Fixture to avoid namespace clashes.

#### 1.0.0

- First version