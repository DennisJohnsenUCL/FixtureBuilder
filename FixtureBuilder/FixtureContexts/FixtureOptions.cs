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
        /// <remarks>
        /// Default is <see cref="InitializeMembers.None" />
        /// </remarks>
        public InitializeMembers InitializeMembersDefault { get; set; } = InitializeMembers.None;

        ///// <summary>
        ///// Whether CreateUnitialized will initialize members recursively, or only the first level.
        ///// </summary>
        //public bool InitializeMembersRecursive { get; set; } = true;

        /// <summary>
        /// Whether UseAutoConstructor will use private constructors.
        /// </summary>
        /// <remarks>
        /// Default is <see langword="true" /> <br /><br />
        /// Public constructors will still be preferred. Set PreferSimplestConstructor to always choose simplest constructor regardless of visibility.
        /// </remarks>
        public bool AllowPrivateConstructors { get; set; } = true;

        /// <summary>
        /// Whether UseAutoConstructor will always prefer the simplest constructor regardless of visibility.
        /// </summary>
        /// <remarks>
        /// Default is <see langword="false" /> <br /><br />
        /// AllowPrivateConstructors must be true for private constructors to be considered, even if simpler.
        /// </remarks>
        public bool PreferSimplestConstructor { get; set; } = false;

        /// <summary>
        /// Whether default values for parameters will be used or ignored by UseAutoConstructor.
        /// </summary>
        /// <remarks>
        /// Default is <see langword="true" />
        /// </remarks>
        public bool PreferDefaultParameterValues { get; set; } = true;
    }
}
