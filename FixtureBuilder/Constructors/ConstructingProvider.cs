using System.Reflection;

namespace FixtureBuilder.Constructors
{
    internal class ConstructingProvider : IConstructingProvider
    {
        public object Resolve(FixtureRequest request, params object[] args)
        {
            try
            {
                return Activator.CreateInstance(
                    type: request.Type,
                    bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    binder: null,
                    args: args,
                    culture: null)!;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create {request.Type.Name} with given parameters.", ex);
            }
        }
    }
}
