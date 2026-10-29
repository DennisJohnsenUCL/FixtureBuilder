#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder.Core
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Defines a custom value converter for backing field assignment.
    /// </summary>
    public interface ICustomConverter
    {
        /// <summary>
        /// Attempts to convert the given value to the target type.
        /// </summary>
        /// <remarks>
        /// Invoked during backing field assignment when the field type differs from the property type.
        /// Return <see cref="NoResult.Instance"/> if the converter does not handle the given conversion. A <see langword="null"/> return is treated as an explicit null assignment.
        /// </remarks>
        object? Convert(Type target, object value);
    }
}
