#### 2.0.0

- Updated FixtureBuilder to 3.0.0.
- Breaking change: Removed the Random setter from BogusFixture.
  - For fixtures created from a BogusFixtureFactory, setting it also changed the Random on the factory and every other fixture created from it.
  - Set Random on BogusFixtureFactory instead to seed generated data.
- Breaking change: Removed the Locale setters from BogusFixture and BogusFixtureFactory.
  - They had no effect, as changing the locale of a Faker after it is created does nothing.
  - Set the locale with the Fixture.WithBogus and FixtureFactory.WithBogus overloads instead.
- Fixed .Build(int count) throwing InvalidOperationException rather than ArgumentOutOfRangeException for counts below 1 when the BogusFixture was created from an instance.
- Fixed CastTo on a BogusFixture created from an instance allowing .Build(int count) to return the same instance several times.
  - It now throws InvalidOperationException for counts above 1, as it does before casting.
- Fixed errors and outdated information in XML doc summaries.

#### 1.2.0

- Updated FixtureBuilder to 2.0.0.
- Added Fixture.WithBogus\<T>(T instance) method.
  - This method takes an object and configures it using the IBogusFixtureConfigurator.
  - An exception will be thrown if .Build(int count) is called with any count other than 1.

#### 1.1.0

- Added BogusFixtureFactory with methods like .New<>() and .Build() to produce BogusFixture instances. Features include:
  - The factory is instantiated by a static factory method on FixtureFactory: FixtureFactory.WithBogus().
  - Faker overloads for With methods, and AddBogusProvider.
  - Locale and Random properties to set these on the internal Faker.
  - Passthroughs for existing FixtureFactory methods.
- Added passthroughs for UseCustomInstantiator to BogusFixture and .Instantiate, including Faker overloads.
- Added CastTo passthrough method to BogusFixture.
- Updated FixtureBuilder dependency to v1.4.0.

#### 1.0.0

- First version