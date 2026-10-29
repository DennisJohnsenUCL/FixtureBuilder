#### 3.0.1

- Deprecated the FixtureRequest.Type setter, as requests should not be modified. It will be removed in 4.0.0.
- Fixed construction methods silently overwriting the instance when called more than once on the same Fixture.
  - They now throw an exception instead. This could happen when casting the Fixture back to IFixtureConstructor, or when calling configuration methods separately while keeping it typed as IFixtureConstructor.
- Fixed CreateUninitialized not initializing struct members whose type declares a parameterless constructor.
- Fixed CreateUninitialized calling overridden Equals methods on struct members when checking for default values, which could give incorrect results or throw exceptions.
- Fixed CreateUninitialized failing for properties with setters that throw on invocation.
  - Such properties are now left unchanged.
- Fixed member expressions failing for nullable struct members chained through .Value, e.g. x => x.NullableStruct.Value.Property.
- Fixed member expressions ending in .Value on a nullable struct throwing inconsistently across configuration methods.
  - They now always throw an exception pointing to the nullable member itself.
- Fixed UseConstructor in .Instantiate throwing for nullable struct members.
- Fixed errors in XML documentation.

#### 3.0.0

- Breaking change: FixtureFactory.New with an instance given now returns IFixtureConfigurator instead of IFixtureConstructor.
  - The instance already exists, so construction methods no longer apply to it.
- Breaking change: InvokePrivate now rethrows exceptions thrown by the invoked method as they are, rather than wrapped in a TargetInvocationException.
- Added providers for bool, char, Delegate, Task and Task<T>.
  - bool members are set to true, and char members to a random ASCII letter.
  - Delegates ignore their arguments and return the default value of their return type.
  - Task members are set to a completed task, and Task<T> members to a completed task holding the default value of T.
- Added OriginalType to FixtureRequest, holding the declared type of the parameter, property or field before nullable unwrapping and type linking.
- Fixed providers, converters and type links registered on FixtureFactory after a .WhenBuilding block overriding the root type specific ones.
  - Registrations made in .WhenBuilding now always take precedence, regardless of registration order.
- Fixed setting root options more than once for the same type throwing an exception.
  - .SetOptions in .WhenBuilding now builds on the options already set for that root type.
- Fixed TypeLink not throwing on registration when the output type is not assignable to the input type.
- Fixed .WithX providers not being used for nullable value types or types with a type link.
- Fixed Guids never being provided, as their provider was not registered.
- Fixed providers returning null for nullable value types throwing an exception.
- Fixed UseAutoConstructor failing for types with constructors taking a parameter of their own type.
  - Such constructors are tried last, and skipped unless that parameter is nullable and PreferNullParameterValues is enabled, or has a default value and PreferDefaultParameterValues is enabled.
- Fixed UseAutoConstructor throwing for structs with no constructors.
- Fixed UseAutoConstructor and CreateUninitialized in .Instantiate not unwrapping nullable types or applying type links.
- Fixed UseCustomInstantiator not throwing when the instantiator returns null.
- Fixed ref, out and in parameters not being unwrapped before nullable checking, type linking and default parameter values.
- Fixed PreferNullParameterValues checking parameter nullability through ReadState rather than WriteState, so [AllowNull] and [DisallowNull] were not respected.
- Fixed struct parameters with a default value of default being given null when PreferDefaultParameterValues is enabled.
- Fixed nullable enum parameters with default values being given the underlying numeric value rather than the enum value.
- Fixed enums with no members throwing an exception. They now get the value 0.
- Fixed CreateUninitialized silently skipping interfaces and abstract types with no type link or provider, rather than throwing.
- Fixed CreateUninitialized not setting get-only auto-properties.
- Fixed CreateUninitialized failing for properties with getters that throw on invocation. Such properties are now left unchanged.
- Fixed CreateUninitialized not initializing private fields of base classes.
- Fixed CreateUninitialized treating types and members from namespaces that merely start with System or Microsoft (e.g. SystemTools) as framework types.
- Fixed CreateUninitialized circular dependency tracking misidentifying sibling members of the same type as circular dependencies.
- Fixed AllowResolveCircularDependencies not copying private fields of base classes into the resolved circular references.
- Fixed AllowResolveCircularDependencies leaving already resolved circular references registered in the rest of the object hierarchy.
- Fixed WithField not finding private fields from base classes.
- Fixed WithField and InvokePrivate on nested members using the declared type of the member rather than its runtime type.
- Fixed InvokePrivate not invoking private methods of base classes.
- Fixed WithBackingField not handling explicit interface implementations, generics and complex override hierarchies correctly.
- Fixed backing field names being lowercased incorrectly in Turkish culture.
- Fixed member expressions with struct members silently failing to write to them.
  - Changes to structs along the expression are now written back to their parent. A changed struct held by a property without a setter throws an exception instead.
- Fixed WithSetter naming the type rather than the property when the property is not writable.
- Fixed CastTo not passing the FixtureFactory configuration along to the new Fixture instance.
- Fixed conversion to Collection and ReadOnlyCollection failing when the source is not an IList.
- Fixed conversion failing for empty dictionaries.

#### 2.0.0

- Added new option DefaultInstantiationMethod to FixtureOptions which, if set, will override other InstantiationMethod options (fixture, chain, .Instantiate).
  - Old DefaultInstantiationMethod option has been renamed to DefaultFixtureInstantiationMethod.
- RootConfigurationBuilder methods now always return the builder so method calls inside the .WhenBuilding() lambda can be chained.
- Breaking change: Moved ICustomTypeLink to FixtureBuilder.Core namespace.
- Breaking change: Moved InstantiationMethod enum to FixtureBuilder.Core namespace.
- Breaking change: Moved InitializeMembers to FixtureBuilder namespace.
- Moved RootConfigurationBuilder to FixtureBuilder namespace.

#### 1.4.0

- Added construction method UseCustomInstantiator.
  - This method will construct the fixture object using any user-provided constructor, factory method, or factory class.
- Added UseCustomInstantiator method to IConstructor, exposed via Fixture's .Instantiate method to instantiate members and nested members.
  - Accepts any user-provided constructor, factory method, or factory class.

#### 1.3.1

- Fixed XML doc summaries not being included with the NuGet package.

#### 1.3.0

- Added .Build method to FixtureFactory to instantly output a test object with only pre-configuration through the factory.
- Added option AllowResolveCircularDependencies to allow .UseAutoConstructor and .CreateUninitialized methods to resolve circular dependencies rather than throwing.
- Added option AllowImplicitConversion to allow WithBackingField to apply implicit conversions for properties with backing fields of different types.
- Added new ways to customize FixtureFactory for specific root object types with FixtureFactory.WhenBuilding:
  - .SetOptions method allows setting root object type specific options on top of already defined global options.
  - .Options allows overriding global options entirely with root object type specific options.
  - .AddProvider allows for registering custom providers for specific root object types.
  - .AddConverter allows for registering custom converters for specific root object types.
  - .AddTypeLink allows for registering custom type links for specific root object types.
- Added summaries everywhere they were missing.

#### 1.2.1

- Added overloads for .AddTypeLink on FixtureFactory that take the input type and output type directly as parameters.
- Added overloads for .WithX methods on FixtureFactory that take funcs instead of a flat value.
  - These allow for adding factories that generate values when needed rather than providing a static value.
- Added .WhenBuilding method to FixtureFactory which allows for adding .WithX providers that apply only when building a test object of a given type.

#### 1.2.0

- Added the new major feature FixtureFactory. new FixtureFactory() will provide a factory instance that can be used to set options for and preconfigure Fixture instances.
  - .SetOptions allows for configuring options used by Fixture instances, allowing users to precisely calibrate the behavior of Fixture.
  - .AddTypeLink allows for adding TypeLinks, which are used to tell Fixture which concrete classes to use when it encounters an interface.
  - .AddConverter allows for adding converters that are used when Fixture encounters a backing field with an unassignable type relative to its property.
  - A series of .WithX methods allows for adding templated providers that are used to populate parameters in constructors, and properties and fields on test objects.
    - This allows for precisely controlling the state of test objects generated by Fixture.
  - .AddProvider allows for adding a fully custom provider which the user can tailor to their exact needs.
  - .New outputs a new Fixture instance for test object generation with the options and additions to the FixtureFactory.

#### 1.1.1

- New method Instantiate allows for setting a member or a nested member on the test object without providing a specific value.
  - A specific construction method can be chosen with a lambda parameter on Instantiate, allowing for precise control.
  - A default option will be used if no construction method is chosen.
- Expressions may now contain public fields when configuring nested members on the test object, not just properties.
- Fixed circular dependencies causing a stack overflow when constructing objects with UseAutoConstructor or CreateUninitialized.
  - The circular dependency is now caught early and an exception is thrown.
- Fixed InvokePrivate throwing an exception for overloaded methods, and failing to find method overloads with optional parameters.
  - The best fitting method is now chosen based on the given parameters.

#### 1.1.0

- Construction method .BypassConstructor has been renamed to .CreateUninitialized.
  - An enum (InitializeMembers) can be provided to control member initialization.
- WithField has been split into two methods as it did two different things depending on arguments provided.
  - WithField now sets the given field on the test object or, if given, a member of the test object.
  - WithBackingField sets the value of the backing field for the given property.
- New method WithBackingFieldUntyped can be used to configure backing fields for properties in cases where the property and backing field have different (unassignable) types.
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