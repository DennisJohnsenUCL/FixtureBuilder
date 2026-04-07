using FixtureBuilder.UninitializedProviders;

namespace FixtureBuilder.FixtureContexts
{
    internal class FixtureOptions
    {
        public FixtureOptions() { }

        public static FixtureOptions Default => new();

        /// <summary>
        /// The default InitializeMembers setting used by CreateUninitialized.
        /// </summary>
        public InitializeMembers InitializeMembersDefault { get; set; } = InitializeMembers.None;

        ///// <summary>
        ///// Whether CreateUnitialized will initialize members recursively, or only the first level.
        ///// </summary>
        //public bool InitializeMembersRecursive { get; set; } = true;

        /// <summary>
        /// Whether UseAutoConstructor will use private constructors.
        /// </summary>
        /// <remarks>
        /// Public constructors will still be preferred. Set PreferSimplestConstructor to always choose simplest constructor regardless of visibility.
        /// </remarks>
        public bool AllowPrivateConstructors { get; set; } = true;
    }
}
