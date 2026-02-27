using System.Reflection;
using System.Runtime.CompilerServices;

namespace FixtureBuilder.Helpers
{
    //TODO: Delete static class when autoconstructor is default
    internal static class InstantiationHelper
    {
        public static object GetInstantiatedInstance(Type type)
        {
            var instance = UseConstructor(type) ?? CreateUninitialized(type)
                ?? throw new InvalidOperationException($"Failed to instantiate {type.Name} with default constructor and by bypassing constructor. Please use 'UseConstructor' and supply known constructor parameters as arguments.");

            return instance;
        }

        public static object CreateUninitialized(Type type)
        {
            return RuntimeHelpers.GetUninitializedObject(type);
        }

        public static object? UseConstructor(Type type, params object[] args)
        {
            return Activator.CreateInstance(
                type: type,
                bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                binder: null,
                args: args,
                culture: null);
        }
    }

    //TODO: Do I need this?
    //Dependency of Root, to split responsibility?
    internal class UninitializedProvider : IUninitializedProvider
    {
        public object Resolve(FixtureRequest request)
        {
            try
            {
                return RuntimeHelpers.GetUninitializedObject(request.Type);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create {request.Type.Name} uninitialized.", ex);
            }
        }
    }

    internal interface IUninitializedProvider
    {
        object Resolve(FixtureRequest request);
    }
}
