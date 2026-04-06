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