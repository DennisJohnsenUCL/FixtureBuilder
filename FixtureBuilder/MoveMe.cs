using FixtureBuilder.FixtureContexts;
using FixtureBuilder.TypeLinks;

namespace FixtureBuilder
{
    internal class ValidatingTypeLink : ITypeLink
    {
        private readonly ITypeLink _inner;

        public ValidatingTypeLink(ITypeLink inner)
        {
            ArgumentNullException.ThrowIfNull(inner);

            _inner = inner;
        }

        public Type? Link(Type target)
        {
            ArgumentNullException.ThrowIfNull(target);

            return _inner.Link(target);
        }
    }

    internal interface IXXCreator
    {
        object? Create(Type type, IFixtureContext context);
    }

    //Creator, Provider, Generator
    //X is ??
    //Same interface for this and CreateUnitialized
    //Different interface/not even on context for UseConstructor(params object[] args)
    internal class UseConstructorAuto() : IXXCreator
    {
        public object? Create(Type type, IFixtureContext context)
        {
            ArgumentNullException.ThrowIfNull(type);

            type = context.Link(type) ?? type;

            var constructors = type.GetConstructors();
            var constructor = constructors.MinBy(ctor => ctor.GetParameters().Length);
            if (constructor == null) return null;

            var parameterTypes = constructor.GetParameters();

            List<object> parameters = [];

            foreach (var paramInfo in parameterTypes)
            {
                if (paramInfo.IsOptional) continue;
                var paramType = paramInfo.ParameterType;
                //var param = context.CreateXX(paramType);
                //parameters.Add(param);
            }

            return constructor.Invoke([.. parameters]);
        }
    }

    internal class XXName
    {
        public Type Type { get; set; }
        public object? RequestSource { get; }

        public XXName(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);
            Type = type;
        }

        public XXName(Type type, object requestSource)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(requestSource);
            Type = type;
            RequestSource = requestSource;
        }
    }
}
