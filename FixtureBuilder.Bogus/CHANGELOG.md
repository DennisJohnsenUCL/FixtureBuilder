#### 1.2.0

- Updated FixtureBuilder to 2.0.0.
- Added Fixture.WithBogus\<T>(T instance) method.
  - This method takes an object and configures it using the IBogusFixtureConfigurator.
  - An exception will be thrown if .Build(int count) is called with any count other than 1.

#### 1.1.0

- Added BogusFixtureFactory with methods like .New<>() and .Build() to produce BogusFixture instances. Features include:
  - The factory is instantiated by a static factory method on FixtureFactory: FixtureFactory.WithBogus()
  - Faker overloads for With methods and AddProvider.
  - Locale and Random properties to set these on the internal Faker.
  - Passthroughs for existing FixtureFactory methods.
- Added passthroughs for UseCustomInstantiator to BogusFixture and .Instantiate, including Faker overloads.
- Added CastTo passthrough method to BogusFixture.
- Updated FixtureBuilder dependency to v1.4.0.

#### 1.0.0

- First version