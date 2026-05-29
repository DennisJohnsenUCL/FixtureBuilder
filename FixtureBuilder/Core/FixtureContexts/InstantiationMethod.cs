#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder.Core
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Specifies the strategy to use when instantiating objects, such as fixtures, members, or instances created along an expression path.
    /// </summary>
    public enum InstantiationMethod
    {
        /// <summary>
        /// Objects will be instantiated using a parameterized constructor to maximize automatic initialization.
        /// </summary>
        UseAutoConstructor,

        /// <summary>
        /// Objects will be instantiated preferring the default (parameterless) constructor if one exists.
        /// </summary>
        UseDefaultConstructor,

        /// <summary>
        /// Objects will be created in an uninitialized state, bypassing constructors entirely.
        /// </summary>
        CreateUninitialized
    }
}
