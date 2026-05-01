namespace FixtureBuilder.Creation.UninitializedProviders
{
    /// <summary>
    /// Specifies the configuration for automatically initializing uninitialized members of a created fixture when calling .CreateUninitialized.
    /// </summary>
    public enum InitializeMembers
    {
        /// <summary>
        /// No members should be initialized automatically.
        /// </summary>
        None,

        /// <summary>
        /// Only non-nullable references and value types will be initialized.
        /// </summary>
        NonNullables,

        /// <summary>
        /// All potential members (including nullables) will be initialized.
        /// </summary>
        All
    }
}
