using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Core.FixtureContexts
{
    internal abstract class ContextBase : IFixtureContext
    {
        public abstract FixtureOptions Options { get; set; }

        public abstract ConverterGraph Converter { get; }
        public abstract ICompositeTypeLink TypeLink { get; }
        public abstract IUninitializedProvider UninitializedProvider { get; }
        public abstract ICompositeValueProvider ValueProvider { get; }
        public abstract IAutoConstructingProvider AutoConstructingProvider { get; }
        public abstract IConstructingProvider ConstructingProvider { get; }

        public void SetOptions(Action<FixtureOptions> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            action.Invoke(Options);
        }

        public Type UnwrapAndLink(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            type = TypeLink.Link(type) ?? type;
            return type;
        }

        public object? ProvideWithStrategy(FixtureRequest request, InstantiationMethod instantiationMethod, InitializeMembers initializeMembers)
        {
            request.Type = UnwrapAndLink(request.Type);
            var result = ValueProvider.ResolveValue(request, this);
            if (result is not NoResult) return result;

            return InstantiateWithStrategy(request, instantiationMethod, initializeMembers);
        }

        public object InstantiateWithStrategy(FixtureRequest request, InstantiationMethod instantiationMethod, InitializeMembers initializeMembers)
        {
            return instantiationMethod switch
            {
                InstantiationMethod.UseAutoConstructor => AutoConstructingProvider.AutoResolve(request, this),

                InstantiationMethod.UseDefaultConstructor => ConstructingProvider.ResolveWithArguments(request),

                InstantiationMethod.CreateUninitialized => UninitializedProvider.ResolveUninitialized(request, initializeMembers, this)
                    ?? throw new InvalidOperationException($"Failed to instantiate {request.Type.Name} uninitialized."),

                _ => throw new InvalidOperationException($"Invalid instantiation method: {instantiationMethod}.")
            };
        }
    }
}
