#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder.Core
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public interface ICustomConverter
    {
        /// <summary>
        /// Attempts to convert the given value to the target type.
        /// </summary>
        /// <remarks>
        /// Invoked during backing field assignment when the field type differs from the property type.
        /// Return <c>new NoResult()</c> if the converter does not handle the given conversion. A <c>null</c> return is treated as an explicit null assignment.
        /// </remarks>
        object? Convert(Type target, object value);
    }
}
