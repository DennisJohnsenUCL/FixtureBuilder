using FixtureBuilder.UninitializedProviders;

namespace FixtureBuilder.FixtureContexts
{
    internal class FixtureOptions
    {
        public static FixtureOptions Default => new();

        /// <summary>
        /// The default InitializeMembers setting used by CreateUninitialized.
        /// </summary>
        public InitializeMembers InitializeMembersDefault { get; set; } = InitializeMembers.None;
    }
}
