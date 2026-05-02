namespace FixtureBuilder.Core
{
    /// <summary>
    /// Represents the absence of a result. This is used by providers and converters
    /// to indicate that they cannot process the current request or provide a valid value.
    /// Evaluating pipeline components check for this object to delegate responsibility to the next capable component.
    /// You must return an object of this type if constructing an ICustomProvider or ICustomConverter rather than null when your provider does not handle the incoming request.
    /// </summary>
    public sealed class NoResult;
}
