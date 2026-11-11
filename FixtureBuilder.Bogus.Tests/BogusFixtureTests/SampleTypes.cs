namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    #region Methods

    internal class RecordingMethodClass
    {
        public string Recorded { get; private set; } = null!;

        public void Record(string value) => Recorded = value;
    }

    #endregion
}
